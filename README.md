# PricingEngineCase

> This is a standalone solution. It has no Azure dependencies, no authentication, and stores
> all state in-memory. Everything runs locally.

## What it does

1. A **pricing plan** is configured per location (energy price per kWh, time price per
   minute, a start fee, and a VAT rate).
2. A **charging session** (location + start/end time + kWh) is submitted for pricing.
3. The system calculates a **cost breakdown** (energy cost, time cost, start fee, subtotal,
   VAT, total) and stores the priced session.

## Architecture

| Project | Role |
| --- | --- |
| `src/PricingEngineCase.Contracts` | Shared, Orleans-serializable DTOs (plan, request, priced session) + constants. |
| `src/PricingEngineCase.Domain` | Orleans grains (`PricingPlanGrain`, `ChargingSessionGrain`) + the `SessionPriceCalculator` domain service. |
| `src/PricingEngineCase.Api` | ASP.NET Core minimal API that co-hosts the Orleans silo in-process (localhost clustering, in-memory storage). |
| `src/PricingEngineCase.Web` | Next.js (Pages Router) + React + TypeScript + Tailwind frontend. |
| `test/PricingEngineCase.Domain.Tests` | xUnit v3: calculator unit tests + Orleans `TestingHost` grain tests. |
| `test/PricingEngineCase.Api.Tests` | xUnit v3: HTTP endpoint integration tests via `WebApplicationFactory`. |

## Prerequisites

- .NET 10 SDK
- Node.js 20+ (Node 24 recommended)

## Run the backend

```powershell
dotnet run --project src/PricingEngineCase.Api
```

The API listens on `http://localhost:5080`. Explore it at `http://localhost:5080/scalar`.

### HTTP surface

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/v1/locations` | List all locations with their current pricing plans. |
| `PUT` | `/v1/locations/{locationId}/pricing-plan` | Set a location's pricing plan. |
| `GET` | `/v1/locations/{locationId}/pricing-plan` | Read a location's pricing plan. |
| `POST` | `/v1/sessions` | Price a charging session and store the result. |
| `GET` | `/v1/sessions/{sessionId}` | Read a previously priced session. |

## Run the frontend

```powershell
cd src/PricingEngineCase.Web
npm install
npm run dev
```

The frontend runs on `http://localhost:3000` and talks to the API at `http://localhost:5080`
(override with `NEXT_PUBLIC_API_BASE_URL`).

## Tests

```powershell
# Backend (from the solution root)
dotnet test

# Frontend unit tests (Node test runner)
cd src/PricingEngineCase.Web
npm test

# Frontend E2E (Playwright - installs browsers on first run).
# Start the API first (dotnet run --project src/PricingEngineCase.Api);
# Playwright starts the web dev server automatically.
npx playwright install chromium
npm run test:e2e
```
