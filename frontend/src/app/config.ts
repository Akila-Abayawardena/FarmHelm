const configuredApiOrigin = import.meta.env.VITE_API_ORIGIN?.trim()

export const appConfig = {
  apiOrigin: configuredApiOrigin ? configuredApiOrigin.replace(/\/$/, '') : '',
} as const

export function buildApiUrl(path: string): string {
  if (!path.startsWith('/')) {
    throw new Error('API paths must start with a forward slash.')
  }

  return `${appConfig.apiOrigin}${path}`
}
