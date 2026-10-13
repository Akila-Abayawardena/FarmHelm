import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, expect, vi } from 'vitest'
import { MemoryRouter, Route, Routes, useLocation } from 'react-router-dom'
import { createBatch } from '../../api/batches/batchApi'
import { masterDataApi } from '../../api/masterData/masterDataApi'
import { BatchCreatePage } from './BatchCreatePage'

vi.mock('../../api/batches/batchApi', () => ({ createBatch: vi.fn() }))
vi.mock('../../api/masterData/masterDataApi', () => ({ masterDataApi: { listFarms: vi.fn(), listLocations: vi.fn(), listCrops: vi.fn(), listVarieties: vi.fn(), listCropStages: vi.fn() } }))
const mockedCreateBatch = vi.mocked(createBatch); const api = vi.mocked(masterDataApi)
const farm = { id: 'farm-1', farmCode: 'FARM-0001', name: 'Test Farm', defaultCurrency: 'LKR', timeZone: 'Asia/Colombo', notes: null }
const crop = { id: 'crop-1', farmId: farm.id, cropCode: 'CROP-0001', name: 'Test Crop', scientificName: null, description: null, isActive: true }
function Location() { return <p data-testid="location">{useLocation().pathname}</p> }

describe('BatchCreatePage', () => {
  beforeEach(() => { mockedCreateBatch.mockReset(); api.listFarms.mockReset(); api.listLocations.mockReset(); api.listCrops.mockReset(); api.listVarieties.mockReset(); api.listCropStages.mockReset() })
  it('guides first-run users to Farm setup', async () => {
    api.listFarms.mockResolvedValue([]); render(<MemoryRouter><BatchCreatePage /></MemoryRouter>)
    expect(await screen.findByRole('link', { name: 'Set up farms' })).toHaveAttribute('href', '/farms')
  })
  it('submits dependent selectors with a null optional Location and navigates to details', async () => {
    api.listFarms.mockResolvedValue([farm]); api.listLocations.mockResolvedValue([]); api.listCrops.mockResolvedValue([crop]); api.listVarieties.mockResolvedValue([{ id: 'variety-1', cropId: crop.id, varietyCode: 'VAR-0001', name: 'Test Variety', description: null, isActive: true }]); api.listCropStages.mockResolvedValue([{ id: 'stage-1', cropId: crop.id, name: 'Initial', displayOrder: 1, isActive: true }]); mockedCreateBatch.mockResolvedValue({ batchId: 'batch-1', batchCode: 'BAT-0001', initialPlantCount: 2, individualTrackingEnabled: false })
    const user = userEvent.setup(); render(<MemoryRouter initialEntries={['/batches/new']}><Routes><Route path="/batches/new" element={<BatchCreatePage />} /><Route path="*" element={<Location />} /></Routes></MemoryRouter>)
    await screen.findByRole('option', { name: /Test Variety/ }); await user.clear(screen.getByLabelText('Initial plant count')); await user.type(screen.getByLabelText('Initial plant count'), '2'); await user.click(screen.getByRole('button', { name: 'Create batch' }))
    await waitFor(() => expect(mockedCreateBatch).toHaveBeenCalledWith(expect.objectContaining({ farmId: farm.id, varietyId: 'variety-1', initialStageId: 'stage-1', locationId: null, initialPlantCount: 2 })))
    expect(await screen.findByTestId('location')).toHaveTextContent('/batches/batch-1')
  })
})
