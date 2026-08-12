import { test, expect } from "@playwright/test";
import { resetDbViaApi, createTaskViaApi } from "../helpers/tasks-api";

test.beforeEach(async ({ request }) => {
  await resetDbViaApi(request);
});

test.describe("Task create validation", () => {
  test("shows validation when title is required", async ({ page }) => {
    await page.goto("/");

    const form = page.locator("form");

    await form.getByRole("button", { name: "Create Task" }).click();

    await expect(form.getByText("Title is required")).toBeVisible();
  });

  test("shows title limit message and prevents title from exceeding 100 characters", async ({ page }) => {
    const titleWith100Chars = "a".repeat(100);

    await page.goto("/");

    const form = page.locator("form");
    const titleInput = form.getByRole("textbox", { name: "Title" });

    await titleInput.fill(titleWith100Chars);

    await expect(titleInput).toHaveValue(titleWith100Chars);
    await expect(form.getByText("Title can only be 100 characters")).toBeVisible();

    await titleInput.press("b");

    await expect(titleInput).toHaveValue(titleWith100Chars);
  });

  test("shows notes limit message when notes reaches 500 characters", async ({ page }) => {
    const notesWith500Chars = "a".repeat(500);

    await page.goto("/");

    const form = page.locator("form");
    const notesInput = form.getByRole("textbox", { name: "Notes" });

    await notesInput.fill(notesWith500Chars);

    await expect(notesInput).toHaveValue(notesWith500Chars);
    await expect(form.getByText("Notes can only be 500 characters")).toBeVisible();

    await notesInput.press("b");

    await expect(notesInput).toHaveValue(notesWith500Chars);
  });
});

test.describe("Task edit validation", () => {
  test("shows validation when title is required while editing", async ({ page, request }) => {
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

    await taskItem.getByRole("button", { name: "Edit" }).click();

    await taskItem.getByRole("textbox", { name: "Title" }).fill("");

    await taskItem.getByRole("button", { name: "Save" }).click();

    await expect(taskItem.getByText("Title is required")).toBeVisible();
  });

  test("shows title limit message and prevents title from exceeding 100 characters while editing", async ({ page, request }) => {
    const uniqueId = Date.now();
    const titleWith100Chars = "a".repeat(100);

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

    await taskItem.getByRole("button", { name: "Edit" }).click();

    const titleInput = taskItem.getByRole("textbox", { name: "Title" });

    await titleInput.fill(titleWith100Chars);

    await expect(titleInput).toHaveValue(titleWith100Chars);
    await expect(taskItem.getByText("Title can only be 100 characters")).toBeVisible();

    await titleInput.press("b");

    await expect(titleInput).toHaveValue(titleWith100Chars);
  });

  test("shows message when notes reaches max length while editing", async ({ page, request }) => {
    const uniqueId = Date.now();
    const notesWith500Chars = "a".repeat(500);

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

    await taskItem.getByRole("button", { name: "Edit" }).click();

    const notesInput = taskItem.getByRole("textbox", { name: "Notes" });

    await notesInput.fill(notesWith500Chars);

    await expect(notesInput).toHaveValue(notesWith500Chars);
    await expect(taskItem.getByText("Notes can only be 500 characters")).toBeVisible();

    await notesInput.press("b");

    await expect(notesInput).toHaveValue(notesWith500Chars);
  });
});
