FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["PromoterBot.csproj", "."]
RUN dotnet restore "PromoterBot.csproj"

COPY . .
WORKDIR "/src"

RUN dotnet build "PromoterBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PromoterBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "PromoterBot.dll"]