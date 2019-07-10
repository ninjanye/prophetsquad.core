using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core.Matcher
{
    public interface IFixturesDatabase
    {
        void Save(Fixture fixture);
    }

    public class FixtureDatabase : IFixturesDatabase, IFixtureProvider
    {
        private readonly IDatabaseConnection _connection;
        private const string mergeSql = @"
BEGIN TRAN;
    WITH data as (SELECT @Id as Id)
    MERGE Matches m
    USING data d on d.Id = m.Id
    WHEN MATCHED 
        THEN UPDATE
            SET m.MatchOddsId = @MatchOddsId
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (OpenFootyId,CompetitionId,GameweekId,Date,
                    HomeTeamId,HomeTeamScore,AwayTeamId,AwayTeamScore,
                    WinnerId,IsResult,MatchOddsId,Processed,ModelState,Created)
             VALUES (@OpenFootyId,@CompetitionId,@GameweekId,@Date,
                    @HomeTeamId,@HomeTeamScore,@AwayTeamId,@AwayTeamScore,
                    @WinnerId,@IsResult,@MatchOddsId,@Processed,@ModelState,GETDATE());
COMMIT TRAN;";

        public FixtureDatabase(IDatabaseConnection connection)
        {
            _connection = connection;
        }        
        public void Save(Fixture fixture)
        {
            _connection.Execute(mergeSql, fixture);
        }

        public async Task<IEnumerable<Fixture>> Retrieve(DateTime from, DateTime to)
        {
            const string selectStatement = "SELECT * FROM Matches WHERE Date >= @from AND Date < @to";

            return await _connection.Query<Fixture>(selectStatement, new { from, to });
        }
    }
}