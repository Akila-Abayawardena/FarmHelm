import { apiClient } from '../http/apiClient'
import type { DatabaseHealthResponse, HealthResponse } from './healthTypes'

export async function getHealth(signal?: AbortSignal): Promise<HealthResponse> {
  const response = await apiClient.get<HealthResponse>('/health', { signal })
  if (!response) throw new Error('The health endpoint returned no response body.')
  return response
}

export async function getDatabaseHealth(signal?: AbortSignal): Promise<DatabaseHealthResponse> {
  const response = await apiClient.get<DatabaseHealthResponse>('/health/database', { signal })
  if (!response) throw new Error('The database health endpoint returned no response body.')
  return response
}
