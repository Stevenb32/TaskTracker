# Setup

This guide walks through cloning TaskTracker, installing the required tools and project dependencies, then running the local development and E2E environments.

## Prerequisites

Install these before setting up the repo:

- Git
- .NET SDK 10
- Node.js and npm, preferably Node 22 or newer
- Docker Desktop or Docker Engine with Docker Compose
- Playwright browsers, only needed for E2E tests

Optional but recommended:

- A terminal that can run PowerShell commands
- An editor such as Visual Studio Code or Visual Studio

## Clone The Repo

```bash
git clone <repo-url>
cd TaskTracker
```

## Install Project Dependencies

Run these commands from the repo root unless a step says otherwise.

Restore the .NET solution:

```bash
dotnet restore TaskTracker.slnx
```

Install the frontend dependencies:

```bash
cd TaskTracker.Ui
npm install
cd ..
```

Install the E2E test dependencies:

```bash
cd TaskTracker.E2E.Tests
npm install
npx playwright install
cd ..
```

Install the EF Core CLI if it is not already installed:

```bash
dotnet tool install --global dotnet-ef
```

If `dotnet-ef` is already installed and you want to update it:

```bash
dotnet tool update --global dotnet-ef
```

## Development Environment

The development environment uses:

- PostgreSQL on `localhost:5432`
- API at `http://localhost:5127`
- Swagger at `http://localhost:5127/swagger`
- UI at `http://localhost:5173`
- API config from `TaskTracker.Api/appsettings.Development.json`

### Start The Development Database

Open a terminal at the repo root and run:

```bash
docker compose -f docker-compose.dev.yml up -d
```

### Apply Development Database Migrations

PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

### Run The Development API

```bash
dotnet run --project TaskTracker.Api --launch-profile http
```

### Run The Development UI

Open a second terminal at the repo root and run:

```bash
cd TaskTracker.Ui
npm run dev
```

Then open `http://localhost:5173`.

The Vite dev server proxies `/api` requests to the local API.

### Stop The Development Database

```bash
docker compose -f docker-compose.dev.yml down
```

## E2E Environment

The E2E environment uses:

- PostgreSQL on `localhost:5433`
- API at `http://localhost:5127`
- UI at `http://localhost:5173`
- API config from `TaskTracker.Api/appsettings.E2E.json`

The Playwright config starts the API and UI automatically before the tests run. You only need to start the E2E database and apply migrations first.

### Start The E2E Database

Open a terminal at the repo root and run:

```bash
docker compose -f docker-compose.e2e.yml up -d
```

### Apply E2E Database Migrations

PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT="E2E"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

### Stop The E2E Database

```bash
docker compose -f docker-compose.e2e.yml down
```

## Local Configuration Notes

- Development database settings live in `TaskTracker.Api/appsettings.Development.json`.
- E2E database settings live in `TaskTracker.Api/appsettings.E2E.json`.
- The E2E API exposes `POST /testing/reset-db` so Playwright can reset data between tests.
- A local `.env` file is only needed for the production/demo Docker Compose setup.
- Use `.env.example` as the template for production/demo values, and do not commit real secrets.
- For test commands and CI details, see [Testing](testing.md).
