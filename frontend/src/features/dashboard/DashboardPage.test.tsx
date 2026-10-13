import { render, screen, waitFor } from '@testing-library/react'
import { beforeEach, expect, vi } from 'vitest'
import { getHealth } from '../../api/health/healthApi'
import { DashboardPage } from './DashboardPage'

vi.mock('../../api/health/healthApi', () => ({ getHealth: vi.fn() }))
const mockedGetHealth = vi.mocked(getHealth)

describe('DashboardPage', () => {
  beforeEach(() => mockedGetHealth.mockReset())

  it('shows checking then connected', async () => {
    mockedGetHealth.mockResolvedValue({ status: 'ok' })
    render(<DashboardPage />)
    expect(screen.getByText(/Checking/)).toBeInTheDocument()
    await waitFor(() => expect(screen.getByText('Connected')).toBeInTheDocument())
  })

  it('shows unavailable when health cannot provide a response', async () => {
    mockedGetHealth.mockResolvedValue(undefined as never)
    render(<DashboardPage />)
    await waitFor(() => expect(screen.getByText('Unavailable')).toBeInTheDocument())
  })
})
