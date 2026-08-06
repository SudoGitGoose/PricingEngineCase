import { test } from "node:test";
import assert from "node:assert/strict";
import { cn, formatMoney } from "../src/lib/utils";

test("cn merges and de-duplicates conflicting tailwind classes", () => {
    assert.equal(cn("px-2", "px-4"), "px-4");
    assert.equal(cn("text-sm", false && "hidden", "font-bold"), "text-sm font-bold");
});

test("formatMoney renders a currency amount", () => {
    const formatted = formatMoney(25, "DKK");
    // Amount and currency should both be present, independent of locale spacing.
    assert.match(formatted, /25/);
    assert.match(formatted, /kr|DKK/);
});
