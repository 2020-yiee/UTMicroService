##See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
#WORKDIR /src
#COPY ["HeatMapAPIServices/HeatMapAPIServices.csproj", "HeatMapAPIServices/"]
#RUN dotnet restore "HeatMapAPIServices/HeatMapAPIServices.csproj"
#COPY . .
#WORKDIR "/src/HeatMapAPIServices"
#RUN dotnet build "HeatMapAPIServices.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "HeatMapAPIServices.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "HeatMapAPIServices.dll"]

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "HeatMapAPIServices.dll"]
