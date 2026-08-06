import type { PricedSession } from "@/domain/pricing";
import { formatMoney } from "@/lib/utils";

interface Props {
    priced: PricedSession;
}

/** Displays the calculated cost breakdown for a priced session. */
export function SessionCostBreakdown({ priced }: Props) {
    const rows: Array<{ label: string; value: string }> = [
        { label: "Energy", value: formatMoney(priced.energyCost, priced.currency) },
        { label: "Time", value: formatMoney(priced.timeCost, priced.currency) },
        { label: "Start fee", value: formatMoney(priced.startFee, priced.currency) },
        {
            label: "Subtotal (excl. VAT)",
            value: formatMoney(priced.subtotalExclVat, priced.currency),
        },
        { label: "VAT", value: formatMoney(priced.vat, priced.currency) },
    ];

    return (
        <section
            aria-label="Cost breakdown"
            className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
        >
            <h2 className="mb-4 text-lg font-semibold text-slate-800">Cost breakdown</h2>
            <dl className="space-y-2">
                {rows.map((row) => (
                    <div key={row.label} className="flex justify-between text-sm">
                        <dt className="text-slate-500">{row.label}</dt>
                        <dd className="font-medium text-slate-800" data-testid={`row-${row.label}`}>
                            {row.value}
                        </dd>
                    </div>
                ))}
                <div className="mt-3 flex justify-between border-t border-slate-200 pt-3 text-base">
                    <dt className="font-semibold text-slate-800">Total</dt>
                    <dd className="font-bold text-emerald-700" data-testid="total">
                        {formatMoney(priced.total, priced.currency)}
                    </dd>
                </div>
            </dl>
        </section>
    );
}
