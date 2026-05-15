# Testing

TaskTracker uses three test layers: domain unit tests, API integration tests, and Playwright E2E tests.

## Domain Unit Tests

Project: `TaskTracker.Domain.Tests`

These tests cover the core task rules without the API, UI, or database. They test behavior such as creating, completing, reopening, and updating task details.

Run them with:

```bash
dotnet test TaskTracker.Domain.Tests/TaskTracker.Domain.Tests.csproj
```

## API Integration Tests

Project: `TaskTracker.Api.Tests`

These tests cover the HTTP API endpoints using `WebApplicationFactory`. The test factory replaces PostgreSQL with an EF Core in-memory database for fast API tests.

Run them with:

```bash
dotnet test TaskTracker.Api.Tests/TaskTracker.Api.Tests.csproj
```

## All .NET Tests

Run all .NET tests from the solution:

```bash
dotnet test TaskTracker.slnx
```

## Playwright E2E Tests

Project: `TaskTracker.E2E.Tests`

The E2E test covers the main UI workflow: create, edit, complete, reopen, and delete a task.

The Playwright config starts the API and UI before the tests run. The E2E database still needs to be running first.

Start the E2E database:

```bash
docker compose -f docker-compose.e2e.yml up -d
```

Apply the E2E database migrations:

```powershell
$env:ASPNETCORE_ENVIRONMENT="E2E"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

Install dependencies if needed:

```bash
cd TaskTracker.Ui
npm install
cd ../TaskTracker.E2E.Tests
npm install
npx playwright install
```

Run the E2E tests:

```bash
npx playwright test
```

Run only the configured Chromium project:

```bash
npx playwright test --project=chromium
```

## E2E Database Reset

When the API runs with the `E2E` environment, it exposes `POST /testing/reset-db`.

The Playwright test calls that endpoint before each test so the workflow starts with an empty task table.
