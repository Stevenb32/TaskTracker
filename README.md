# TaskTracker

TaskTracker is a full-stack task management app built as a learning project for backend development, testing, and frontend fundamentals. It is currently a work in progress.

## Tech Stack

- .NET 10 Minimal API
- EF Core InMemory
- xUnit, FluentAssertions, and WebApplicationFactory
- React with Vite
- Tailwind CSS

## Features

- Create tasks with a title and optional notes
- View all tasks
- Complete tasks
- Reopen completed tasks
- Validate core task rules in the domain layer
- Cover domain behavior with unit tests
- Cover API behavior with integration tests

## Project Structure

```text
TaskTracker.Api          Backend API
TaskTracker.Domain       Core domain logic
TaskTracker.Api.Tests    API integration tests
TaskTracker.Domain.Tests Domain unit tests
TaskTracker.Ui           React frontend
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

## Current Focus

The project currently uses in-memory storage while the task workflow, API behavior, tests, and frontend are being built out. Planned improvements include persistent storage, task editing, task deletion, filtering, and deployment setup.
