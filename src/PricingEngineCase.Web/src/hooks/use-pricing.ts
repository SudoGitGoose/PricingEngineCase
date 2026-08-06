import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { pricingApi } from "@/lib/api-client";
import type { PriceSessionRequest, PricingPlan } from "@/domain/pricing";

/** Fetch the selectable locations and their pricing plans. */
export function useLocations() {
    return useQuery({
        queryKey: ["locations"],
        queryFn: () => pricingApi.getLocations(),
    });
}

/** Fetch the pricing plan for a location. */
export function usePricingPlan(locationId: string | undefined) {
    return useQuery({
        queryKey: ["pricing-plan", locationId],
        enabled: Boolean(locationId),
        queryFn: () =>
            pricingApi.getPricingPlan({ params: { locationId: locationId! } }),
    });
}

/** Persist the pricing plan for a location. */
export function useSetPricingPlan(locationId: string) {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (plan: PricingPlan) =>
            pricingApi.setPricingPlan(plan, { params: { locationId } }),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: ["pricing-plan", locationId] }),
    });
}

/** Price a charging session. */
export function usePriceSession() {
    return useMutation({
        mutationFn: (request: PriceSessionRequest) => pricingApi.priceSession(request),
    });
}
