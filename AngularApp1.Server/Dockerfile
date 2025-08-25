# ========= STAGE 1: build Angular =========
FROM node:18-alpine AS ng-build
WORKDIR /ng
# copia package* e instala
COPY angularapp1.client/package*.json ./angularapp1.client/
WORKDIR /ng/angularapp1.client
RUN npm ci
# copia el código y build prod
COPY angularapp1.client/ .
RUN npm run build -- --configuration=production

# ========= STAGE 2: restore & publish .NET =========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ---- agrega Node (ej. Node 20) ----
RUN apt-get update && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get install -y nodejs \
    && node -v && npm -v
# -----------------------------------
COPY AngularApp1.Server/*.csproj ./AngularApp1.Server/

# COPY AngularApp1.server/*/*.csproj AngularApp1.server/*/
# Restaura
RUN dotnet restore AngularApp1.Server/AngularApp1.Server.csproj

# Copia todo y publica
COPY . .
RUN dotnet publish AngularApp1.Server/AngularApp1.Server.csproj -c Release -o /app /p:UseAppHost=false

# ========= STAGE 3: runtime =========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia publicación
COPY --from=build /app .
# Copia el build de Angular dentro de wwwroot
# (El build de Angular queda en angularapp1.client/dist/<nombre>/browser)
COPY --from=ng-build /ng/angularapp1.client/dist/angularapp1.client/browser ./wwwroot

# Asegura el directorio de logos (persistente por volumen)
RUN mkdir -p /app/wwwroot/logos

# Exponer puerto estándar
EXPOSE 8080
# ASPNETCORE_URLS para usar 8080
ENV ASPNETCORE_URLS=http://+:8080

# Variables de entorno que puedes sobreescribir en compose
# ENV ConnectionStrings__Default="Server=sql,1433;Database=Scoreboard;User ID=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"

ENTRYPOINT ["dotnet", "AngularApp1.Server.dll"]
