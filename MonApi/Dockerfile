
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY MonApi/MonApi.csproj MonApi/
COPY MonApi.Tests/MonApi.Tests.csproj MonApi.Tests/
COPY MonApi/MonApi.slnx MonApi/

RUN dotnet restore MonApi/MonApi.slnx

COPY MonApi/ MonApi/
COPY MonApi.Tests/ MonApi.Tests/

RUN dotnet publish MonApi/MonApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "MonApi.dll"]
