#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/../.."

set -a
source .env
set +a

timestamp() {
  date "+%Y-%m-%d %H:%M:%S %Z"
}

run_psql_scalar() {
  docker compose \
    --env-file .env \
    -f docker-compose.prod.yml \
    exec -T tasktracker-db-prod \
    psql \
      -q -t -A \
      -U "$TASKTRACKER_POSTGRES_USER" \
      -d "$TASKTRACKER_POSTGRES_DB" \
      -c "$1"
}

echo
echo "========================================"
echo " TaskTracker Demo DB Reset"
echo " Started: $(timestamp)"
echo "========================================"
echo

echo "Checking current task count..."

DELETED_TASK_COUNT=$(run_psql_scalar 'SELECT COUNT(*) FROM "Tasks";')

echo "Tasks currently in database: $DELETED_TASK_COUNT"
echo

echo "Resetting database..."

docker compose \
  --env-file .env \
  -f docker-compose.prod.yml \
  exec -T tasktracker-db-prod \
  psql \
    -q \
    -U "$TASKTRACKER_POSTGRES_USER" \
    -d "$TASKTRACKER_POSTGRES_DB" \
    -f /dev/stdin < scripts/prod-demo/seed-demo-db.sql

echo "Database reset complete."
echo "Deleted task count: $DELETED_TASK_COUNT"
echo

echo "Verifying seeded task count..."

SEEDED_TASK_COUNT=$(run_psql_scalar 'SELECT COUNT(*) FROM "Tasks";')

echo "Seeded task count: $SEEDED_TASK_COUNT"
echo

echo "========================================"
echo " Demo database reset complete"
echo " Finished: $(timestamp)"
echo "========================================"
echo