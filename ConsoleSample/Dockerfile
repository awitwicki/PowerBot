#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ConsoleSample/ConsoleSample.csproj", "ConsoleSample/"]
COPY ["PowerBot.Web/PowerBot.Web.csproj", "PowerBot.Web/"]
COPY ["PowerBot.Core/PowerBot.Core.csproj", "PowerBot.Core/"]
RUN dotnet restore "ConsoleSample/ConsoleSample.csproj"
COPY . .
WORKDIR "/src/ConsoleSample"
RUN dotnet build "ConsoleSample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConsoleSample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConsoleSample.dll"]
