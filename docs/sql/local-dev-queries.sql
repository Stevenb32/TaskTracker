-- view all tasks
SELECT
    "Id",
    "Title",
    "Notes",
    "Status",
    "CreatedAt",
    "CompletedAt",
    "UpdatedAt"
FROM public."Tasks"
LIMIT 1000;

-- select by id
SELECT *
FROM public."Tasks"
WHERE "Id" = 'ec5b1576-2d5e-416b-a82c-eb1b72bffb53';

-- newest tasks first
SELECT *
FROM public."Tasks"
ORDER BY "CreatedAt" DESC;

-- active tasks
SELECT *
FROM public."Tasks"
WHERE "Status" = 0;

-- completed tasks
SELECT *
FROM public."Tasks"
WHERE "Status" = 1;

-- count tasks
SELECT COUNT(*)
FROM public."Tasks";

-- EF migration history
SELECT *
FROM public."__EFMigrationsHistory";

-- create  task
INSERT INTO public."Tasks"
(
    "Id",
    "Title",
    "Notes",
    "Status",
    "CreatedAt",
    "CompletedAt",
    "UpdatedAt"
)
VALUES
(
    gen_random_uuid(),
    'Test task from SQL',
    'Created directly in Postgres',
    0,
    NOW(),
    NULL,
    NOW()
);