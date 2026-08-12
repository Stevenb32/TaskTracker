# Setup

This guide covers local setup for TaskTracker.

## Prerequisites

- Git
- .NET SDK 10
- Node.js and npm, preferably Node 22 or newer
- Docker Desktop or Docker Engine with Docker Compose
- EF Core CLI (`dotnet-ef`) for database migrations
- Playwright browsers for E2E tests

## Clone And Install

```bash
git clone <repo-url>
cd TaskTracker
```

Restore the .NET solution:

```bash
dotnet restore TaskTracker.slnx
```

Install frontend dependencies:

```bash
cd TaskTracker.Ui
npm install
cd ..
```

Install E2E dependencies and browsers:

```bash
cd TaskTracker.E2E.Tests
npm install
npx playwright install
cd ..
```

Install the EF Core CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

If it is already installed and you want to update it:

```bash
dotnet tool update --global dotnet-ef
```

## Development Environment

The development environment uses PostgreSQL on `localhost:5432`, the API at `http://localhost:5127`, Swagger at `http://localhost:5127/swagger`, and the UI at `http://localhost:5173`.

Start the development database:

```bash
docker compose -f docker-compose.dev.yml up -d
```

Apply migrations:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

Run the API:

```bash
dotnet run --project TaskTracker.Api --launch-profile http
```

Run the UI from a second terminal:

```bash
cd TaskTracker.Ui
npm run dev
```

Open `http://localhost:5173`. The Vite dev server proxies `/api` requests to the local API and strips the `/api` prefix.

Stop the development database:

```bash
docker compose -f docker-compose.dev.yml down
```

## E2E Environment

The E2E environment uses a separate PostgreSQL database on `localhost:5433` and API configuration from `TaskTracker.Api/appsettings.E2E.json`.

Start the E2E database:

```bash
docker compose -f docker-compose.e2e.yml up -d
```

Apply migrations:

```powershell
$env:ASPNETCORE_ENVIRONMENT="E2E"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

The Playwright config starts the API and UI automatically before tests run. When the API runs with `ASPNETCORE_ENVIRONMENT=E2E`, it exposes `POST /testing/reset-db` for test isolation.

Stop the E2E database:

```bash
docker compose -f docker-compose.e2e.yml down
```

## Configuration Notes

- Development connection settings live in `TaskTracker.Api/appsettings.Development.json`.
- E2E connection settings live in `TaskTracker.Api/appsettings.E2E.json`.
- Swagger/OpenAPI is enabled for `Development` and `E2E`.
- A local `.env` file is only needed for the production/demo Docker Compose stack.
- Use `.env.example` as the production/demo template and do not commit real secrets.
