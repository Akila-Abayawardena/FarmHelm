import { apiClient } from '../http/apiClient'
import type { BatchDetails, BatchListFilter, BatchSummary, ChangeBatchStageInput, CreateBatchInput, CreatedBatch, RecordBatchMortalityInput, RecordPlantMortalityInput, RemoveBatchInput } from './batchTypes'

function buildListPath(filter: BatchListFilter): string {
  const parameters = new URLSearchParams()
  for (const [key, value] of Object.entries(filter)) {
    if (value?.trim()) parameters.set(key, value)
  }

  const query = parameters.toString()
  return query ? `/api/batches?${query}` : '/api/batches'
}

export async function listBatches(filter: BatchListFilter = {}, signal?: AbortSignal): Promise<BatchSummary[]> {
  return (await apiClient.get<BatchSummary[]>(buildListPath(filter), { signal })) ?? []
}

export async function getBatchDetails(batchId: string, signal?: AbortSignal): Promise<BatchDetails> {
  const response = await apiClient.get<BatchDetails>(`/api/batches/${encodeURIComponent(batchId)}`, { signal })
  if (!response) throw new Error('The batch details endpoint returned no response body.')
  return response
}

export const createBatch = (input: CreateBatchInput, signal?: AbortSignal) => apiClient.post<CreatedBatch, CreateBatchInput>('/api/batches', input, { signal })
export const recordBatchMortality = (batchId: string, input: RecordBatchMortalityInput, signal?: AbortSignal) => apiClient.post(`/api/batches/${encodeURIComponent(batchId)}/mortality`, input, { signal })
export const recordPlantMortality = (batchId: string, plantId: string, input: RecordPlantMortalityInput, signal?: AbortSignal) => apiClient.post(`/api/batches/${encodeURIComponent(batchId)}/plants/${encodeURIComponent(plantId)}/mortality`, input, { signal })
export const changeBatchStage = (batchId: string, input: ChangeBatchStageInput, signal?: AbortSignal) => apiClient.post<void, ChangeBatchStageInput>(`/api/batches/${encodeURIComponent(batchId)}/stage-changes`, input, { signal })
export const removeBatch = (batchId: string, input: RemoveBatchInput, signal?: AbortSignal) => apiClient.post<void, RemoveBatchInput>(`/api/batches/${encodeURIComponent(batchId)}/removal`, input, { signal })
