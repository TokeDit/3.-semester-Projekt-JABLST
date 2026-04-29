# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

3rd semester school project — a security system (Sikkerhedssystem) built by team JABLST. ASP.NET Core 9 backend serving both a REST API and a Vue 3 frontend as static files.

## Commands

### Backend (ASP.NET Core 9)
```bash
# Run API (from repo root)
cd Rest_SikkerApi && dotnet run --project Rest_SikkerApi

# Build
cd Rest_SikkerApi && dotnet build

# Run tests
cd Rest_SikkerApi && dotnet test TestAPI/TestAPI.csproj

# Run single test class
cd Rest_SikkerApi && dotnet test TestAPI/TestAPI.csproj --filter "FullyQualifiedName~UnitTest1"
```

### Frontend (Vue 3 + Vite)
```bash
cd FrontEnd/SikkerhedsFrontEnd && npm install
cd FrontEnd/SikkerhedsFrontEnd && npm run dev    # dev server
cd FrontEnd/SikkerhedsFrontEnd && npm run build  # outputs to dist/
```

### EF Core Migrations (Code-First)
```bash
cd Rest_SikkerApi/Rest_SikkerApi
dotnet ef migrations add <MigrationName>
dotnet ef database update
dotnet ef migrations remove   # undo last
```

### EF Core Scaffolding (DB-First)
See `Rest_SikkerApi/Rest_SikkerApi/ScafoldingCormands.txt` for scaffold commands with SQL Server connection strings.

## Architecture

### Request Flow
Frontend (Vue SPA) → served from `wwwroot/` by the .NET API → Vue Router handles client-side routing via `MapFallbackToFile("index.html")`. API controllers take priority over the fallback.

### Backend Structure (`Rest_SikkerApi/Rest_SikkerApi/`)
- `Program.cs` — wires up EF Core, JWT auth, CORS (`allowAll`), Swagger, controllers, and static file serving
- `Controllers/AuthController.cs` — JWT login/register (stub, not yet implemented)
- `Controllers/SikkerController.cs` — main feature endpoints (stub)
- `repos/SikkerRepo.cs` — repository injected into controllers via DI; wraps `AppDbContext`
- `data/AppDbContext.cs` — EF Core DbContext; add `DbSet<T>` and configure constraints in `OnModelCreating`
- `models/Model.cs` — entity models (stub)
- `interfaces/Interface.cs` — interfaces for repos/services (stub)

### JWT Configuration
Configured in `appsettings.json` under `"Jwt"` key (Key, Issuer, Audience, ExpireDays). Token generation belongs in `AuthController`; validation is already wired in `Program.cs`.

### Database
SQL Server via EF Core. Connection string must be added to `appsettings.json` as `"DefaultConnection"` or set as an environment variable/Azure App Setting. No `DefaultConnection` is currently present in appsettings — must be added before the API can start.

### Deployment (GitHub Actions → Azure App Service)
1. Vue frontend builds to `FrontEnd/SikkerhedsFrontEnd/dist/`
2. .NET API publishes to `Rest_SikkerApi/publish/`
3. `dist/` contents are copied into `publish/wwwroot/`
4. Combined output deploys to Azure App Service `sikkerheds-app-JABLST`

Requires `AZURE_PUBLISH_PROFILE` secret in GitHub repository settings. CI runs on push to `main`. Unit tests run on every PR to `main`.

### Swagger
Enabled unconditionally (not gated to Development). Accessible at `/swagger` in all environments including production.
