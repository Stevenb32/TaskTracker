import { defineConfig, devices } from "@playwright/test";

const uiBaseUrl = process.env.UI_BASE_URL ?? "http://localhost:5173";
const apiBaseUrl = process.env.API_BASE_URL ?? "http://localhost:5127";

export default defineConfig({
  testDir: "./tests",
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: 1,
  reporter: "html",

  use: {
    baseURL: uiBaseUrl,
    screenshot: "only-on-failure",
    trace: "on-first-retry",
  },

  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },

    {
      name: "firefox",
      use: { ...devices["Desktop Firefox"] },
    },

    {
      name: "webkit",
      use: { ...devices["Desktop Safari"] },
    },
  ],

  webServer: [
    {
      command: "dotnet run --project ../TaskTracker.Api --launch-profile e2e",
      url: apiBaseUrl,
      reuseExistingServer: !process.env.CI,
      timeout: 120 * 1000,
    },
    {
      command: "npm run dev --prefix ../TaskTracker.Ui",
      url: uiBaseUrl,
      reuseExistingServer: !process.env.CI,
      timeout: 120 * 1000,
    },
  ],
});
