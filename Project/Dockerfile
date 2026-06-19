# --- Etap budowania ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiuj plik projektu i przywróć zależności (lepsze cache)
COPY FitTrack/FitTrack.csproj FitTrack/
RUN dotnet restore FitTrack/FitTrack.csproj

# Kopiuj resztę kodu i opublikuj aplikację
COPY FitTrack/ FitTrack/
RUN dotnet publish FitTrack/FitTrack.csproj -c Release -o /app/publish

# --- Etap uruchomieniowy ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FitTrack.dll"]
