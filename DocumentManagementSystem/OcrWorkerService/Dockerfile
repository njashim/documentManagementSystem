FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ["DocumentManagementSystem/OcrWorkerService/OcrWorkerService.csproj", "OcrWorkerService/"]
COPY ["DocumentManagementSystem/BusinessLayer/BusinessLayer.csproj", "BusinessLayer/"]
COPY ["DocumentManagementSystem/DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]

RUN dotnet restore OcrWorkerService/OcrWorkerService.csproj

COPY DocumentManagementSystem/OcrWorkerService ./OcrWorkerService/
COPY DocumentManagementSystem/BusinessLayer ./BusinessLayer/
COPY DocumentManagementSystem/DataAccessLayer ./DataAccessLayer/
COPY DocumentManagementSystem/Model ./Model/

RUN dotnet publish OcrWorkerService/OcrWorkerService.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build-env /out ./

EXPOSE 8082

ENV ASPNETCORE_URLS=http://+:8082

ENTRYPOINT ["dotnet", "OcrWorkerService.dll"]
