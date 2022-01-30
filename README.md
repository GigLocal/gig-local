# Gig Local

[![CI/CD](https://github.com/GigLocal/gig-local-backend/actions/workflows/cicd.yml/badge.svg)](https://github.com/GigLocal/gig-local-backend/actions/workflows/cicd.yml)

Gig Local website, an ASP.NET Core web app.

## Getting started

1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop).
2. Install [VS Code](https://code.visualstudio.com/download).
3. Install [Remote Development extension pack](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.vscode-remote-extensionpack) for VS Code.
4. Install latest [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0).
6. Generate a self-signed HTTPS certificate for your system:

    - Windows (PowerShell):
        ```
        dotnet dev-certs https --trust; dotnet dev-certs https -ep "$env:USERPROFILE/.aspnet/https/aspnetapp.pfx" -p "SecurePwdGoesHere"
        ```
	- macOS/Linux terminal:
        ```
        dotnet dev-certs https --trust; dotnet dev-certs https -ep "${HOME}/.aspnet/https/aspnetapp.pfx" -p "SecurePwdGoesHere"
        ```

7. Get a copy of the `secrets.json` file from the repository owner, and place at:
    - For Mac/Linux: `~/.microsoft/usersecrets/91188907-dc75-4aae-8f00-39a6949edd1a/secrets.json`.
    - For Windows: `~/Microsoft/UseSecrets/91188907-dc75-4aae-8f00-39a6949edd1a/secrets.json`.

## Running locally

1. Start Docker and wait for it to be ready
2. Clone/Open this repository in VS Code
3. Open the command pallete (F1) and run `Remote-Containers: Reopen in Container` and wait for container to be built
4. F5 to run the website

## License

Gig Local source code is licensed under the [MIT License](LICENSE).
