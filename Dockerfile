# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore first for better layer caching
# If your csproj is NOT in repo root, adjust the COPY path accordingly.
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Render web services commonly expect port 10000 by default (configurable),
# and your service must bind to 0.0.0.0 to accept public traffic. [1](https://render.com/docs/web-services)
EXPOSE 10000

# Copy published output
COPY --from=build /app/publish ./

# Copy entrypoint script (sets ASPNETCORE_URLS from $PORT)
COPY start.sh ./
RUN chmod +x ./start.sh

ENTRYPOINT ["./start.sh"]