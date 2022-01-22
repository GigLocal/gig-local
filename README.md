# Gig Local

[![CI/CD](https://github.com/GigLocal/gig-local-backend/actions/workflows/cicd.yml/badge.svg)](https://github.com/GigLocal/gig-local-backend/actions/workflows/cicd.yml)

Gig Local website, an ASP.NET Core web app.

## Getting started

1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop) for your system.
2. Install latest [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) for your system.
3. Install [PowerShell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.1) for your system.
4. Run the following commands to add user secrets (ask @marcusturewicz for the secret values):
    - `dotnet user-secrets set Storage:ConnectionString "<SecretValue>"`
    - `dotnet user-secrets set Authentication:Google:ClientId "<SecretValue>"`
    - `dotnet user-secrets set Authentication:Google:ClientSecret "<SecretValue>"`
    - `dotnet user-secrets set Authentication:Admin:Emails "<SecretValue>"`
    - `dotnet user-secrets set Recaptcha:SiteKey "<SecretValue>"`
    - `dotnet user-secrets set Recaptcha:SecretKey "<SecretValue>"`
5. Install the following .NET tools:
    - `dotnet tool install -g dotnet-ef --version 6.0.1`
    - `dotnet tool install -g Microsoft.Web.LibraryManager.Cli --version 2.1.113`

## Running locally

1. Start Docker and wait for it to be ready
2. If this is the first time you're running the app, run `./create-local-db.ps1`
3. Run `./run.ps1`

## License

Gig Local source code is licensed under the [MIT License](LICENSE).
