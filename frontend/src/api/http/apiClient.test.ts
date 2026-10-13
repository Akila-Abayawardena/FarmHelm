import { afterEach, expect, vi } from 'vitest'
import { apiClient } from './apiClient'
const fetchMock = vi.fn()
afterEach(() => { fetchMock.mockReset(); vi.unstubAllGlobals() })
describe('apiClient', () => {
  it('parses successful JSON', async () => { fetchMock.mockResolvedValue(new Response(JSON.stringify({ status: 'ok' }), { status: 200 })); vi.stubGlobal('fetch', fetchMock); await expect(apiClient.get<{ status: string }>('/health')).resolves.toEqual({ status: 'ok' }) })
  it('handles no content', async () => { fetchMock.mockResolvedValue(new Response(null, { status: 204 })); vi.stubGlobal('fetch', fetchMock); await expect(apiClient.get('/health')).resolves.toBeUndefined() })
  it('turns Problem Details into ApiError', async () => { fetchMock.mockResolvedValue(new Response(JSON.stringify({ title: 'Validation failed.', status: 400, detail: 'Name is required.' }), { status: 400 })); vi.stubGlobal('fetch', fetchMock); await expect(apiClient.get('/health')).rejects.toMatchObject({ status: 400, title: 'Validation failed.', detail: 'Name is required.' }) })
  it('uses safe fallback for non-JSON failures', async () => { fetchMock.mockResolvedValue(new Response('Unexpected server text', { status: 500 })); vi.stubGlobal('fetch', fetchMock); await expect(apiClient.get('/health')).rejects.toMatchObject({ status: 500, detail: 'The request could not be completed.' }) })
  it('forwards an AbortSignal', async () => { const controller = new AbortController(); fetchMock.mockResolvedValue(new Response(JSON.stringify({ status: 'ok' }), { status: 200 })); vi.stubGlobal('fetch', fetchMock); await apiClient.get('/health', { signal: controller.signal }); expect(fetchMock).toHaveBeenCalledWith('/health', expect.objectContaining({ signal: controller.signal })) })
})
