# TaskTracker

TaskTracker is a full-stack task management app built as a learning project for backend development, automated testing, frontend fundamentals, Docker, CI, and deployment.

Live demo: `https://tasktracker.stevenborkowski.dev`

## Tech Stack

- .NET 10 Minimal API
- EF Core with PostgreSQL
- PostgreSQL for dev, e2e, and production/demo data
- xUnit, FluentAssertions, and WebApplicationFactory
- React with Vite
- Tailwind CSS
- Playwright
- Docker and Docker Compose
- Nginx reverse proxy
- GitHub Actions CI

## Features

- Create tasks with a title and optional notes
- View all tasks
- Edit task title and notes
- Complete tasks
- Reopen completed tasks
- Delete tasks
- Validate core task rules in the domain layer
- Persist demo data with PostgreSQL
- Reset and seed the demo database with scripts
- Cover domain behavior with unit tests
- Cover API behavior with integration tests
- Cover the main UI workflow with Playwright E2E tests
- Serve the UI and API behind `/api` routing in production

## Project Structure

```text
TaskTracker.Api          Backend API
TaskTracker.Domain       Core domain logic
TaskTracker.Api.Tests    API integration tests
TaskTracker.Domain.Tests Domain unit tests
TaskTracker.Ui           React frontend
TaskTracker.E2E.Tests    Playwright E2E tests
docs                     Local setup and command notes
scripts                  Demo/database utility scripts
sql                      Local database helper queries
```

## Run Locally

Start the dev database:

```bash
docker compose -f docker-compose.dev.yml up -d
```

Start the API:

```bash
dotnet run --project TaskTracker.Api --launch-profile http
```

Start the frontend:

```bash
cd TaskTracker.Ui
npm install
npm run dev
```

The API runs at `http://localhost:5127`. The frontend runs through Vite and proxies `/api` requests to the backend.

## Run Tests

.NET tests:

```bash
dotnet test TaskTracker.slnx
```

Playwright E2E tests:

```bash
docker compose -f docker-compose.e2e.yml up -d
cd TaskTracker.E2E.Tests
npm install
npx playwright test
```

## Production/Demo Setup

The demo app is hosted on a Raspberry Pi using Docker Compose. Nginx routes public traffic to the frontend and API, with the API available under `/api`.

Production services include:

- TaskTracker API container
- TaskTracker UI container
- PostgreSQL database container

The demo database can be reset and seeded using the scripts in `scripts/prod-demo`.

## Current Focus

The main task workflow, API, frontend, unit tests, integration tests, E2E test, CI, production Docker setup, public routing, and demo database reset process are in place.

Planned improvements include branch protection, required pull requests, additional Playwright E2E tests, Postman/Newman API tests, filtering, and a portfolio project page.
