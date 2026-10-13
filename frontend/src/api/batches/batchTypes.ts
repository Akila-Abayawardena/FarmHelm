export type BatchStatus = 'Active' | 'Removed'

export interface BatchSummary {
  batchId: string
  batchCode: string
  farmId: string
  cropId: string
  varietyId: string
  varietyName: string
  plantingDate: string
  status: BatchStatus
}

export interface Plant {
  plantId: string
  plantNumber: number
  plantCode: string
  status: 'Active' | 'Dead' | 'Removed'
  notes: string | null
}

export interface MortalityRecord {
  mortalityRecordId: string
  plantId: string | null
  mortalityDate: string
  quantity: number
  reasonId: string
  notes: string | null
}

export interface BatchStageHistoryEntry {
  historyId: string
  stageId: string
  stageName: string | null
  effectiveDate: string
  notes: string | null
}

export interface BatchDetails {
  batchId: string
  batchCode: string
  cropId: string
  cropName: string
  varietyId: string
  varietyName: string
  farmId: string
  farmName: string
  locationId: string | null
  locationName: string | null
  plantingDate: string
  initialPlantCount: number
  alivePlantCount: number
  deadPlantCount: number
  survivalRate: number
  individualTrackingEnabled: boolean
  currentStageId: string
  currentStageName: string
  status: BatchStatus
  removedDate: string | null
  notes: string | null
  plants: Plant[]
  mortalityRecords: MortalityRecord[]
  stageHistory: BatchStageHistoryEntry[]
}

export interface BatchListFilter {
  farmId?: string
  cropId?: string
  varietyId?: string
  status?: BatchStatus
}
