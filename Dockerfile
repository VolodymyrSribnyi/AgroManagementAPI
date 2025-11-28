FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AgroManagementAPI.csproj", "./"]
RUN dotnet restore "AgroManagementAPI.csproj"
COPY . . 
RUN dotnet build "AgroManagementAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgroManagementAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "AgroManagementAPI.dll"]