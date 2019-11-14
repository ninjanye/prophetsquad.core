using ProphetSquad.Core.Data.Models;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Databases
{
    public class StandingsDatabase : IDatabase<Standing>
    {
        private readonly IDatabaseConnection _connection;
        private readonly IDatabase<Team> _teamDb;
        private readonly IDatabase<Competition> _competitionDb;
        private const string mergeSql = @"
BEGIN TRAN;
    WITH data as (SELECT @CompetitionId as CompetitionId, @TeamId as TeamId)
    MERGE CompetitionPositions cp
    USING data d on d.CompetitionId = cp.CompetitionId
                AND d.TeamId = cp.TeamId
    WHEN MATCHED 
        THEN UPDATE SET 
            cp.Played = @Played,
            cp.Wins = @Wins,
            cp.Draws = @Draws,
            cp.Losses = @Losses,
            cp.GoalsFor = @GoalsFor,
            cp.GoalsAgainst = @GoalsAgainst,
            cp.Points = @Points,
            cp.Form = @Form
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (CompetitionId,TeamId,Played,Wins,Draws,Losses,GoalsFor,GoalsAgainst,Points,Form)
             VALUES (@CompetitionId,@TeamId,@Played,@Wins,@Draws,@Losses,@GoalsFor,@GoalsAgainst,@Points,@Form);
COMMIT TRAN;";

        public StandingsDatabase(IDatabaseConnection connection, IDatabase<Team> teamDb, IDatabase<Competition> competitionDb)
        {
            _connection = connection;
            _teamDb = teamDb;
            _competitionDb = competitionDb;
        }

        public Task<Standing> GetBySourceId(int id)
        {
            throw new System.NotImplementedException();
        }

        public void Save(Standing standing)
        {
            var team = _teamDb.GetBySourceId(standing.SourceTeamId).Result;
            standing.TeamId = team.Id;
            var competition = _competitionDb.GetBySourceId(standing.SourceCompetitionId).Result;
            standing.CompetitionId = competition.Id;
            _connection.Execute(mergeSql, standing);
        }
    }
}