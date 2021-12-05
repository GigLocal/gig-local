# Gig Local - Backend

[![CI/CD](https://github.com/GigLocal/gig-local-backend/actions/workflows/cicd.yml/badge.svg)](https://github.com/GigLocal/gig-local-backend/actions/workflows/cicd.yml)

This is the repository for the Gig Local Backend; an ASP.NET Core web app.

## Getting started

1. Install latest [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
2. Install [PowerShell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.1)
2. Install [Docker Desktop](https://www.docker.com/products/docker-desktop)
3. Add the [dotnet user secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=linux#set-a-secret). Please request these from a repository owner.
    - Azure Storage Connection String: `Storage:ConnectionString`
    - Google Authentication client id: `Authentication:Google:ClientId`
    - Google Authentication client secret: `Authentication:Google:ClientSecret`
    - Google email address: `Authentication:Admin:Emails`

## Running locally

1. Start Docker and wait for it to be ready
2. Start `./run.ps1` from PowerShell

## License

Gig Local source code is licensed under the [MIT License](LICENSE).
