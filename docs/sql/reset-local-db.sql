-- DEV ONLY: delete all task rows
DELETE FROM public."Tasks";

-- DEV ONLY: delete all task rows and return deleted rows
DELETE FROM public."Tasks"
RETURNING *;