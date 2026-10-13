import { apiClient } from '../http/apiClient'
import type { CreateCropInput, CreateFarmInput, CreateLocationInput, CreateMortalityReasonInput, CreateStageInput, CreateVarietyInput, CreatedCrop, CreatedFarm, CreatedLocation, CreatedMortalityReason, CreatedStage, CreatedVariety, Crop, CropStage, Farm, FarmLocation, MortalityReason, Variety } from './masterDataTypes'

function nestedPath(parent: string, id: string, child: string, includeInactive = false) {
  const path = `/api/${parent}/${encodeURIComponent(id)}/${child}`
  return includeInactive ? `${path}?includeInactive=true` : path
}

export const masterDataApi = {
  async listFarms(signal?: AbortSignal) { return (await apiClient.get<Farm[]>('/api/farms', { signal })) ?? [] },
  createFarm(input: CreateFarmInput, signal?: AbortSignal) { return apiClient.post<CreatedFarm, CreateFarmInput>('/api/farms', input, { signal }) },
  async listLocations(farmId: string, includeInactive = false, signal?: AbortSignal) { return (await apiClient.get<FarmLocation[]>(nestedPath('farms', farmId, 'locations', includeInactive), { signal })) ?? [] },
  createLocation(farmId: string, input: CreateLocationInput, signal?: AbortSignal) { return apiClient.post<CreatedLocation, CreateLocationInput>(nestedPath('farms', farmId, 'locations'), input, { signal }) },
  async listCrops(farmId: string, includeInactive = false, signal?: AbortSignal) { return (await apiClient.get<Crop[]>(nestedPath('farms', farmId, 'crops', includeInactive), { signal })) ?? [] },
  createCrop(farmId: string, input: CreateCropInput, signal?: AbortSignal) { return apiClient.post<CreatedCrop, CreateCropInput>(nestedPath('farms', farmId, 'crops'), input, { signal }) },
  async listMortalityReasons(farmId: string, includeInactive = false, signal?: AbortSignal) { return (await apiClient.get<MortalityReason[]>(nestedPath('farms', farmId, 'mortality-reasons', includeInactive), { signal })) ?? [] },
  createMortalityReason(farmId: string, input: CreateMortalityReasonInput, signal?: AbortSignal) { return apiClient.post<CreatedMortalityReason, CreateMortalityReasonInput>(nestedPath('farms', farmId, 'mortality-reasons'), input, { signal }) },
  async listVarieties(cropId: string, includeInactive = false, signal?: AbortSignal) { return (await apiClient.get<Variety[]>(nestedPath('crops', cropId, 'varieties', includeInactive), { signal })) ?? [] },
  createVariety(cropId: string, input: CreateVarietyInput, signal?: AbortSignal) { return apiClient.post<CreatedVariety, CreateVarietyInput>(nestedPath('crops', cropId, 'varieties'), input, { signal }) },
  async listCropStages(cropId: string, includeInactive = false, signal?: AbortSignal) { return (await apiClient.get<CropStage[]>(nestedPath('crops', cropId, 'stages', includeInactive), { signal })) ?? [] },
  createCropStage(cropId: string, input: CreateStageInput, signal?: AbortSignal) { return apiClient.post<CreatedStage, CreateStageInput>(nestedPath('crops', cropId, 'stages'), input, { signal }) },
}
