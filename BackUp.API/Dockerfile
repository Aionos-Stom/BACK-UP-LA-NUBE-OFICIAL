FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BackUp.sln .
COPY BackUp.API/BackUp.API.csproj BackUp.API/
COPY BackUp.Aplication/BackUp.Aplication.csproj BackUp.Aplication/
COPY BackUp.Persistence/BackUp.Persistence.csproj BackUp.Persistence/
COPY BackUp.IOC1/BackUp.IOC1.csproj BackUp.IOC1/
COPY BackUp.Domainn/BackUp.Domain.csproj BackUp.Domainn/

RUN dotnet restore BackUp.API/BackUp.API.csproj

COPY BackUp.API/ BackUp.API/
COPY BackUp.Aplication/ BackUp.Aplication/
COPY BackUp.Persistence/ BackUp.Persistence/
COPY BackUp.IOC1/ BackUp.IOC1/
COPY BackUp.Domainn/ BackUp.Domainn/

RUN dotnet publish BackUp.API/BackUp.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BackUp.API.dll"]
