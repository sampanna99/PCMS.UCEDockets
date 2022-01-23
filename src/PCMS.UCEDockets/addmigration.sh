#!/bin/bash

dotnet ef migrations add $1 --context SqlServerUCEDocketsContext --output-dir Migrations/SqlServerMigrations
dotnet ef migrations add $1 --context SqliteUCEDocketsContext --output-dir Migrations/SqliteMigrations