import { afterEach, expect, vi } from 'vitest'
import { getBatchDetails, listBatches } from './batchApi'

const fetchMock = vi.fn()
afterEach(() => { fetchMock.mockReset(); vi.unstubAllGlobals() })

function success(body: unknown) { return new Response(JSON.stringify(body), { status: 200, headers: { 'content-type': 'application/json' } }) }

describe('batchApi', () => {
  it('lists batches without a query string when filters are empty', async () => {
    fetchMock.mockResolvedValue(success([])); vi.stubGlobal('fetch', fetchMock)
    await listBatches()
    expect(fetchMock).toHaveBeenCalledWith('/api/batches', expect.anything())
  })

  it('encodes a status filter', async () => {
    fetchMock.mockResolvedValue(success([])); vi.stubGlobal('fetch', fetchMock)
    await listBatches({ status: 'Active' })
    expect(fetchMock).toHaveBeenCalledWith('/api/batches?status=Active', expect.anything())
  })

  it('encodes multiple supported filters', async () => {
    fetchMock.mockResolvedValue(success([])); vi.stubGlobal('fetch', fetchMock)
    await listBatches({ farmId: 'farm id', cropId: 'crop/id', varietyId: 'variety id', status: 'Removed' })
    expect(fetchMock).toHaveBeenCalledWith('/api/batches?farmId=farm+id&cropId=crop%2Fid&varietyId=variety+id&status=Removed', expect.anything())
  })

  it('forwards an AbortSignal', async () => {
    const controller = new AbortController(); fetchMock.mockResolvedValue(success([])); vi.stubGlobal('fetch', fetchMock)
    await listBatches({}, controller.signal)
    expect(fetchMock).toHaveBeenCalledWith('/api/batches', expect.objectContaining({ signal: controller.signal }))
  })

  it('gets a batch through an encoded details route', async () => {
    fetchMock.mockResolvedValue(success({ batchId: 'id' })); vi.stubGlobal('fetch', fetchMock)
    await getBatchDetails('batch/id')
    expect(fetchMock).toHaveBeenCalledWith('/api/batches/batch%2Fid', expect.anything())
  })
})
