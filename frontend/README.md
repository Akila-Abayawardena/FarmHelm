# FarmHelm Frontend

React, TypeScript, and Vite frontend.

## Commands

```powershell
npm install
npm run dev
npm run build
npm run typecheck
npm run lint
npm test
```

Vite proxies `/api` and `/health` to `http://localhost:5222` during development; override this safe local target with `FARMHELM_BACKEND_PROXY_TARGET`. `VITE_API_ORIGIN` is available for a future alternate origin. Tauri and authentication are not implemented yet.

The Agricultural Core MVP provides first-run Farm setup, locations, mortality reasons, Crop/Variety/Stage setup, Batch creation, Batch list/details, and lifecycle actions for mortality, stage changes, and historical removal. It uses the backend master-data read APIs for dependent selectors; Farm/Crop/Variety filters remain intentionally deferred from the Batch list.

Treatments and tasks, inventory and purchasing, harvest and sales, finance and assets, reports, calendar/search, authentication, and Tauri desktop integration are future scope.

Frontend environment variables are browser-visible. Never put database credentials, private tokens, or other secrets in frontend environment files. See `.env.example`.

## Windows desktop (Tauri)

On a configured Windows 11 development/demo machine, run `npm run desktop:dev` to bundle the React UI and start FarmHelm.Api as a self-contained loopback sidecar on `127.0.0.1:5223`. Run `npm run desktop:build` to create the unsigned NSIS package. PostgreSQL is not bundled: the PostgreSQL service, FarmHelm database, and existing ASP.NET User Secret connection string remain prerequisites. Browser development (`npm run dev` plus the API at port 5222) remains supported separately.
