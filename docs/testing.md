# Testing

TaskTracker uses layered automated tests: domain unit tests, API integration tests, and Playwright E2E tests.

## Test Layers

| Layer | Project | Purpose |
| --- | --- | --- |
| Domain | `TaskTracker.Domain.Tests` | Verify task business rules without API, UI, or database dependencies. |
| API integration | `TaskTracker.Api.Tests` | Verify HTTP behavior through `WebApplicationFactory`. |
| E2E | `TaskTracker.E2E.Tests` | Verify user workflows through the React UI against a PostgreSQL E2E database. |

## Domain Tests

Domain tests cover:

- Required title, title length, notes length, trimming, and empty-notes normalization.
- Create, complete, reopen, update details, and change-priority behavior.
- Idempotent complete, reopen, update-details, and unchanged-priority operations.
- Priority validation for `Low`, `Medium`, `High`, and `Critical`.
- Rejection of `Unknown`, undefined priority enum values, and default timestamps.

Run domain tests:

```bash
dotnet test TaskTracker.Domain.Tests/TaskTracker.Domain.Tests.csproj
```

## API Integration Tests

Task endpoint integration tests use a custom `WebApplicationFactory` that replaces PostgreSQL with EF Core's in-memory provider. 

The factory creates an isolated database name and exposes reset/add/read helpers so each test can control its state.

API tests cover:

- Root and health endpoints.
- Create, list, get by ID, update title/notes, change priority, complete, reopen, and delete endpoints.
- Priority in create requests, responses, persisted entities, list/detail responses, and workflow tests.
- Default `Medium` priority when a create request omits priority.
- Rejection of `Unknown`, integer, null, missing, and malformed priority payloads where applicable.
- Not-found behavior and validation boundaries for title and notes.

Run API integration tests:

```bash
dotnet test TaskTracker.Api.Tests/TaskTracker.Api.Tests.csproj
```

Run all .NET tests:

```bash
dotnet test TaskTracker.slnx
```

## Playwright E2E Tests

Playwright runs against the React UI and the E2E API, with PostgreSQL provided by `docker-compose.e2e.yml`.

The config starts the API and UI automatically, runs with one worker, and targets Chromium, Firefox, and WebKit.

The E2E suite covers:

- Full task lifecycle: create with priority, edit details, change priority, complete, reopen, and delete.
- Persistence after reload for created, completed, changed-priority, and deleted tasks.
- UI validation for required title, title length, and notes length.

The test helper resets data before each test by calling `POST /testing/reset-db`, which is only mapped when the API runs in the `E2E` environment.

Prepare the E2E database:

```bash
docker compose -f docker-compose.e2e.yml up -d
```

```powershell
$env:ASPNETCORE_ENVIRONMENT="E2E"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

Run E2E tests:

```bash
cd TaskTracker.E2E.Tests
npm run test
```

Open UI mode:

```bash
npm run test-ui
```

Open the HTML report:

```bash
npm run report
```

Stop the E2E database:

```bash
docker compose -f docker-compose.e2e.yml down
```

## CI

GitHub Actions runs on pushes and pull requests to `main`.

The `.NET` job restores the solution, builds in Release, and runs `dotnet test TaskTracker.slnx --configuration Release --no-build`.

The Playwright job waits for the .NET job, restores .NET dependencies, installs `dotnet-ef`, starts and migrates the E2E database, installs Node dependencies with `npm ci`, builds the UI, installs Playwright browsers, and runs `npx playwright test`. On failure, it uploads the Playwright report and test results as artifacts, then stops the E2E database.
