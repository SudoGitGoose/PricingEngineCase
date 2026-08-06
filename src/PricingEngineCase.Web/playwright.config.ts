import { defineConfig, devices } from "@playwright/test";

/**
 * E2E config. Assumes the API (port 5080) and the web app (port 3000) are running.
 * `npm run dev` starts the web app; start the API with `dotnet run` in PricingEngineCase.Api.
 */
export default defineConfig({
    testDir: "./tests-e2e",
    fullyParallel: true,
    forbidOnly: !!process.env.CI,
    retries: process.env.CI ? 2 : 0,
    workers: 1,
    reporter: "list",
    use: {
        baseURL: "http://localhost:3000",
        trace: "on-first-retry",
    },
    projects: [
        {
            name: "chromium",
            use: { ...devices["Desktop Chrome"] },
        },
    ],
    webServer: {
        command: "npm run dev",
        url: "http://localhost:3000",
        reuseExistingServer: !process.env.CI,
        timeout: 120_000,
    },
});
