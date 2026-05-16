# Setup

This guide gets TaskTracker running locally for development.

## Prerequisites

- .NET SDK 10
- Node.js and npm, preferably Node 22 or newer
- Docker Desktop or Docker Engine with Docker Compose
- Git
- Playwright browsers, only needed for E2E tests

## Clone The Repo

```bash
git clone <repo-url>
```
```bash
cd TaskTracker
```


## Install Frontend Dependencies

```bash
cd TaskTracker.Ui
npm install
cd ..
```

## Start The Local Database

The local development database is PostgreSQL on port `5432`.

```bash
docker compose -f docker-compose.dev.yml up -d
```

## Apply Database Migrations

If this is your first time using the local database, apply the EF Core migrations.

If the EF Core CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

Then update the local database:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

## Run The API

```bash
dotnet run --project TaskTracker.Api --launch-profile http
```

The API runs at `http://localhost:5127`.

In development, Swagger is available at `http://localhost:5127/swagger`.

## Run The UI

```bash
cd TaskTracker.Ui
npm run dev
```

The UI runs at `http://localhost:5173`.

The Vite dev server proxies `/api` requests to the local API.

## Local Configuration Notes

- Local development uses `TaskTracker.Api/appsettings.Development.json`.
- The dev database connection points to PostgreSQL on `localhost:5432`.
- The E2E database uses `TaskTracker.Api/appsettings.E2E.json` and port `5433`.
- A local `.env` file is only needed for the production/demo Docker Compose setup.
- Use `.env.example` as the template for production/demo values, and do not commit real secrets.

## Playwright Setup

Only install Playwright browsers if you plan to run E2E tests.

```bash
cd TaskTracker.E2E.Tests
npm install
npx playwright install
```
