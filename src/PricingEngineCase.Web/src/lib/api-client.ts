import { makeApi, Zodios } from "@zodios/core";
import {
    locationSchema,
    pricedSessionSchema,
    priceSessionRequestSchema,
    pricingPlanSchema,
} from "@/domain/pricing";

/**
 * Type-safe API client for the PricingEngineCase backend, built with Zodios
 * (the same client library the real Pricing Engine frontend uses).
 */
const api = makeApi([
    {
        method: "get",
        path: "/v1/locations",
        alias: "getLocations",
        response: locationSchema.array(),
    },
    {
        method: "put",
        path: "/v1/locations/:locationId/pricing-plan",
        alias: "setPricingPlan",
        parameters: [
            { name: "locationId", type: "Path", schema: pricingPlanSchema.shape.currency },
            { name: "body", type: "Body", schema: pricingPlanSchema },
        ],
        response: pricingPlanSchema.nullable(),
        status: 204,
    },
    {
        method: "get",
        path: "/v1/locations/:locationId/pricing-plan",
        alias: "getPricingPlan",
        response: pricingPlanSchema,
    },
    {
        method: "post",
        path: "/v1/sessions",
        alias: "priceSession",
        parameters: [{ name: "body", type: "Body", schema: priceSessionRequestSchema }],
        response: pricedSessionSchema,
    },
    {
        method: "get",
        path: "/v1/sessions/:sessionId",
        alias: "getSession",
        response: pricedSessionSchema,
    },
]);

export const apiBaseUrl =
    process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5080";

export const pricingApi = new Zodios(apiBaseUrl, api);
