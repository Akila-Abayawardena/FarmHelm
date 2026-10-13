import { apiClient } from '../http/apiClient'
import type { BatchDetails, BatchListFilter, BatchSummary } from './batchTypes'

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
