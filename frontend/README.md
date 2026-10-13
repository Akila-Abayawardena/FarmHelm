# FarmHelm Frontend

React, TypeScript, and Vite frontend for the FarmHelm Agricultural Core MVP.

## Commands

```powershell
npm ci
npm run dev
npm run build
npm run typecheck
npm run lint
npm test
npm run desktop:dev
npm run desktop:build
```

For browser development, Vite proxies `/api` and `/health` to `http://localhost:5222`; override this safe local target with `FARMHELM_BACKEND_PROXY_TARGET`. `VITE_API_ORIGIN` is available for an alternate API origin when needed.

The Agricultural Core MVP provides first-run Farm setup, locations, mortality reasons, Crop/Variety/Stage setup, Batch creation, Batch list/details, and lifecycle actions for mortality, stage changes, and historical removal. It uses the backend master-data read APIs for dependent selectors; Farm/Crop/Variety filters remain intentionally deferred from the Batch list.

Treatments and tasks, inventory and purchasing, harvest and sales, finance and assets, reports, calendar/search, and authentication are future scope.

Frontend environment variables are browser-visible. Never put database credentials, private tokens, or other secrets in frontend environment files. See `.env.example`.

## Windows desktop (Tauri)

On a configured Windows 11 development/demo machine, run `npm run desktop:dev` to prepare the sidecar and start the React/Vite UI in Tauri. FarmHelm.Api runs automatically as a self-contained loopback sidecar on `127.0.0.1:5223`; do not manually start the API for desktop mode.

Run `npm run desktop:build` to create the unsigned Windows x64 NSIS package under `src-tauri/target/release/bundle/nsis/`. PostgreSQL is not bundled: the PostgreSQL service, FarmHelm database, and existing ASP.NET User Secret connection string remain prerequisites. Browser development (`npm run dev` plus the API at port 5222) remains supported separately.
