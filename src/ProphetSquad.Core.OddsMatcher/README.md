# Matching algorithm

The purpose of this app is to automatically build a link between a fixture and it's
corresponding odds

## Algorythm steps

1. Match by id:
    ``` SQL
    [odds.CompetitionId] == [match.CompetitionId] AND
    [odds.HomeTeamId] == [match.HomeTeamId] AND
    [odds.AwayTeamId] == [match.AwayTeamId] AND
    [odds.MatchDateTime] == [match.DateTime]
    ```
1. Match by competition name and team ids:
    ``` SQL
    [odds.CompetitionName] == [match.CompetitionName] AND
    [odds.HomeTeamId] == [match.HomeTeamId] AND
    [odds.AwayTeamId] == [match.AwayTeamId] AND
    [odds.MatchDateTime] == [match.DateTime]
    ```
1. Match by competition id and home team id:
    ``` SQL
    [odds.CompetitionId] == [match.CompetitionId] AND
    [odds.HomeTeamId] == [match.HomeTeamId] AND
    [odds.MatchDateTime] == [match.DateTime]
    ```
1. Match by competition id and away team id:
    ``` SQL
    [odds.CompetitionName] == [match.CompetitionName] AND
    [odds.AwayTeamId] == [match.AwayTeamId] AND
    [odds.MatchDateTime] == [match.DateTime]
    ```
1. Match by competition id and team names:
    ``` SQL
    [odds.CompetitionName] == [match.CompetitionName] AND
    [odds.HomeTeamId] == [match.HomeTeamName] AND
    [odds.AwayTeamId] == [match.AwayTeamName] AND
    [odds.MatchDateTime] == [match.DateTime]
    ```
