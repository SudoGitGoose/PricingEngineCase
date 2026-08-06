import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

/** Merge Tailwind classes, resolving conflicts (mirrors the real frontend's cn()). */
export function cn(...inputs: ClassValue[]): string {
    return twMerge(clsx(inputs));
}

/** Format a decimal money amount for display in the given ISO currency. */
export function formatMoney(amount: number, currency: string): string {
    return new Intl.NumberFormat("en-DK", {
        style: "currency",
        currency,
    }).format(amount);
}
