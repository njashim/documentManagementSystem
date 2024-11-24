FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ["DocumentManagementSystem/API/API.csproj", "API/"]
COPY ["DocumentManagementSystem/BusinessLayer/BusinessLayer.csproj", "BusinessLayer/"]
COPY ["DocumentManagementSystem/DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]
COPY ["DocumentManagementSystem/Model/Model.csproj", "Model/"]

RUN dotnet restore

COPY DocumentManagementSystem/API ./API/
COPY DocumentManagementSystem/BusinessLayer ./BusinessLayer/
COPY DocumentManagementSystem/DataAccessLayer ./DataAccessLayer/
COPY DocumentManagementSystem/Model ./Model/

RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build-env /out .

EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8081

ENTRYPOINT ["dotnet", "API.dll"]