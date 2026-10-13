import { afterEach, expect, vi } from 'vitest'
import { changeBatchStage, createBatch, getBatchDetails, listBatches, recordPlantMortality, removeBatch } from './batchApi'

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

  it('posts a created Batch using API-owned input', async () => {
    fetchMock.mockResolvedValue(success({ batchId: 'batch-id' })); vi.stubGlobal('fetch', fetchMock)
    await createBatch({ farmId: 'farm-id', varietyId: 'variety-id', locationId: null, plantingDate: '2026-10-13', initialPlantCount: 3, initialStageId: 'stage-id', individualTrackingEnabled: true })
    expect(fetchMock).toHaveBeenCalledWith('/api/batches', expect.objectContaining({ method: 'POST' }))
  })

  it('posts lifecycle action routes with encoded identifiers', async () => {
    fetchMock.mockImplementation(() => Promise.resolve(new Response(null, { status: 204 }))); vi.stubGlobal('fetch', fetchMock)
    await recordPlantMortality('batch/id', 'plant/id', { mortalityDate: '2026-10-13', mortalityReasonId: 'reason-id' })
    await changeBatchStage('batch/id', { stageId: 'stage-id', effectiveDate: '2026-10-13' })
    await removeBatch('batch/id', { removedDate: '2026-10-13' })
    expect(fetchMock).toHaveBeenNthCalledWith(1, '/api/batches/batch%2Fid/plants/plant%2Fid/mortality', expect.objectContaining({ method: 'POST' }))
    expect(fetchMock).toHaveBeenNthCalledWith(2, '/api/batches/batch%2Fid/stage-changes', expect.objectContaining({ method: 'POST' }))
    expect(fetchMock).toHaveBeenNthCalledWith(3, '/api/batches/batch%2Fid/removal', expect.objectContaining({ method: 'POST' }))
  })
})
