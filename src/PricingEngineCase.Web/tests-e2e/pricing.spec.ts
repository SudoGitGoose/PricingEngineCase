import { test, expect } from "@playwright/test";

/**
 * Happy-path E2E: price a session and assert the breakdown renders.
 * Requires the backend API to be running on http://localhost:5080.
 */
test("prices a charging session and shows the total", async ({ page }) => {
    await page.goto("/");

    await expect(page.getByRole("heading", { name: "Charging session pricing" })).toBeVisible();

    await page.getByRole("button", { name: "Price session" }).click();

    await expect(page.getByTestId("total")).toBeVisible();
    await expect(page.getByRole("heading", { name: "Cost breakdown" })).toBeVisible();
});
