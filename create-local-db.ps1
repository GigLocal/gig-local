docker stop mssql-server-gigs
docker rm mssql-server-gigs
docker pull mcr.microsoft.com/mssql/server:2017-CU20-ubuntu-16.04
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=bc21765a-de25-4720-984c-5831dae8234c" `
   -p 1433:1433 --name mssql-server-gigs `
   -d mcr.microsoft.com/mssql/server:2017-CU20-ubuntu-16.04

Start-Sleep -s 30

docker exec -i mssql-server-gigs /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'bc21765a-de25-4720-984c-5831dae8234c' -Q 'CREATE DATABASE gigs'

dotnet ef database update -p ./src -s ./src
