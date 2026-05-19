import { test, expect } from '@playwright/test';

test.beforeEach(async ({ request }) => {
  await request.post('http://localhost:5127/testing/reset-db');
});

test('user can create, update, complete, reopen, and delete a task', async ({ page }) => {
  const uniqueId = Date.now();

  const originalTitle = `Buy milk ${uniqueId}`;
  const originalNotes = `From the store ${uniqueId}`;

  const updatedTitle = `Buy oat milk ${uniqueId}`;
  const updatedNotes = `From Publix ${uniqueId}`;

  await page.goto('http://localhost:5173/');

  await test.step('Create task', async () => {
    await page.getByRole('textbox', { name: 'Title' }).fill(originalTitle);
    await page.getByRole('textbox', { name: 'Notes' }).fill(originalNotes);
    await page.getByRole('button', { name: 'Create' }).click();

    const task = page.getByRole('listitem').filter({ has: page.getByRole('heading', { name: originalTitle }) });

    await expect(task).toBeVisible();
    await expect(task).toContainText(originalNotes);
  });  

  await test.step('Edit task', async () => {
    const task = page.getByRole('listitem').filter({ has: page.getByRole('heading', { name: originalTitle }) });

    await task.getByRole('button', { name: 'Edit' }).click();

    await task.getByRole('textbox', { name: 'Title' }).fill(updatedTitle);
    await task.getByRole('textbox', { name: 'Notes' }).fill(updatedNotes);

    await task.getByRole('button', { name: 'Save' }).click();

    const updatedTask = page.getByRole('listitem').filter({ has: page.getByRole('heading', { name: updatedTitle }) });

    await expect(updatedTask).toBeVisible();
    await expect(updatedTask).toContainText(updatedNotes);
  });

  await test.step('Complete task', async () => {
    const task = page.getByRole('listitem').filter({ has: page.getByRole('heading', { name: updatedTitle }) });

    await task.getByRole('button', { name: 'Complete' }).click();

    await expect(task).toContainText('Status: Completed');
  });


  await test.step('Reopen task', async () => {  
    const task = page.getByRole('listitem').filter({ has: page.getByRole('heading', { name: updatedTitle }) });

    await task.getByRole('button', { name: 'Reopen' }).click();

    await expect(task).toContainText('Status: Active');
  });

    await test.step('Delete task', async () => {
    const task = page.getByRole('listitem').filter({ has: page.getByRole('heading', { name: updatedTitle }) });

    await task.getByRole('button', { name: 'Delete' }).click();

    await expect(task).not.toBeVisible(); 
  });

});