DELETE FROM "Tasks";

INSERT INTO "Tasks"
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
    'Try the public demo',
    'Create, complete, reopen, and delete tasks.',
    0,
    NOW() - INTERVAL '3 days',
    NULL,
    NOW() - INTERVAL '3 days'
),
(
    gen_random_uuid(),
    'Completed example task',
    'This task shows what a completed task looks like.',
    1,
    NOW() - INTERVAL '2 days',
    NOW() - INTERVAL '1 day',
    NOW() - INTERVAL '1 day'
),
(
    gen_random_uuid(),
    'Task with no notes',
    NULL,
    0,
    NOW() - INTERVAL '1 day',
    NULL,
    NOW() - INTERVAL '1 day'
),
(
    gen_random_uuid(),
    'Edit me',
    'This is a good test task for future edit functionality.',
    0,
    NOW(),
    NULL,
    NOW()
);