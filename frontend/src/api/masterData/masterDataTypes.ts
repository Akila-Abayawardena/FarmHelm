export interface Farm { id: string; farmCode: string; name: string; defaultCurrency: string; timeZone: string; notes: string | null }
export interface FarmLocation { id: string; farmId: string; locationCode: string; name: string; description: string | null; isActive: boolean }
export interface Crop { id: string; farmId: string; cropCode: string; name: string; scientificName: string | null; description: string | null; isActive: boolean }
export interface Variety { id: string; cropId: string; varietyCode: string; name: string; description: string | null; isActive: boolean }
export interface CropStage { id: string; cropId: string; name: string; displayOrder: number; isActive: boolean }
export interface MortalityReason { id: string; farmId: string; name: string; displayOrder: number; isActive: boolean }

export interface CreateFarmInput { name: string; defaultCurrency?: string; timeZone?: string; notes?: string }
export interface CreateLocationInput { name: string; description?: string }
export interface CreateCropInput { name: string; scientificName?: string; description?: string }
export interface CreateVarietyInput { name: string; description?: string }
export interface CreateStageInput { name: string; displayOrder: number }
export interface CreateMortalityReasonInput { name: string; displayOrder: number }

export interface CreatedFarm { farmId: string; farmCode: string; name: string }
export interface CreatedLocation { locationId: string; locationCode: string; farmId: string; name: string }
export interface CreatedCrop { cropId: string; cropCode: string; farmId: string; name: string }
export interface CreatedVariety { varietyId: string; varietyCode: string; cropId: string; name: string }
export interface CreatedStage { cropStageId: string; cropId: string; name: string; displayOrder: number }
export interface CreatedMortalityReason { mortalityReasonId: string; farmId: string; name: string; displayOrder: number }
