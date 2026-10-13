import { render, screen, waitFor } from '@testing-library/react'
import { beforeEach, expect, vi } from 'vitest'
import { MemoryRouter } from 'react-router-dom'
import { masterDataApi } from '../../api/masterData/masterDataApi'
import { CropsPage } from './CropsPage'

vi.mock('../../api/masterData/masterDataApi', () => ({ masterDataApi: { listFarms: vi.fn(), listCrops: vi.fn(), listVarieties: vi.fn(), listCropStages: vi.fn(), createCrop: vi.fn(), createVariety: vi.fn(), createCropStage: vi.fn() } }))
const api = vi.mocked(masterDataApi)
const farm = { id: 'farm-1', farmCode: 'FARM-0001', name: 'Test Farm', defaultCurrency: 'LKR', timeZone: 'Asia/Colombo', notes: null }
const crop = { id: 'crop-1', farmId: farm.id, cropCode: 'CROP-0001', name: 'Test Crop', scientificName: null, description: null, isActive: true }

describe('CropsPage', () => {
  beforeEach(() => { Object.values(api).forEach((method) => { if (typeof method === 'function') vi.mocked(method).mockReset() }); api.listVarieties.mockResolvedValue([]); api.listCropStages.mockResolvedValue([]) })
  it('guides a first-run user to Farm setup', async () => {
    api.listFarms.mockResolvedValue([]); render(<MemoryRouter><CropsPage /></MemoryRouter>)
    expect(await screen.findByRole('link', { name: 'Create a Farm first' })).toHaveAttribute('href', '/farms')
  })
  it('loads a Farm Crop and its dependent Variety and Stage data', async () => {
    api.listFarms.mockResolvedValue([farm]); api.listCrops.mockResolvedValue([crop]); api.listVarieties.mockResolvedValue([{ id: 'variety-1', cropId: crop.id, varietyCode: 'VAR-0001', name: 'Test Variety', description: null, isActive: true }]); api.listCropStages.mockResolvedValue([{ id: 'stage-1', cropId: crop.id, name: 'Initial', displayOrder: 1, isActive: true }])
    render(<MemoryRouter><CropsPage /></MemoryRouter>)
    expect(await screen.findByText('Test Variety')).toBeInTheDocument(); expect(screen.getByText('Initial')).toBeInTheDocument()
    await waitFor(() => expect(api.listCrops).toHaveBeenCalledWith(farm.id))
  })
})
