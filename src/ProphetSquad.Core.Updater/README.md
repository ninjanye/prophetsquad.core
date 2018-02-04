# Common Tasks

## Deploying to azure function

1. Create a self contained exe of the project

`cd src/ProphetSquad.Core.Updater`  
`dotnet publish --self-contained -r win10-x64 -c Released`

2. In Azure, upload published files to the `RetrieveBetfairOdds` function in `OddUpdater` function