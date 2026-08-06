import Head from "next/head";
import { useState } from "react";
import { SessionCostBreakdown } from "@/components/charging-sessions/session-cost-breakdown";
import { SessionPricingForm } from "@/components/charging-sessions/session-pricing-form";
import type { PricedSession } from "@/domain/pricing";

export default function HomePage() {
    const [priced, setPriced] = useState<PricedSession | null>(null);

    return (
        <>
            <Head>
                <title>PricingEngineCase</title>
            </Head>
            <main className="mx-auto max-w-3xl px-4 py-12">
                <header className="mb-8">
                    <h1 className="text-2xl font-bold text-slate-900">
                        Charging session pricing
                    </h1>
                    <p className="mt-1 text-sm text-slate-500">
                        Submit a charging session and see how the Pricing Engine breaks down the cost.
                    </p>
                </header>

                <div className="grid gap-6 md:grid-cols-2">
                    <SessionPricingForm onPriced={setPriced} />
                    {priced ? (
                        <SessionCostBreakdown priced={priced} />
                    ) : (
                        <div className="flex items-center justify-center rounded-xl border border-dashed border-slate-300 p-6 text-sm text-slate-400">
                            Price a session to see the breakdown.
                        </div>
                    )}
                </div>
            </main>
        </>
    );
}
