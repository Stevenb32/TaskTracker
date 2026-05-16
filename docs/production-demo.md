# Production Demo

The public demo runs the same TaskTracker app using Docker Compose.

Live demo: [https://tasktracker.stevenborkowski.dev](https://tasktracker.stevenborkowski.dev)

## Deployment Overview

The public demo is currently hosted on my Raspberry Pi using Docker Compose. The Compose setup is not Raspberry Pi-specific and can run on any host with Docker and Docker Compose installed, assuming the required environment variables, ports, and networking are configured.

Traffic flow:

```text
Browser
  -> Cloudflare
  -> Nginx reverse proxy on the Raspberry Pi
  -> TaskTracker UI container
  -> /api requests proxied to the TaskTracker API container
  -> PostgreSQL container
```

In my public demo environment, the containers bind host ports to `127.0.0.1` only. This keeps the containers from being exposed directly to the public internet. Public traffic reaches the app through the host-level Nginx reverse proxy.

## Services

`docker-compose.prod.yml` defines three main services:

- `tasktracker-db-prod`: PostgreSQL 17
- `tasktracker-api-prod`: the .NET API container
- `tasktracker-ui-prod`: the React build served by Nginx

The Compose file expects a local `.env` file based on `.env.example`.

## Nginx And `/api` Routing

The UI container uses `TaskTracker.Ui/nginx.conf`.

- `/` serves the built React app.
- `/api/` proxies requests to `tasktracker-api-prod:8080`.

This lets the browser call the API through the same site origin using `/api`.

## External Docker Network

My deployment uses an external Docker network named `steven_web` so the host reverse proxy can reach the app containers.

This is deployment-specific. If you run this project on another host, you can rename this network, create your own external reverse proxy network, or remove it depending on your Docker and reverse proxy setup.

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
