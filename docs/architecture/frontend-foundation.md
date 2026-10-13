# Frontend Foundation

FarmHelm's browser frontend uses React, TypeScript, Vite, React Router, a native `fetch` API client, and Vitest with React Testing Library. It uses feature-oriented organization and API-owned contracts. Vite proxies `/api` and `/health` to ASP.NET Core during local development.

The frontend must not contain farm business rules; frontend validation may improve UX but the backend remains authoritative. Tauri integration is planned later and is not implemented here.

The Batch read slice follows the same boundary: its feature page calls an API module, which uses the shared native-fetch transport and frontend-owned contract types. It currently exposes only the Batch status filter because Farm, Crop, and Variety selectors need future master-data read APIs.
