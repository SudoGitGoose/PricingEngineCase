import { z } from "zod";

/**
 * Zod schemas + inferred types mirroring the backend Contracts DTOs
 * (PricingPlan.v1, PriceSessionRequest.v1, PricedSession.v1).
 */

export const pricingPlanSchema = z.object({
    currency: z.string().min(1),
    energyPricePerKwh: z.number().nonnegative(),
    timePricePerMinute: z.number().nonnegative(),
    startFee: z.number().nonnegative(),
    vatRate: z.number().nonnegative(),
});
export type PricingPlan = z.infer<typeof pricingPlanSchema>;

export const locationSchema = z.object({
    locationId: z.string().min(1),
    name: z.string().min(1),
    plan: pricingPlanSchema,
});
export type Location = z.infer<typeof locationSchema>;

export const priceSessionRequestSchema = z.object({
    sessionId: z.string().min(1),
    locationId: z.string().min(1),
    startTime: z.string().min(1),
    endTime: z.string().min(1),
    energyKwh: z.number().nonnegative(),
});
export type PriceSessionRequest = z.infer<typeof priceSessionRequestSchema>;

export const pricedSessionSchema = z.object({
    sessionId: z.string(),
    locationId: z.string(),
    energyKwh: z.number(),
    durationMinutes: z.number(),
    energyCost: z.number(),
    timeCost: z.number(),
    startFee: z.number(),
    subtotalExclVat: z.number(),
    vat: z.number(),
    total: z.number(),
    currency: z.string(),
});
export type PricedSession = z.infer<typeof pricedSessionSchema>;
