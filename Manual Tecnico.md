# Manual técnico — Marcador de Baloncesto
**Stack:** Angular (standalone, signals) + ASP.NET Core 8 (Web API + SignalR) + EF Core + SQL Server 2022 + Docker

> Documento técnico para desarrolladores: arquitectura, estructura del repo, configuración, ejecución en **dev** y con **Docker**, contratos HTTP/SignalR, migraciones, logging, seguridad y troubleshooting.  

---

## 1) Arquitectura

### Capas y responsabilidades
- **Frontend (Angular)**
  - Rutas/Componentes standalone para crear/listar/operar juegos.
  - `GameService` (HTTP) → CRUD y comandos (puntos/faltas/timer/cuarto).
  - `RealtimeService` (SignalR) → conexión WS, join/leave por `gameId`, handler `GameUpdated`.
  - Estado local mínimo: siempre se sincroniza con el servidor vía `GameUpdated`.

- **Backend (ASP.NET Core)**
  - **Controllers** (`GamesController`) → endpoints REST `/api/games...`.
  - **SignalR Hub** (`GameHub`) → canal tiempo real `/hubs/game`.
  - **EF Core** (`AppDbContext`) → modelo, migraciones, persistencia.
  - **Repos/Servicios** → lógica de dominio (puntos, faltas, reloj, cuartos).

- **Base de datos**
  - SQL Server 2022 (Linux container o instancia local).
  - Migraciones EF aplicadas en arranque (`db.Database.Migrate()`).

---

## 2) Estructura del repositorio
```text
/Scoreboard-Basketball-main
├─ angularapp1.client/                 # Frontend Angular
│  ├─ src/
│  │  ├─ app/
│  │  │  ├─ pages/                    # setup/list/control del juego
│  │  │  ├─ services/
│  │  │  │  ├─ game.service.ts        # HTTP /api/games...
│  │  │  │  └─ realtime.service.ts    # SignalR /hubs/game
│  │  │  └─ app.routes.ts
│  │  └─ proxy.conf.js                # Proxy dev (api, hubs, logos)
│  └─ package.json
│
├─ AngularApp1.Server/                 # Backend ASP.NET Core
│  ├─ Controllers/
│  │  └─ GamesController.cs
│  ├─ Application/Hubs/
│  │  └─ GameHub.cs
│  ├─ Domain/                         # entidades / interfaces
│  ├─ Infrastructure/
│  │  ├─ AppDbContext.cs
│  │  └─ Repository/...
│  ├─ wwwroot/
│  │  └─ logos/                       # archivos estáticos (logos)
│  ├─ Program.cs
│  ├─ appsettings.json
│  └─ Dockerfile
│
├─ docker-compose.yml
└─ README.md
```

---

## 3) Configuración

### 3.1 Variables de entorno (Backend)
| Variable | Descripción | Ejemplo |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Development` / `Production` | `Production` |
| `ASPNETCORE_URLS` | URLs que escucha la API | `http://+:8080` |
| `ConnectionStrings__Default` | **Sobrescribe** `GetConnectionString("Default")` | `Server=sqlserver,1433;Database=ScoreboardDb;User Id=sa;Password=Strong!Passw0rd;Encrypt=True;TrustServerCertificate=True` |

### 3.2 Proxy de desarrollo (Angular)
`angularapp1.client/src/proxy.conf.js` (dev con API en 8080):
```js
module.exports = [
  { context: ["/api", "/logos"], target: "http://localhost:8080", secure: false, ws: true },
  { context: ["/hubs"],          target: "http://localhost:8080", secure: false, ws: true }
];
```

`angularapp1.client/package.json` (scripts):
```json
{
  "scripts": {
    "start": "ng serve --proxy-config src/proxy.conf.js --host=127.0.0.1",
    "build": "ng build"
  }
}
```

---

## 4) Ejecución en desarrollo

### 4.1 Prerrequisitos
- **.NET SDK 8**
- **Node.js 18/20** + npm
- **SQL Server** (instancia local o contenedor)

### 4.2 Levantar SQL Server (Docker)
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Strong!Passw0rd_2025" \
  -p 1433:1433 -v mssqldata:/var/opt/mssql \
  --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

### 4.3 Backend (.NET)
Configura la cadena:
- **Local (host):** `Server=localhost,1433;...`
- **En Docker Compose:** `Server=sqlserver,1433;...`

Comandos:
```bash
# (opcional) aplicar migraciones manualmente
dotnet ef database update --project AngularApp1.Server

# ejecutar backend
dotnet run --project AngularApp1.Server
# esperado: http://localhost:8080
```

### 4.4 Frontend (Angular)
```bash
cd angularapp1.client
npm install
npm run start
# abrir http://127.0.0.1:4200 (proxy a /api, /hubs, /logos)
```

---

## 5) Contratos de API y tiempo real

### 5.1 Endpoints REST (principales)
```http
GET    /api/games                  # lista (opcional)
POST   /api/games                  # crear juego
GET    /api/games/{id}             # snapshot actual

POST   /api/games/{id}/points      # body: { side: "home"|"away", points: 1|2|3 }
POST   /api/games/{id}/fouls       # body: { side: "home"|"away", delta: 1|-1 }

POST   /api/games/{id}/timer/start
POST   /api/games/{id}/timer/pause
POST   /api/games/{id}/timer/reset

POST   /api/games/{id}/quarter/next
POST   /api/games/{id}/quarter/prev

POST   /api/games/{id}/finish

POST   /api/games/{id}/teams/{side}/logo   # multipart/form-data
GET    /logos/{filename}                   # estático
```

**Ejemplo `points` (JSON):**
```json
{ "side": "home", "points": 3 }
```

### 5.2 DTOs usados en el cliente
```ts
export interface TeamDto {
  name: string;
  score: number;
  foulsTeam: number;
  logoUrl?: string; // puede ser relativo: "/logos/xxx.png"
}

export interface GameDto {
  id: number;
  home: TeamDto;
  away: TeamDto;
  quarter: number;
  remainingSeconds: number;
}
```

### 5.3 SignalR Hub `/hubs/game`
- **Servidor → Cliente**
  - `GameUpdated(GameDto dto)` → estado consolidado tras cada cambio.

- **Cliente → Servidor**
  - `JoinGame(int gameId)`
  - `LeaveGame(int gameId)`

**Convención de grupo:**
- Recomendado: `"game-{id}"` en `JoinGame` y en los broadcasts:
```csharp
await _hub.Clients.Group($"game-{gameId}").SendAsync("GameUpdated", gameDto);
```

---

