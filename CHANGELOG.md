# PasteBinIngest Change Log

## VERSION 2.2.1

- more docker changes.
- linux docker container was failing due to file slashes.
- removed timezone setting within container, this can be handled by users.
- updated deployment scripts.

## VERSION 2.2.0

- major docker changes.
- updates docker-compose and docker scripts.
- addressed some issues.

## VERSION 2.1.2

- Dockerfile for the ingest was missing cron stuff...
- updated scripts.

## VERSION 2.1.1

- added docker support.
- added docker-compose.yml deployment file.
- added docker helper scripts.
- changed log and data locations to use the root path of the executable (for docker support).

## VERSION 2.1.0

- complete project refactor
- many renames and some code clean up
- updated README.md

## VERSION 2.0.0

- fixed existing data check
- changed implementation of getting request data to make it faster
- created script to clean up and remove duplicate entry data
- this script will load all existing data into memory, loop through all the data, and remove any duplicates. it will then save the data back to disk
- finished adding API controllers
- added/updated documentation

## VERSION 1.0.1

- started adding API controllers
- started to rethink data save structure due to slow data retrieval
- created script to adjust existing data into new format
- after data structure changes, data retrieval is way faster
- data changes broke existing data check, this caused every request to save every entry
- data grew exponentially due to broken existing data check

## VERSION 1.0.0

- major changes made as problems were found in async calls
- program works from here on...
- changed/updated/improved logging

## VERSION 0.2.0

- made more async <- breaking change
- refactored the service and repository
- changed/updated/improved logging

## VERSION 0.1.0

- initial creation of sele (serve-file) project
- created source code repository
- not much has changed when you have just started
- project can ingest pastebin data and store it into a file
