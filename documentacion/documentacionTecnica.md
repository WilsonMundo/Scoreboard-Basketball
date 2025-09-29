#  Proyecto Scoreboard Basketball


**Scoreboard Basketball** aplicación web para la gestión de equipos, jugadores y partidos de baloncesto.  
El sistema permite:  
- Administrar equipos con datos básicos y logos.  
- Administrar jugadores asociados a equipos.  
- Gestionar partidos, asignar roster de jugadores y registrar resultados.  

La arquitectura se basa en un **backend en .NET 8 (C#)** y un **frontend en Angular 18 con TailwindCSS**, desplegado con **Docker** y usando **SQL Server** como base de datos.  



## Arquitectura General

### Backend
- **Framework:** ASP.NET Core 8 Web API  
- **Patrón:** Arquitectura en capas (Domain, Application, Infrastructure, Controllers)  
- **ORM:** Entity Framework Core 8  
- **Base de Datos:** SQL Server 2022  
- **Migraciones:** `dotnet ef migrations`  
- **Seguridad:** Autenticación JWT con roles (Admin / User)  
- **DTOs:** Separados por entidad (Teams, Players, Matches)  



### Frontend
- **Framework:** Angular 18 (standalone components)  
- **Estilos:** TailwindCSS  
- **UI Components:** Botones, tablas, formularios con validaciones    
- **Servicios:** Cada entidad (`team.service.ts`, `player.service.ts`, `match.service.ts` etc) consume los endpoints del backend  

### Infraestructura
- **Orquestación:** Docker Compose  
- **Contenedores:**  
  - `api` (ASP.NET Core)  
  - `sqlserver` (SQL Server 2022)  
  - `client` (Angular SSR con Nginx)  




###  Módulo de Equipos
**Requerimientos:**  
- RF-ADM-04: Registrar equipos con nombre, ciudad y logo.  
- RF-ADM-05: Editar o eliminar equipos existentes.  
- RF-ADM-06: Listar equipos con filtros por ciudad y búsqueda por nombre.  

**Clases y DTOs principales:**  
- `Team` (Entidad en Domain)  
- `TeamCreateDto`, `TeamUpdateDto`, `TeamDto`, `TeamListRequest`  
- `TeamService` (frontend Angular)  

**Pantallas:**  
- `team-list.component` → tabla con paginación, búsqueda, filtro de ciudades  
- `team-form.component` → formulario para crear/editar equipo + carga de logo  

---

### Módulo de Jugadores
**Requerimientos:**  
- RF-ADM-07: Registrar jugadores asociados a un equipo.  
- RF-ADM-08: Editar o eliminar jugadores.  
- RF-ADM-09: Listar jugadores con filtros por equipo y búsqueda por nombre/nacionalidad.  

**Clases y DTOs principales:**  
- `Player` (Entidad en Domain)  
- `PlayerCreateDto`, `PlayerUpdateDto`, `PlayerDto`, `PlayerListRequest`  
- `PlayerService` (frontend Angular)  

**Pantallas:**  
- `player-list.component` → tabla con paginación, filtro por equipo  
- `player-form.component` → formulario para crear/editar jugador  

---

### Módulo de Partidos
**Requerimientos:**  
- RF-ADM-10: Crear partidos seleccionando equipos y fecha/hora.  
- RF-ADM-11: Asignar roster de jugadores por equipo.  
- RF-ADM-12: Ver historial de partidos con resultados finales.  

**Clases y DTOs principales:**  
- `Match` (Entidad en Domain)  
- `MatchCreateDto`, `MatchUpdateDto`, `MatchDto`, `MatchListRequest`  
- `MatchService` (frontend Angular)  

**Pantallas:**  
- `match-list.component` → historial de partidos con marcadores finales  
- `match-form.component` → selección de equipos, fecha, roster y edición de resultados  

## Flujo de Datos
1. El **usuario Admin** interactúa con el frontend Angular.  
2. Angular envía requests al **API REST en .NET** (`/api/teams`, `/api/players`, `/api/matches`).  
3. El backend valida la petición y utiliza **EF Core** para persistir datos en SQL Server.  
4. El backend responde con un objeto `ResultAPI<T>` con `isSuccess`, `message`, `code` y `result`.  
5. Angular muestra notificaciones (`app-notification`) en base a la respuesta.  

## Seguridad
- Autenticación basada en **JWT**.  
- Middleware que valida tokens en cada request.  
- **Guards en Angular:**  
  - `AuthGuard`: protege rutas generales.  
  - `AdminGuard`: protege rutas `/admin`.  



##  Base de Datos
Tablas principales:  
- `Teams` 
- `Game` 
- `Players` 
- `Matches` 
- `MatchPlayers`   
etc. 







## Tecnologías

- **Backend:** .NET 8, EF Core 8, SQL Server 2022  
- **Frontend:** Angular 18, TailwindCSS  
- **DevOps:** Docker, Docker Compose  
- **Control de versiones:** GitHub 

