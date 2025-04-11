FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Используем SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем все файлы проекта и собираем приложение
COPY ["LibraryWebApp_v2/LibraryWebApp_v2.csproj", "LibraryWebApp_v2/"]
COPY ["UnitTests/UnitTests.csproj", "UnitTests/"]
COPY ["BLL/BLL.csproj", "BLL/"]
COPY ["DAL/DAL.csproj", "DAL/"]
RUN dotnet restore "LibraryWebApp_v2/LibraryWebApp_v2.csproj"

# Копируем остальные файлы и собираем приложение
COPY . .
WORKDIR "/src/LibraryWebApp_v2"
RUN dotnet publish -c Release -o /app/publish

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LibraryWebApp_v2.dll"]
