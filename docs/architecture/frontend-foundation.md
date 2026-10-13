# Frontend Foundation

FarmHelm's browser frontend uses React, TypeScript, Vite, React Router, a native `fetch` API client, and Vitest with React Testing Library. It uses feature-oriented organization and API-owned contracts. Vite proxies `/api` and `/health` to ASP.NET Core during local development.

The frontend must not contain farm business rules; frontend validation may improve UX but the backend remains authoritative. Tauri integration is planned later and is not implemented here.

The Agricultural Core MVP follows the same boundary: feature pages call small API modules, which use the shared native-fetch transport and frontend-owned contract types. Master-data selectors load dependent records only after their parent Farm or Crop is selected. Client validation is limited to basic required fields and quantities; ownership, active-state, and lifecycle rules remain backend-authoritative.
