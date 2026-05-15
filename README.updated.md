# TaskTracker

TaskTracker is a full-stack task management app built as a learning project for backend development, automated testing, frontend fundamentals, Docker, CI, and deployment.

Live demo: `https://tasktracker.stevenborkowski.dev`

## Tech Stack

- .NET 10 Minimal API
- EF Core
- PostgreSQL for dev, e2e, and production/demo data
- xUnit, FluentAssertions, and WebApplicationFactory
- React with Vite
- Tailwind CSS
- Docker and Docker Compose
- Nginx reverse proxy
- GitHub Actions CI

## Features

- Create tasks with a title and optional notes
- View all tasks
- Complete tasks
- Reopen completed tasks
- Delete tasks through the API
- Validate core task rules in the domain layer
- Persist demo data with PostgreSQL
- Reset and seed the demo database with scripts
- Cover domain behavior with unit tests
- Cover API behavior with integration tests
- Serve the UI and API behind `/api` routing in production

## Project Structure

```text
TaskTracker.Api          Backend API
TaskTracker.Domain       Core domain logic
TaskTracker.Api.Tests    API integration tests
TaskTracker.Domain.Tests Domain unit tests
TaskTracker.Ui           React frontend
scripts                  Utility scripts for demo/database setup
```

## Run Locally

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

```bash
dotnet test TaskTracker.slnx
```

## Production/Demo Setup

The demo app is hosted on a Raspberry Pi using Docker Compose. Nginx routes public traffic to the frontend and API, with the API available under `/api`.

Production services include:

- TaskTracker API container
- TaskTracker UI container
- PostgreSQL database container

The demo database can be reset and seeded using the script in `scripts/Prod.Demo`.

## Current Focus

The main workflow, API, frontend, tests, production Docker setup, public routing, and demo database reset process are in place.

Planned improvements include branch protection, required pull requests, required CI checks, additonal Playwright E2E tests, Postman/Newman API tests, filtering, and a portfolio project page.
