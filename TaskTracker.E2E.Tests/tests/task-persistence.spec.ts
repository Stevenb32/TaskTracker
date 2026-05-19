import { test, expect } from '@playwright/test';

test.beforeEach(async ({ request }) => {
  await request.post('http://localhost:5127/testing/reset-db');
});

test('created task persists after reload', async ({ page }) => {
  await page.goto('http://localhost:5173/');

});

test('completed task stays completed after reload', async ({ page }) => {
  await page.goto('http://localhost:5173/');

});

test('deleted task stays deleted after reload', async ({ page }) => {
  await page.goto('http://localhost:5173/');

});