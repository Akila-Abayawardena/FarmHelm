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

The first read feature provides a Batch list, Active/Removed status filter, and Batch details. Farm, Crop, and Variety selectors await reusable backend master-data read APIs; no write operations are implemented in this frontend yet.

Frontend environment variables are browser-visible. Never put database credentials, private tokens, or other secrets in frontend environment files. See `.env.example`.
