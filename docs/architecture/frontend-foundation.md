# Frontend Foundation

FarmHelm's frontend uses React, TypeScript, Vite, React Router, a native `fetch` API client, and Vitest with React Testing Library. It uses feature-oriented organization and API-owned contracts. In browser development, Vite proxies `/api` and `/health` to ASP.NET Core.

The frontend runs in two supported modes: browser development through Vite at `http://localhost:5173`, and the production Windows Tauri desktop shell. In desktop mode, the frontend calls the local API sidecar at `http://127.0.0.1:5223` while Tauri manages sidecar startup and shutdown.

The frontend must not contain farm business rules; frontend validation may improve UX but the backend remains authoritative.

The Agricultural Core MVP follows the same boundary: feature pages call small API modules, which use the shared native-fetch transport and frontend-owned contract types. Master-data selectors load dependent records only after their parent Farm or Crop is selected. Client validation is limited to basic required fields and quantities; ownership, active-state, and lifecycle rules remain backend-authoritative.
