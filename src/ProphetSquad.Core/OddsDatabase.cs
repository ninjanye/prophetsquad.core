using System;
using ProphetSquad.Core.Data.Models;

namespace ProphetSquad.Core
{
    public interface IOddsDatabase
    {
        void Save(MatchOdds odds);   
    }

    public class OddsDatabase : IOddsDatabase
    {
        IDatabaseConnection _connection;
        
        public OddsDatabase(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        private const string mergesql = @"
BEGIN TRAN;
    WITH data as (SELECT @Id as Id)
    MERGE MatchOdds mo
    USING data d on d.Id = mo.Id
    WHEN MATCHED 
        THEN UPDATE
            SET mo.HomeTeamOdds = @HomeTeamOdds,
                mo.HomeTeamOddsDecimal = @HomeTeamOddsDecimal, 
                mo.AwayTeamOdds = @AwayTeamOdds, 
                mo.AwayTeamOddsDecimal = @AwayTeamOddsDecimal,
                mo.LastUpdate = GETDATE()
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (Id,CompetitionId,CompetitionName,[Date],
                    HomeTeamId,HomeTeamName,HomeTeamOdds,
                    HomeTeamOddsDecimal,AwayTeamId,AwayTeamName,
                    AwayTeamOdds,AwayTeamOddsDecimal,LastUpdate,Processed)
             VALUES (@Id,@CompetitionId,@CompetitionName,@Date,
                    @HomeTeamId,@HomeTeamName,@HomeTeamOdds,
                    @HomeTeamOddsDecimal,@AwayTeamId,@AwayTeamName,
                    @AwayTeamOdds,@AwayTeamOddsDecimal,@LastUpdate,@Processed);
COMMIT TRAN;";

        public void Save(MatchOdds odds)
        {
            _connection.Execute(mergesql, odds);
        }
    }
}