import { test, expect } from '@playwright/test';

test.beforeEach(async ({ request }) => {
  await request.post('http://localhost:5127/testing/reset-db');
});


test('user can create, update, complete, reopen, and delete a task', async ({ page }) => {
  await page.goto('http://localhost:5173/');

  // Create task
  await page.getByRole('textbox', { name: 'Title' }).fill('Buy milk');
  await page.getByRole('textbox', { name: 'Notes' }).fill('From the store');
  await page.getByRole('button', { name: 'Create' }).click();

  // Verify task appears
  await expect(page.getByRole('heading', { name: 'Buy milk' })).toBeVisible();
  await expect(page.getByText('From the store')).toBeVisible();

  // Update task details
  await page.getByRole('button', { name: 'Edit' }).click();
  await page.getByRole('listitem').getByRole('textbox', { name: 'Title' }).fill('Buy oat milk');
  await page.getByRole('listitem').getByRole('textbox', { name: 'Notes' }).fill('From Publix');
//   await page.getByLabel('Notes').fill('From Publix');
  await page.getByRole('button', { name: 'Save' }).click();

  // Verify updated task appears
  await expect(page.getByText('Buy oat milk')).toBeVisible();
  await expect(page.getByText('From Publix')).toBeVisible();

  // Complete task
  await page.getByRole('button', { name: 'Complete' }).click();
  await expect(page.getByText('Status: Completed')).toBeVisible();

  // Reopen task
  await page.getByRole('button', { name: 'Reopen' }).click();
  await expect(page.getByText('Status: Active')).toBeVisible();

  // Delete task
  await page.getByRole('button', { name: 'Delete' }).click();

  // Verify task is gone
  await expect(page.getByText('Buy oat milk')).not.toBeVisible();  
});