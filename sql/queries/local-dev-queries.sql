-- view all tasks
SELECT *
FROM public."Tasks"
LIMIT 1000;

SELECT COUNT(*)
FROM "Tasks"
WHERE "Priority" IS NULL;

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

-- create  task at specific time
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
    '2026-07-19 10:30:22.494856+00',
    NULL,
    NOW()
);

-- create completed task
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
    'Completed directly in Postgres',
    1,
    NOW(),
    NOW(),
    NOW()
);




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

