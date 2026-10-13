import type { BatchDetails, BatchSummary } from '../api/batches/batchTypes'

export const batchSummary: BatchSummary = {
  batchId: '11111111-1111-4111-8111-111111111111', batchCode: 'BAT-0042', farmId: '22222222-2222-4222-8222-222222222222', cropId: '33333333-3333-4333-8333-333333333333', varietyId: '44444444-4444-4444-8444-444444444444', varietyName: 'Validation Variety', plantingDate: '2026-10-01', status: 'Active',
}

export const batchDetails: BatchDetails = {
  ...batchSummary,
  cropName: 'Validation Crop', farmName: 'Validation Farm', locationId: '55555555-5555-4555-8555-555555555555', locationName: 'North Field', initialPlantCount: 3, alivePlantCount: 2, deadPlantCount: 1, survivalRate: 66.67, individualTrackingEnabled: true, currentStageId: '66666666-6666-4666-8666-666666666666', currentStageName: 'Vegetative', removedDate: null, notes: 'Synthetic test data.',
  plants: [{ plantId: '77777777-7777-4777-8777-777777777777', plantNumber: 1, plantCode: 'BAT-0042-P001', status: 'Dead', notes: null }, { plantId: '88888888-8888-4888-8888-888888888888', plantNumber: 2, plantCode: 'BAT-0042-P002', status: 'Active', notes: 'Healthy' }],
  mortalityRecords: [{ mortalityRecordId: '99999999-9999-4999-8999-999999999999', plantId: '77777777-7777-4777-8777-777777777777', mortalityDate: '2026-10-08', quantity: 1, reasonId: 'aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa', notes: 'Synthetic history.' }],
  stageHistory: [{ historyId: 'bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb', stageId: '66666666-6666-4666-8666-666666666666', stageName: 'Vegetative', effectiveDate: '2026-10-01', notes: null }],
}
