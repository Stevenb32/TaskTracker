import { test, expect } from "@playwright/test";
import { resetDbViaApi } from "../helpers/tasks-api";

test.beforeEach(async ({ request }) => {
  await resetDbViaApi(request);
});

test("user can create, update, change priority, complete, reopen, and delete a task", async ({ page }) => {
  const uniqueId = Date.now();

  const originalTitle = `Buy milk ${uniqueId}`;
  const originalNotes = `From the store ${uniqueId}`;
  const originalPriority = "Low";

  const updatedTitle = `Buy oat milk ${uniqueId}`;
  const updatedNotes = `From Publix ${uniqueId}`;
  const updatedPriority = "Critical";

  let taskId: string;

  await page.goto("/");

  await test.step("Create task", async () => {
    const form = page.locator('form');

    await form.getByRole("textbox", { name: "Title" }).fill(originalTitle);
    await form.getByRole("textbox", { name: "Notes" }).fill(originalNotes);
    await form.getByRole("combobox", { name: "Priority" }).selectOption(originalPriority);

    const createTaskResponsePromise = page.waitForResponse(
      (response) => response.url().includes("/tasks") && response.request().method() === "POST" && response.status() === 201,
    );

    await page.getByRole("button", { name: "Create Task" }).click();

    const createTaskResponse = await createTaskResponsePromise;
    const createdTask = await createTaskResponse.json();

    taskId = createdTask.id;

    const taskItem = page.getByTestId(`task-item-${taskId}`);

    await expect(taskItem).toBeVisible();
    await expect(taskItem).toContainText(originalTitle);
    await expect(taskItem).toContainText(originalNotes);
    await expect(taskItem).toContainText(`Priority: ${originalPriority}`);
  });

  await test.step("Edit task", async () => {
    const taskItem = page.getByTestId(`task-item-${taskId}`);

    await taskItem.getByRole("button", { name: "Edit" }).click();

    await taskItem.getByRole("textbox", { name: "Title" }).fill(updatedTitle);
    await taskItem.getByRole("textbox", { name: "Notes" }).fill(updatedNotes);

    await taskItem.getByRole("button", { name: "Save" }).click();

    await expect(taskItem).toBeVisible();
    await expect(taskItem).toContainText(updatedTitle);
    await expect(taskItem).toContainText(updatedNotes);
    await expect(taskItem).toContainText(`Priority: ${originalPriority}`);
  });

  await test.step("Change priority", async () => {
    const taskItem = page.getByTestId(`task-item-${taskId}`);

    await taskItem.getByRole("combobox", { name: "Change priority" }).selectOption(updatedPriority);

    await expect(taskItem).toContainText(`Priority: ${updatedPriority}`);
  });

  await test.step("Complete task", async () => {
    const taskItem = page.getByTestId(`task-item-${taskId}`);

    await taskItem.getByRole("button", { name: "Complete" }).click();

    await expect(taskItem).toContainText("Status: Completed");
    await expect(taskItem).toContainText(`Priority: ${updatedPriority}`);
  });

  await test.step("Reopen task", async () => {
    const taskItem = page.getByTestId(`task-item-${taskId}`);

    await taskItem.getByRole("button", { name: "Reopen" }).click();

    await expect(taskItem).toContainText("Status: Active");
    await expect(taskItem).toContainText(`Priority: ${updatedPriority}`);
  });

  await test.step("Delete task", async () => {
    const taskItem = page.getByTestId(`task-item-${taskId}`);

    await taskItem.getByRole("button", { name: "Delete" }).click();

    await expect(taskItem).toHaveCount(0);
  });
});
