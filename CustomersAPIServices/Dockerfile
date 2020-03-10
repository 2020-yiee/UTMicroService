FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 9001

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["CustomersAPIServices.csproj", "./"]
RUN dotnet restore "./CustomersAPIServices.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CustomersAPIServices.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CustomersAPIServices.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomersAPIServices.dll"]
