const configuredApiOrigin = import.meta.env.VITE_API_ORIGIN?.trim()
const tauriApiOrigin = import.meta.env.TAURI_ENV_PLATFORM ? 'http://127.0.0.1:5223' : ''

export const appConfig = {
  apiOrigin: configuredApiOrigin ? configuredApiOrigin.replace(/\/$/, '') : tauriApiOrigin,
} as const

export function buildApiUrl(path: string): string {
  if (!path.startsWith('/')) {
    throw new Error('API paths must start with a forward slash.')
  }

  return `${appConfig.apiOrigin}${path}`
}
