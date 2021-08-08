docker start mssql-server-gigs
$env:ASPNETCORE_ENVIRONMENT="Development"
Set-Location ./src
libman restore
Set-Location ../
dotnet run -p ./src
