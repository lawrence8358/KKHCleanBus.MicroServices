# 多階段建置 - Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["KKHCleanBus.MicroServices.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish "KKHCleanBus.MicroServices.csproj" -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# 從 build 階段複製資料庫檔案
COPY --from=build /src/CleanBus.db .

EXPOSE 5000

ENTRYPOINT ["dotnet", "KKHCleanBus.MicroServices.dll"]
