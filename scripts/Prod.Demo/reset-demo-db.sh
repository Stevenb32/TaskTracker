#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/../.."

set -a
source .env
set +a

echo "Resetting TaskTracker demo database..."

docker compose \
  --env-file .env \
  -f docker-compose.prod.yml \
  exec -T tasktracker-db-prod \
  psql \
    -U "$TASKTRACKER_POSTGRES_USER" \
    -d "$TASKTRACKER_POSTGRES_DB" \
    -f /dev/stdin < scripts/Prod.Demo/seed-demo-db.sql

echo "Demo database reset complete."