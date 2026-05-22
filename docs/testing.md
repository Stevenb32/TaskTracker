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

The Playwright E2E suite covers task lifecycle, validation, and persistence workflows.

The Playwright config starts the API and UI before the tests run. The E2E database still needs to be running first.

### Start the E2E database from the repo root:

```bash
docker compose -f docker-compose.e2e.yml up -d
```

### Apply the E2E database migrations if needed:

```powershell
$env:ASPNETCORE_ENVIRONMENT="E2E"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

### Run the E2E tests:

```bash
cd TaskTracker.E2E.Tests
npx playwright test
```

Or use the npm script:
```bash
npm run test
```

### Run the Playwright UI mode:

```bash
npx playwright test --ui
```

Or use the npm script:
```bash
npm run test-ui
```

### Open the Playwright HTML report after a test run:

```bash
npx playwright show-report
```

Or use the npm script:
```bash
npm run report
```

### Stop the E2E database:

```bash
docker compose -f docker-compose.e2e.yml down
```

## E2E Database Reset

When the API runs with the `E2E` environment, it exposes `POST /testing/reset-db`.

The Playwright test calls that endpoint before each test so the workflow starts with an empty task table.

## GitHub Actions CI

TaskTracker uses GitHub Actions to run automated checks on pushes and pull requests.

Current CI jobs:

- Build and test .NET
  - Restores the .NET solution
  - Builds the backend projects
  - Runs domain unit tests
  - Runs API integration tests

- Playwright E2E
  - Installs frontend and E2E test dependencies
  - Starts the E2E database
  - Applies database migrations
  - Runs the Playwright browser test suite

The goal is to practice a real pull request workflow where code changes are reviewed and automated checks must pass before merging to `main`.
