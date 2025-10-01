# Scoreboard-Basketball
Proyecto de universidad Scoreboard de Basketball
# url del proyecto

https://scoreboard.mundoalonzo.com/

```
/AngularApp1
├─ AngularApp1.server/                  # API ASP.NET Core 8
│  ├─ Controllers/
│  ├─ Domain/
│  │  ├─ Entities/ (Game, Team, QuarterScore)
│  ├─ Application/
│  │  ├─ DTO/ (GameDto, TeamDto, QuarterPointsDto, CreateGameDto, UpdateScoreDto, ResetTimerDto)
│  │  ├─ Interfaces/ (IGameService, repos, IImageService)
│  │  └─ Services/ (GameService, ImageService, etc.)
│  ├─ Infrastructure/
│  │  ├─ Persistence/ (AppDbContext, repos EF)
│  │  └─ Services/ (implementaciones)
│  ├─ Profiles/ (AutoMapper)
│  ├─ wwwroot/logos/ (archivos estáticos de logos
│  └─ Program.cs (UseStaticFiles, Swagger, CORS/SignalR opcional)
└─ angularapp1.client/                  # Angular 18
   ├─ src/app/
   │  ├─ components/
   │  │  ├─ game/ (GameComponent: vista pública/control marcador+timer+box)
   │  │  └─ layout/ (LayoutComponent)
   │  ├─ pages/
   │  │  └─ game-setup/ (GameSetupComponent: crear juego + subir logos)
   │  ├─ services/ (game.service.ts, realtime.service.ts opcional)
   │  ├─ app.routes.ts / app.config.ts
   │  └─ styles.css (Tailwind)
   ├─ proxy.conf.js (proxy a /api y /logos)
   |
   └─ tailwind.config.js
```


✅ Funcionalidades

-  Crear juego (POST /api/games): equipos local/visitante, duración del cuarto.

-  Subir logo por equipo (compresión básica en servidor).

-  Marcador en vivo con atajos de puntos (±1/2/3).

-  Timer por cuarto (start / pause / reset) — materializado en servidor para evitar “saltos”.

-  Cuartos: Next/Prev, reseteando timer por periodo.

-  Overtime solo si hay empate (Q4 → OT1; OTn solo si persiste el empate).

-  Finalizar partido (por botón o al pausar con tiempo 0 y sin empate).

-  Box score por periodo (QuarterScore en BD).

🛠️ Requisitos

-  .NET 8 SDK

-  Node.js 18+ y pnpm/npm

-  SQL Server (local o contenedor

⚙️ Configuración

  appsettings.json (cadena de conexión):

```json
{
  "ConnectionStrings": {
    "Default": "Server={server};Database=Scoreboard;User ID={user};Password={YourStrong!Passw0rd};TrustServerCertificate=True"
  }
}
```

## 1. Base de Datos

La aplicación usa **SQL Server** (puede ajustarse a otro provider si es necesario).  
La base de datos debe llamarse:

```text
Scoreboard
```
## 2.Variables de Entorno
En las varaibles de entorno  se debe crear las siguientes variables
- **Jwt:Key**
  *Clave secreta utilizada para firmar y validar los tokens JWT.*

  Debe ser una cadena segura, larga y difícil de adivinar.

Si esta clave cambia, todos los tokens emitidos anteriormente dejan de ser válidos.

Ejemplo de valor:

```ruby
Jwt:Key=MI_CLAVE_SUPER_SECRETA_123456
```
- **Jwt:Issuer**
   *Identificador del emisor del token.*
  
  Normalmente es la URL o nombre de la aplicación que genera el token.

Se utiliza para validar que el token fue emitido por una fuente confiable.

Ejemplo de valor:

```ruby
Jwt:Issuer=https://mi-api.com
```
- **Jwt:Audience**
  *Identificador de la audiencia del token.*

  Indica quién debe aceptar y procesar el token.

Protege contra el uso del token en aplicaciones no autorizadas.

Ejemplo de valor:

```ruby
Jwt:Audience=https://mi-frontend.com
```
``` csharp
var jwtKey = builder.Configuration["Jwt:Key"] ?? "";
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
```

