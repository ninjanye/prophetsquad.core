# `ProphetSquad.Core.Importer`

This project holds each of the azure functions for azure.  Each function runs on a timer.

## TODO

### Background Workers

* ~Remove `AuthToken` from local settings (BEFORE COMMITING TO GITHUB)~
* ~Correctly namespace `ProphetSquad.Core` project~
* ~Upload Competition/Team Importer (and remove old webjob)~
* ~Move `ProphetSquad.Core.OddsMatcher` to Azure function and upload (and remove old webjob)~
* ~Move `ProphetSquad.Core.FixtureUpdater` to Azure function and upload (and remove old webjob)~
* ~Build Import league tables functionality~
* ~OddsMatcher to not process odds for a game that has begun~
* ~Move `ProphetSquad.Core.Updater` (OddsUpdater) to Azure function and upload (and remove old webjob)~
* Build Process Points functionality
* Upgrade from Dapper to EFCore

### Site (larger)

* ~Remove Openfooty logos and mentions~
* Build AccuLeague API for site to use
* Convert site to vue.js at a component by component level
