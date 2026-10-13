import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { afterEach, beforeEach, expect, vi } from 'vitest'
import { changeBatchStage, getBatchDetails, removeBatch } from '../../api/batches/batchApi'
import { masterDataApi } from '../../api/masterData/masterDataApi'
import { batchDetails } from '../../test/batchFixtures'
import { BatchDetailsPage } from './BatchDetailsPage'

vi.mock('../../api/batches/batchApi', () => ({ getBatchDetails: vi.fn(), changeBatchStage: vi.fn(), recordBatchMortality: vi.fn(), recordPlantMortality: vi.fn(), removeBatch: vi.fn() }))
vi.mock('../../api/masterData/masterDataApi', () => ({ masterDataApi: { listCropStages: vi.fn(), listMortalityReasons: vi.fn() } }))
const mockedGetDetails = vi.mocked(getBatchDetails); const mockedStageChange = vi.mocked(changeBatchStage); const mockedRemoveBatch = vi.mocked(removeBatch); const api = vi.mocked(masterDataApi)
const reason = { id: batchDetails.mortalityRecords[0].reasonId, farmId: batchDetails.farmId, name: 'Weather damage', displayOrder: 1, isActive: true }
const alternativeStage = { id: 'stage-2', cropId: batchDetails.cropId, name: 'Flowering', displayOrder: 2, isActive: true }
function renderPage() { return render(<MemoryRouter initialEntries={[`/batches/${batchDetails.batchId}`]}><Routes><Route path="/batches/:batchId" element={<BatchDetailsPage />} /></Routes></MemoryRouter>) }

describe('BatchDetails actions', () => {
  beforeEach(() => { mockedGetDetails.mockReset(); mockedStageChange.mockReset(); mockedRemoveBatch.mockReset(); api.listCropStages.mockReset(); api.listMortalityReasons.mockReset(); api.listCropStages.mockResolvedValue([alternativeStage]); api.listMortalityReasons.mockResolvedValue([reason]) })
  afterEach(() => vi.restoreAllMocks())
  it('shows batch mortality for non-individual Batches and resolves historical reason names', async () => {
    mockedGetDetails.mockResolvedValue({ ...batchDetails, individualTrackingEnabled: false, plants: [] }); renderPage()
    expect(await screen.findByText('Record batch mortality')).toBeInTheDocument(); expect(screen.queryByText('Record plant mortality')).not.toBeInTheDocument(); expect((await screen.findAllByText('Weather damage')).length).toBeGreaterThan(0)
  })
  it('shows only active Plant mortality controls for individually tracked Batches', async () => {
    mockedGetDetails.mockResolvedValue(batchDetails); renderPage()
    expect(await screen.findByText('Record plant mortality')).toBeInTheDocument(); expect(screen.queryByText('Record batch mortality')).not.toBeInTheDocument(); expect(screen.getByRole('option', { name: /BAT-0042-P002/ })).toBeInTheDocument(); expect(screen.queryByRole('option', { name: /BAT-0042-P001/ })).not.toBeInTheDocument()
  })
  it('refreshes details after a stage mutation', async () => {
    mockedGetDetails.mockResolvedValue(batchDetails); mockedStageChange.mockResolvedValue(undefined); const user = userEvent.setup(); renderPage()
    await screen.findByRole('button', { name: 'Change stage' }); await user.click(screen.getByRole('button', { name: 'Change stage' }))
    await waitFor(() => expect(mockedStageChange).toHaveBeenCalled()); await waitFor(() => expect(mockedGetDetails).toHaveBeenCalledTimes(2))
  })
  it('hides mutation controls after confirmed historical removal', async () => {
    mockedGetDetails.mockResolvedValueOnce(batchDetails).mockResolvedValueOnce({ ...batchDetails, status: 'Removed', removedDate: '2026-10-13' }); mockedRemoveBatch.mockResolvedValue(undefined); vi.spyOn(window, 'confirm').mockReturnValue(true)
    const user = userEvent.setup(); renderPage(); await screen.findByText('Mark as removed'); await user.click(screen.getByRole('button', { name: 'Mark as removed' }))
    expect(await screen.findByText(/This batch is removed/)).toBeInTheDocument(); expect(screen.queryByRole('button', { name: 'Record mortality' })).not.toBeInTheDocument()
  })
})
