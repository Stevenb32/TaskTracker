# Production Demo

The public demo runs the same TaskTracker app with Docker Compose on a Raspberry Pi.

Live demo: [https://tasktracker.stevenborkowski.dev](https://tasktracker.stevenborkowski.dev)

## Services

`docker-compose.prod.yml` defines three main services:

- `tasktracker-db-prod`: PostgreSQL 17
- `tasktracker-api-prod`: the .NET API container
- `tasktracker-ui-prod`: the React build served by Nginx

The Compose file expects a local `.env` file based on `.env.example`.

Do not commit real production or demo secrets.

## Raspberry Pi Hosting Note

The demo is intended to run on a Raspberry Pi host with Docker and Docker Compose installed.

The production/demo Compose file publishes service ports to `127.0.0.1`, so public traffic should go through the host reverse proxy instead of exposing the containers directly.

## Nginx And `/api` Routing

The UI container uses `TaskTracker.Ui/nginx.conf`.

- `/` serves the built React app.
- `/api/` proxies requests to `tasktracker-api-prod:8080`.

This lets the browser call the API through the same site origin using `/api`.

The production Compose file also attaches the API and UI containers to the external Docker network `steven_web`, which is intended for the host reverse proxy setup.

## Start Or Update The Demo Stack

Create `.env` from the example file and fill in real values on the host:

```bash
cp .env.example .env
```

Build and start the stack:

```bash
docker compose --env-file .env -f docker-compose.prod.yml up -d --build
```

Check the running services:

```bash
docker compose --env-file .env -f docker-compose.prod.yml ps
```

## Production Database Migrations

If the demo database has not been migrated yet, run EF Core migrations from a shell that can reach the demo database.

Use values from `.env` and do not paste real secrets into documentation:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:ConnectionStrings__TaskTrackerDb="Host=localhost;Port=<host-port>;Database=<database>;Username=<user>;Password=<password>"
dotnet ef database update --project TaskTracker.Api --startup-project TaskTracker.Api
```

## Demo Database Reset

The demo database can be reset and seeded with:

```bash
bash scripts/prod-demo/reset-demo-db.sh
```

The script:

- Loads values from `.env`
- Connects to `tasktracker-db-prod` through Docker Compose
- Deletes existing rows from `"Tasks"`
- Runs `scripts/prod-demo/seed-demo-db.sql`
- Inserts the sample demo tasks

Run this only when the production/demo stack is running.

## Useful Health Checks

Use the host ports from `.env`:

```bash
curl http://localhost:<api-host-port>/health
curl http://localhost:<ui-host-port>/
```
