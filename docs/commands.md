# Commands

Common TaskTracker commands. Run commands from the repo root unless a section says otherwise.

## Local Development

```bash
docker compose -f docker-compose.dev.yml up -d
dotnet run --project TaskTracker.Api --launch-profile http
```

```bash
cd TaskTracker.Ui
npm install
npm run dev
```

## Docker Compose

Start and stop the dev database:

```bash
docker compose -f docker-compose.dev.yml up -d
docker compose -f docker-compose.dev.yml down
```

Start and stop the E2E database:

```bash
docker compose -f docker-compose.e2e.yml up -d
docker compose -f docker-compose.e2e.yml down
```

Start and stop the production/demo stack:

```bash
docker compose --env-file .env -f docker-compose.prod.yml up -d --build
docker compose --env-file .env -f docker-compose.prod.yml down
```

View production/demo logs:

```bash
docker compose --env-file .env -f docker-compose.prod.yml logs -f
```

## .NET

```bash
dotnet restore TaskTracker.slnx
dotnet build TaskTracker.slnx
dotnet test TaskTracker.slnx
```

Run test projects directly:

```bash
dotnet test TaskTracker.Domain.Tests/TaskTracker.Domain.Tests.csproj
dotnet test TaskTracker.Api.Tests/TaskTracker.Api.Tests.csproj
```

Run the API:

```bash
dotnet run --project TaskTracker.Api --launch-profile http
dotnet run --project TaskTracker.Api --launch-profile e2e
```

## EF Core

Install the EF Core CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

Update the dev database:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

Update the E2E database:

```powershell
$env:ASPNETCORE_ENVIRONMENT="E2E"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

Add a new migration:

```bash
dotnet ef migrations add MigrationName --project TaskTracker.Api --startup-project TaskTracker.Api
```

## Frontend

```bash
cd TaskTracker.Ui
npm install
npm run dev
npm run build
npm run preview
```

## Playwright

```bash
cd TaskTracker.E2E.Tests
npm install
npx playwright install
npx playwright test
```

Useful Playwright variants:

```bash
npx playwright test --project=chromium
npx playwright test --ui
npx playwright show-report
```

## Demo Database

Reset and seed the production/demo database:

```bash
bash scripts/prod-demo/reset-demo-db.sh
```

This expects the production/demo stack to be running and `.env` to contain the required values from `.env.example`.
