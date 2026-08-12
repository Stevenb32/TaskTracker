import { test, expect } from "@playwright/test";
import { resetDbViaApi, createTaskViaApi, completeTaskViaApi, deleteTaskViaApi } from "../helpers/tasks-api";

test.beforeEach(async ({ request }) => {
  await resetDbViaApi(request);
});

test("created task persists after reload", async ({ page, request }) => {
  const uniqueId = Date.now();

  const originalTitle = `Buy milk ${uniqueId}`;
  const originalNotes = `From the store ${uniqueId}`;
  const originalPriority = "Low";

  const createdTask = await createTaskViaApi(request, {
    title: originalTitle,
    notes: originalNotes,
    priority: originalPriority,
  });

  await page.goto("/");

  const taskItem = page.getByTestId(`task-item-${createdTask.id}`);

  await expect(taskItem).toBeVisible();
  await expect(taskItem).toContainText(originalTitle);
  await expect(taskItem).toContainText(originalNotes);
  await expect(taskItem).toContainText(`Priority: ${originalPriority}`);

  await page.reload();

  await expect(taskItem).toBeVisible();
  await expect(taskItem).toContainText(originalTitle);
  await expect(taskItem).toContainText(originalNotes);
  await expect(taskItem).toContainText(`Priority: ${originalPriority}`);
});

test("completed task stays completed after reload", async ({ page, request }) => {
  const uniqueId = Date.now();

  const originalTitle = `Buy milk ${uniqueId}`;
  const originalNotes = `From the store ${uniqueId}`;
  const originalPriority = "Low";

  const createdTask = await createTaskViaApi(request, {
    title: originalTitle,
    notes: originalNotes,
    priority: originalPriority,
  });

  await completeTaskViaApi(request, createdTask.id);

  await page.goto("/");

  const taskItem = page.getByTestId(`task-item-${createdTask.id}`);

  await expect(taskItem).toBeVisible();
  await expect(taskItem).toContainText("Status: Completed");

  await page.reload();

  await expect(taskItem).toBeVisible();
  await expect(taskItem).toContainText("Status: Completed");
  await expect(taskItem).toContainText(`Priority: ${originalPriority}`);
});

test("changed priority persists after reload", async ({ page, request }) => {
  const uniqueId = Date.now();

  const originalTitle = `Buy milk ${uniqueId}`;
  const originalNotes = `From the store ${uniqueId}`;
  const originalPriority = "Low";
  const updatedPriority = "Critical";

  const createdTask = await createTaskViaApi(request, {
    title: originalTitle,
    notes: originalNotes,
    priority: originalPriority,
  });

  await page.goto("/");

  const taskItem = page.getByTestId(`task-item-${createdTask.id}`);

  await expect(taskItem).toBeVisible();
  await expect(taskItem).toContainText(`Priority: ${originalPriority}`);

  await taskItem.getByRole("combobox", { name: "Change priority" }).selectOption(updatedPriority);

  await expect(taskItem).toContainText(`Priority: ${updatedPriority}`);

  await page.reload();

  await expect(taskItem).toContainText(`Priority: ${updatedPriority}`);
});

test("deleted task stays deleted after reload", async ({ page, request }) => {
  const uniqueId = Date.now();

  const originalTitle = `Buy milk ${uniqueId}`;
  const originalNotes = `From the store ${uniqueId}`;
  const originalPriority = "Low";

  const createdTask = await createTaskViaApi(request, {
    title: originalTitle,
    notes: originalNotes,
    priority: originalPriority,
  });

  await page.goto("/");

  const taskItem = page.getByTestId(`task-item-${createdTask.id}`);

  await expect(taskItem).toBeVisible();

  await deleteTaskViaApi(request, createdTask.id);

  await page.reload();

  await expect(taskItem).toHaveCount(0);
});
