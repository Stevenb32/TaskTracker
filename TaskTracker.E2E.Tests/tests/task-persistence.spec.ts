import { test, expect } from "@playwright/test";
import { resetDbViaApi, createTaskViaApi, completeTaskViaApi, deleteTaskViaApi } from "../helpers/tasks-api";

test.beforeEach(async ({ request }) => {
  await resetDbViaApi(request);
});

test("created task persists after reload", async ({ page, request }) => {
  const uniqueId = Date.now();

  const originalTitle = `Buy milk ${uniqueId}`;
  const originalNotes = `From the store ${uniqueId}`;

  const createdTask = await createTaskViaApi(request, {
    title: originalTitle,
    notes: originalNotes,
  });

  await page.goto("/");

  const taskItem = page.getByTestId(`task-item-${createdTask.id}`);

  await expect(taskItem).toBeVisible();
  await expect(taskItem).toContainText(originalTitle);
  await expect(taskItem).toContainText(originalNotes);

  await page.reload();

  await expect(taskItem).toBeVisible();
  await expect(taskItem).toContainText(originalTitle);
  await expect(taskItem).toContainText(originalNotes);
});

test("completed task stays completed after reload", async ({ page, request }) => {
  const uniqueId = Date.now();

  const originalTitle = `Buy milk ${uniqueId}`;
  const originalNotes = `From the store ${uniqueId}`;

  const createdTask = await createTaskViaApi(request, {
    title: originalTitle,
    notes: originalNotes,
  });

  await completeTaskViaApi(request, createdTask.id);

  await page.goto("/");

  const taskItem = page.getByTestId(`task-item-${createdTask.id}`);

  await expect(taskItem).toBeVisible();

  await expect(taskItem).toContainText("Status: Completed");

  await page.reload();

  await expect(taskItem).toBeVisible();
  await expect(taskItem).toContainText("Status: Completed");
});

test("deleted task stays deleted after reload", async ({ page, request }) => {
  const uniqueId = Date.now();

  const originalTitle = `Buy milk ${uniqueId}`;
  const originalNotes = `From the store ${uniqueId}`;

  const createdTask = await createTaskViaApi(request, {
    title: originalTitle,
    notes: originalNotes,
  });

  await page.goto("/");

  const taskItem = page.getByTestId(`task-item-${createdTask.id}`);

  await expect(taskItem).toBeVisible();

  await deleteTaskViaApi(request, createdTask.id);

  await page.reload();

  await expect(taskItem).toBeHidden();
});
