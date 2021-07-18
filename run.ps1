docker start mssql-server-gigs
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run -p src
