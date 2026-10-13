import { buildApiUrl } from '../../app/config'
import { ApiError } from './ApiError'
import { isProblemDetails } from './problemDetails'

export interface ApiRequestOptions extends Omit<RequestInit, 'body' | 'headers'> {
  body?: unknown
  headers?: HeadersInit
}

async function readJson(response: Response): Promise<unknown | undefined> {
  if (response.status === 204 || response.headers.get('content-length') === '0') return undefined
  const text = await response.text()
  if (!text.trim()) return undefined
  try { return JSON.parse(text) as unknown } catch { return undefined }
}

async function createApiError(response: Response): Promise<ApiError> {
  const body = await readJson(response)
  const problem = isProblemDetails(body) ? body : undefined
  return new ApiError(
    response.status,
    problem?.title || `Request failed (${response.status})`,
    problem?.detail || 'The request could not be completed.',
    problem,
  )
}

async function request<T>(path: string, options: ApiRequestOptions = {}): Promise<T | undefined> {
  const { body, headers: suppliedHeaders, ...requestOptions } = options
  const headers = new Headers(suppliedHeaders)
  headers.set('Accept', 'application/json')
  if (body !== undefined) headers.set('Content-Type', 'application/json')

  const response = await fetch(buildApiUrl(path), {
    ...requestOptions,
    headers,
    body: body === undefined ? undefined : JSON.stringify(body),
  })
  if (!response.ok) throw await createApiError(response)
  return (await readJson(response)) as T | undefined
}

export const apiClient = {
  get<T>(path: string, options?: Omit<ApiRequestOptions, 'body' | 'method'>) {
    return request<T>(path, { ...options, method: 'GET' })
  },
  request,
}
