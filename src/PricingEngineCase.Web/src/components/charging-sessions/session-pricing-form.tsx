import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { useLocations, usePriceSession } from "@/hooks/use-pricing";
import { formatMoney } from "@/lib/utils";
import type { PricedSession, PricingPlan } from "@/domain/pricing";

const formSchema = z.object({
    sessionId: z.string().min(1, "Session id is required"),
    locationId: z.string().min(1, "Location id is required"),
    startTime: z.string().min(1, "Start time is required"),
    endTime: z.string().min(1, "End time is required"),
    energyKwh: z.coerce.number().nonnegative("Energy cannot be negative"),
});

type FormValues = z.input<typeof formSchema>;

interface Props {
    onPriced: (priced: PricedSession) => void;
}

/** Form for submitting a charging session to be priced by the backend. */
export function SessionPricingForm({ onPriced }: Props) {
    const priceSession = usePriceSession();
    const { data: locations } = useLocations();
    const {
        register,
        handleSubmit,
        watch,
        setValue,
        formState: { errors },
    } = useForm<FormValues>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            sessionId: `session-${Math.random().toString(36).slice(2, 8)}`,
            locationId: "loc-copenhagen-01",
            startTime: "2026-01-01T10:00",
            endTime: "2026-01-01T11:00",
            energyKwh: 10,
        },
    });

    const selectedLocationId = watch("locationId");
    const selectedLocation = locations?.find(
        (location) => location.locationId === selectedLocationId,
    );

    // Once locations load, make sure the form points at a location that actually exists.
    useEffect(() => {
        if (!locations) {
            return;
        }
        const firstLocation = locations[0];
        if (
            firstLocation &&
            !locations.some((location) => location.locationId === selectedLocationId)
        ) {
            setValue("locationId", firstLocation.locationId);
        }
    }, [locations, selectedLocationId, setValue]);

    const onSubmit = handleSubmit(async (values) => {
        const priced = await priceSession.mutateAsync({
            sessionId: values.sessionId,
            locationId: values.locationId,
            startTime: new Date(values.startTime).toISOString(),
            endTime: new Date(values.endTime).toISOString(),
            energyKwh: Number(values.energyKwh),
        });
        onPriced(priced);
    });

    return (
        <form
            onSubmit={onSubmit}
            className="space-y-4 rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
        >
            <Field label="Session id" error={errors.sessionId?.message}>
                <input className={inputClass} {...register("sessionId")} />
            </Field>
            <Field label="Location" error={errors.locationId?.message}>
                <select className={inputClass} {...register("locationId")}>
                    {locations?.map((location) => (
                        <option key={location.locationId} value={location.locationId}>
                            {location.name}
                        </option>
                    ))}
                </select>
            </Field>
            {selectedLocation && <PlanSummary plan={selectedLocation.plan} />}
            <div className="grid grid-cols-2 gap-4">
                <Field label="Start" error={errors.startTime?.message}>
                    <input type="datetime-local" className={inputClass} {...register("startTime")} />
                </Field>
                <Field label="End" error={errors.endTime?.message}>
                    <input type="datetime-local" className={inputClass} {...register("endTime")} />
                </Field>
            </div>
            <Field label="Energy (kWh)" error={errors.energyKwh?.message}>
                <input type="number" step="0.01" className={inputClass} {...register("energyKwh")} />
            </Field>

            {priceSession.isError && (
                <p role="alert" className="text-sm text-red-600">
                    Could not price the session. Is the API running on port 5080?
                </p>
            )}

            <button
                type="submit"
                disabled={priceSession.isPending}
                className="w-full rounded-lg bg-emerald-700 px-4 py-2 font-semibold text-white transition hover:bg-emerald-800 disabled:opacity-60"
            >
                {priceSession.isPending ? "Pricing…" : "Price session"}
            </button>
        </form>
    );
}

const inputClass =
    "w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-emerald-600 focus:outline-none focus:ring-1 focus:ring-emerald-600";

interface FieldProps {
    label: string;
    error?: string;
    children: React.ReactNode;
}

function Field({ label, error, children }: FieldProps) {
    return (
        <label className="block">
            <span className="mb-1 block text-sm font-medium text-slate-600">{label}</span>
            {children}
            {error && <span className="mt-1 block text-xs text-red-600">{error}</span>}
        </label>
    );
}

/** Short, human-readable summary of a location's pricing plan. */
function PlanSummary({ plan }: { plan: PricingPlan }) {
    const parts = [
        `${formatMoney(plan.energyPricePerKwh, plan.currency)} / kWh`,
        plan.timePricePerMinute > 0
            ? `${formatMoney(plan.timePricePerMinute, plan.currency)} / min`
            : null,
        plan.startFee > 0 ? `${formatMoney(plan.startFee, plan.currency)} start fee` : null,
        `${Math.round(plan.vatRate * 100)}% VAT`,
    ].filter((part): part is string => part !== null);

    return (
        <p className="text-xs text-slate-500" data-testid="plan-summary">
            {parts.join(" · ")}
        </p>
    );
}
