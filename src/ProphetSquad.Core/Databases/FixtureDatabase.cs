using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Databases
{
    public class FixtureDatabase : IFixturesDatabase, IFixtureProvider
    {
        private readonly IDatabaseConnection _connection;
        private const string saveFixture = @"
BEGIN TRAN;
    WITH data as (SELECT @OpenFootyId as OpenFootyId)
    MERGE Matches m
    USING data d on d.OpenFootyId = m.OpenFootyId
    WHEN MATCHED 
        THEN UPDATE SET
            m.MatchOddsId = COALESCE(@MatchOddsId, m.MatchOddsId),
            m.HomeTeamScore = @HomeTeamScore,
            m.AwayTeamScore = @AwayTeamScore,
            m.IsResult = @IsResult,
            m.WinnerId = @WinnerId
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (OpenFootyId,CompetitionId,GameweekId,Date,
                    HomeTeamId,HomeTeamScore,AwayTeamId,AwayTeamScore,
                    WinnerId,IsResult,MatchOddsId,Processed,ModelState,Created)
             VALUES (@OpenFootyId,@CompetitionId,@GameweekId,@Date,
                    @HomeTeamId,@HomeTeamScore,@AwayTeamId,@AwayTeamScore,
                    @WinnerId,@IsResult,@MatchOddsId,@Processed,@ModelState,GETDATE());
COMMIT TRAN;";

        private const string saveCompetition = @"
BEGIN TRAN;
    WITH data as (SELECT @CompetitionId as CompetitionId)
    MERGE Competitions c
    USING data d on d.CompetitionId = c.Id
    WHEN MATCHED 
        THEN UPDATE SET c.BookieId = @BookieId;
COMMIT TRAN;";

        private const string saveTeam = @"
BEGIN TRAN;
    WITH data as (SELECT @Id as Id)
    MERGE Teams t
    USING data d on d.Id = t.Id
    WHEN MATCHED 
        THEN UPDATE SET t.BookieName = @BookieName;
COMMIT TRAN;";

        public FixtureDatabase(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Save(Fixture fixture)
        {
            _connection.Execute(saveFixture, fixture);
            _connection.Execute(saveCompetition, new { fixture.CompetitionId, fixture.Competition.BookieId });
            _connection.Execute(saveTeam, new { fixture.HomeTeam.Id, fixture.HomeTeam.BookieName });
            _connection.Execute(saveTeam, new { fixture.AwayTeam.Id, fixture.AwayTeam.BookieName });
        }

        const string selectStatement = @"
SELECT m.*, c.*, ht.*, at.* 
FROM Matches m
INNER JOIN Competitions c ON m.CompetitionId = c.Id
INNER JOIN Teams ht ON m.HomeTeamId = ht.Id
INNER JOIN Teams at ON m.AwayTeamId = at.Id
WHERE Date >= @from AND Date < @to";

        public async Task<IEnumerable<Fixture>> Retrieve(DateTime from, DateTime to)
        {
            string splitOn = "Id, Id, Id";
            return await _connection.Query<Fixture, Competition, Team, Team, Fixture>(selectStatement,
                (match, competition, homeTeam, awayteam) =>
                {
                    match.Competition = competition;
                    match.HomeTeam = homeTeam;
                    match.AwayTeam = awayteam;
                    return match;
                }, new { from, to }, splitOn);
        }

        public async Task<Fixture> RetrieveBySourceId(int id)
        {
            const string selectStatement = "SELECT * FROM Matches WHERE OpenFootyId = @id";
            return await _connection.QuerySingle<Fixture>(selectStatement, new { id });
        }
    }
}