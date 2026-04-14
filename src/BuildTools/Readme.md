# BuildTools

A self-contained CLI-driven build and configuration pipeline built on top of the [IDFCR](../../README.md) framework. BuildTools provides interactive management of **environments**, **settings**, and **packages** stored in a SQL Server database, as well as a database migration tool for schema lifecycle management.

---

## Table of Contents

- [Projects](#projects)
- [Prerequisites](#prerequisites)
- [Local Setup](#local-setup)
  - [User Secrets](#user-secrets)
- [Running the Database Updater](#running-the-database-updater)
- [Running the CLI](#running-the-cli)
  - [environment commands](#environment-commands)
  - [setting commands](#setting-commands)
  - [package commands](#package-commands)
- [Database Schema](#database-schema)

---

## Projects

| Project | Purpose |
|---|---|
| `BuildTools.Shared` | Common DTOs and query models shared across layers |
| `BuildTools.Infrastructure` | Abstract repository contracts and `DbSettings` configuration model |
| `BuildTools.Infrastructure.SqlServer` | EF Core 10 / SQL Server implementations of all repositories; owns EF migrations |
| `BuildTools.DatabaseUpdater` | Console app — applies pending EF Core migrations against the target database |
| `BuildTools.Cli` | Interactive CLI console app — manages environments, settings, and packages |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server instance (local or remote)
- `dotnet user-secrets` (included with the .NET SDK)

---

## Local Setup

### User Secrets

Both `BuildTools.DatabaseUpdater` and `BuildTools.Cli` are configured to load connection details from [.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) in development.

Initialise secrets for the project you want to run:

````````
dotnet user-secrets init
````````

Then populate the secrets file (typically at `%APPDATA%\Microsoft\UserSecrets\<user-secrets-id>\secrets.json`) with the following structure:

````````
{
  "ConnectionStrings": {
    "<connection-name>":"MultipleActiveResultsets=true;TrustServerCertificate=true"
  },
  "Server:"localhost,<port>",
  "InitialCatalog":"<db-name>",
  "UserId:"sa",
  "Password:"<password>",
  "DefaultConnectionString":"<connection-name>"
}
````````

> To generate a different user, use `dotnet user-secrets set UserId --prompt`.
> To generate a strong password, use `dotnet user-secrets set Password --prompt`.
