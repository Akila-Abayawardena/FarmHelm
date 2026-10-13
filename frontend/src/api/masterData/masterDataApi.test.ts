import { afterEach, expect, vi } from 'vitest'
import { masterDataApi } from './masterDataApi'

const fetchMock = vi.fn()
const success = (body: unknown) => new Response(JSON.stringify(body), { status: 200, headers: { 'content-type': 'application/json' } })
afterEach(() => { fetchMock.mockReset(); vi.unstubAllGlobals() })

describe('masterDataApi', () => {
  it('uses the Farm master-data routes and only requests inactive records when asked', async () => {
    fetchMock.mockImplementation(() => Promise.resolve(success([]))); vi.stubGlobal('fetch', fetchMock)
    await masterDataApi.listLocations('farm/id')
    await masterDataApi.listCrops('farm/id', true)
    expect(fetchMock).toHaveBeenNthCalledWith(1, '/api/farms/farm%2Fid/locations', expect.anything())
    expect(fetchMock).toHaveBeenNthCalledWith(2, '/api/farms/farm%2Fid/crops?includeInactive=true', expect.anything())
  })

  it('uses dependent Crop routes and forwards cancellation', async () => {
    const controller = new AbortController(); fetchMock.mockImplementation(() => Promise.resolve(success([]))); vi.stubGlobal('fetch', fetchMock)
    await masterDataApi.listVarieties('crop/id', false, controller.signal)
    await masterDataApi.listCropStages('crop/id', true)
    expect(fetchMock).toHaveBeenNthCalledWith(1, '/api/crops/crop%2Fid/varieties', expect.objectContaining({ signal: controller.signal }))
    expect(fetchMock).toHaveBeenNthCalledWith(2, '/api/crops/crop%2Fid/stages?includeInactive=true', expect.anything())
  })

  it('posts API-owned Farm creation input', async () => {
    fetchMock.mockResolvedValue(success({ farmId: 'farm-id', farmCode: 'FARM-0001', name: 'Test Farm' })); vi.stubGlobal('fetch', fetchMock)
    await masterDataApi.createFarm({ name: 'Test Farm', defaultCurrency: 'LKR', timeZone: 'Asia/Colombo' })
    expect(fetchMock).toHaveBeenCalledWith('/api/farms', expect.objectContaining({ method: 'POST', body: JSON.stringify({ name: 'Test Farm', defaultCurrency: 'LKR', timeZone: 'Asia/Colombo' }) }))
  })
})
