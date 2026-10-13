# Frontend Foundation

FarmHelm's browser frontend uses React, TypeScript, Vite, React Router, a native `fetch` API client, and Vitest with React Testing Library. It uses feature-oriented organization and API-owned contracts. Vite proxies `/api` and `/health` to ASP.NET Core during local development.

The frontend must not contain farm business rules; frontend validation may improve UX but the backend remains authoritative. Tauri integration is planned later and is not implemented here.
