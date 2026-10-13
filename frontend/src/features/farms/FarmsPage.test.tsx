import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, expect, vi } from 'vitest'
import { masterDataApi } from '../../api/masterData/masterDataApi'
import { FarmsPage } from './FarmsPage'

vi.mock('../../api/masterData/masterDataApi', () => ({ masterDataApi: { listFarms: vi.fn(), createFarm: vi.fn(), listLocations: vi.fn(), listMortalityReasons: vi.fn(), createLocation: vi.fn(), createMortalityReason: vi.fn() } }))
const api = vi.mocked(masterDataApi)
const farm = { id: 'farm-1', farmCode: 'FARM-0001', name: 'Test Farm', defaultCurrency: 'LKR', timeZone: 'Asia/Colombo', notes: null }

describe('FarmsPage', () => {
  beforeEach(() => { Object.values(api).forEach((method) => { if (typeof method === 'function') vi.mocked(method).mockReset() }); api.listLocations.mockResolvedValue([]); api.listMortalityReasons.mockResolvedValue([]) })
  it('shows first-run guidance when no Farms exist', async () => {
    api.listFarms.mockResolvedValue([])
    render(<FarmsPage />)
    expect(await screen.findByText(/No farms have been created yet/)).toBeInTheDocument()
  })
  it('creates a Farm with defaults then selects the created Farm and loads its setup', async () => {
    api.listFarms.mockResolvedValueOnce([]).mockResolvedValue([farm]); api.createFarm.mockResolvedValue({ farmId: farm.id, farmCode: farm.farmCode, name: farm.name })
    const user = userEvent.setup(); render(<FarmsPage />)
    await user.type(screen.getByLabelText('Name'), farm.name); await user.click(screen.getByRole('button', { name: 'Create farm' }))
    await waitFor(() => expect(api.createFarm).toHaveBeenCalledWith({ name: farm.name, defaultCurrency: 'LKR', timeZone: 'Asia/Colombo', notes: undefined }))
    expect((await screen.findByLabelText('Farm') as HTMLSelectElement).value).toBe(farm.id)
    await waitFor(() => expect(api.listLocations).toHaveBeenCalledWith(farm.id))
  })
})
