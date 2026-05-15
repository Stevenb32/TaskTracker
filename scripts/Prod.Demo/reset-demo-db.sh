#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/../.."

set -a
source .env
set +a

echo "[$(date)] Resetting TaskTracker demo database..."

docker compose \
  --env-file .env \
  -f docker-compose.prod.yml \
  exec -T tasktracker-db-prod \
  psql \
    -U "$TASKTRACKER_POSTGRES_USER" \
    -d "$TASKTRACKER_POSTGRES_DB" \
    -f /dev/stdin < scripts/Prod.Demo/seed-demo-db.sql

echo "[$(date)] Verifying seeded task count..."

docker compose \
  --env-file .env \
  -f docker-compose.prod.yml \
  exec -T tasktracker-db-prod \
  psql \
    -U "$TASKTRACKER_POSTGRES_USER" \
    -d "$TASKTRACKER_POSTGRES_DB" \
    -c 'SELECT COUNT(*) AS task_count FROM "Tasks";'

echo "[$(date)] Demo database reset complete."