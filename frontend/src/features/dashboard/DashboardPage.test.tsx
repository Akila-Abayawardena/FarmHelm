import { render, screen, waitFor } from '@testing-library/react'
import { beforeEach, expect, vi } from 'vitest'
import { getHealth } from '../../api/health/healthApi'
import { listBatches } from '../../api/batches/batchApi'
import { masterDataApi } from '../../api/masterData/masterDataApi'
import { MemoryRouter } from 'react-router-dom'
import { DashboardPage } from './DashboardPage'

vi.mock('../../api/health/healthApi', () => ({ getHealth: vi.fn() }))
vi.mock('../../api/batches/batchApi', () => ({ listBatches: vi.fn() }))
vi.mock('../../api/masterData/masterDataApi', () => ({ masterDataApi: { listFarms: vi.fn() } }))
const mockedGetHealth = vi.mocked(getHealth)
const mockedListBatches = vi.mocked(listBatches)
const mockedMasterDataApi = vi.mocked(masterDataApi)

describe('DashboardPage', () => {
  beforeEach(() => { mockedGetHealth.mockReset(); mockedListBatches.mockReset(); mockedMasterDataApi.listFarms.mockReset(); mockedListBatches.mockResolvedValue([]); mockedMasterDataApi.listFarms.mockResolvedValue([]) })

  it('shows checking then connected', async () => {
    mockedGetHealth.mockResolvedValue({ status: 'ok' })
    render(<MemoryRouter><DashboardPage /></MemoryRouter>)
    expect(screen.getByText(/Checking/)).toBeInTheDocument()
    await waitFor(() => expect(screen.getByText('Connected')).toBeInTheDocument())
  })

  it('shows unavailable when health cannot provide a response', async () => {
    mockedGetHealth.mockResolvedValue(undefined as never)
    render(<MemoryRouter><DashboardPage /></MemoryRouter>)
    await waitFor(() => expect(screen.getByText('Unavailable')).toBeInTheDocument())
  })

  it('renders counts and recent Batches independently from health', async () => {
    mockedGetHealth.mockResolvedValue({ status: 'ok' })
    mockedMasterDataApi.listFarms.mockResolvedValue([{ id: 'farm', farmCode: 'FARM-0001', name: 'Test Farm', defaultCurrency: 'LKR', timeZone: 'Asia/Colombo', notes: null }])
    mockedListBatches.mockResolvedValue([{ batchId: 'one', batchCode: 'BAT-0001', farmId: 'farm', cropId: 'crop', varietyId: 'variety', varietyName: 'First variety', plantingDate: '2026-10-13', status: 'Active' }, { batchId: 'two', batchCode: 'BAT-0002', farmId: 'farm', cropId: 'crop', varietyId: 'variety', varietyName: 'Second variety', plantingDate: '2026-10-12', status: 'Removed' }])
    render(<MemoryRouter><DashboardPage /></MemoryRouter>)
    expect(await screen.findByText('BAT-0001')).toBeInTheDocument()
    expect(screen.getByText('Farms')).toBeInTheDocument(); expect(screen.getByText('Active batches')).toBeInTheDocument(); expect(screen.getByText('Removed batches')).toBeInTheDocument()
    expect(screen.getByRole('link', { name: 'BAT-0001' })).toHaveAttribute('href', '/batches/one')
  })

  it('shows onboarding when no Farms are configured', async () => {
    mockedGetHealth.mockResolvedValue({ status: 'ok' })
    render(<MemoryRouter><DashboardPage /></MemoryRouter>)
    expect(await screen.findByText('Begin FarmHelm setup')).toBeInTheDocument()
  })
})
