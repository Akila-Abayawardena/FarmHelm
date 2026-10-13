import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { afterEach, expect, vi } from 'vitest'
import { BatchDetailsPage } from './BatchDetailsPage'
import { batchDetails } from '../../test/batchFixtures'

const fetchMock = vi.fn()
afterEach(() => { fetchMock.mockReset(); vi.unstubAllGlobals() })
function response(body: unknown, status = 200) { return new Response(JSON.stringify(body), { status, headers: { 'content-type': 'application/json' } }) }
function renderPage(path = `/batches/${batchDetails.batchId}`) { return render(<MemoryRouter initialEntries={[path]}><Routes><Route path="/batches/:batchId" element={<BatchDetailsPage />} /></Routes></MemoryRouter>) }

describe('BatchDetailsPage', () => {
  it('shows loading before details arrive', () => {
    fetchMock.mockImplementation(() => new Promise(() => undefined)); vi.stubGlobal('fetch', fetchMock)
    renderPage(); expect(screen.getByRole('status')).toHaveTextContent('Loading batch details…')
  })

  it('renders details, production values, plants, mortality, and stage history', async () => {
    fetchMock.mockResolvedValue(response(batchDetails)); vi.stubGlobal('fetch', fetchMock)
    renderPage()
    expect(await screen.findByRole('heading', { name: 'BAT-0042' })).toBeInTheDocument()
    expect(screen.getByText('66.67%')).toBeInTheDocument(); expect(screen.getByText('BAT-0042-P001')).toBeInTheDocument()
    expect(screen.getByText('Synthetic history.')).toBeInTheDocument(); expect(screen.getAllByText('Vegetative').length).toBeGreaterThan(0)
    expect(screen.getByRole('link', { name: 'Back to Batches' })).toHaveAttribute('href', '/batches')
  })

  it('shows a dedicated state for a missing batch', async () => {
    fetchMock.mockResolvedValue(response({ title: 'Not found.' }, 404)); vi.stubGlobal('fetch', fetchMock)
    renderPage(); expect(await screen.findByRole('heading', { name: 'Batch not found.' })).toBeInTheDocument()
  })

  it('shows an error and retries after a non-404 failure', async () => {
    fetchMock.mockRejectedValueOnce(new Error('Offline')).mockResolvedValueOnce(response(batchDetails)); vi.stubGlobal('fetch', fetchMock)
    renderPage(); expect(await screen.findByRole('button', { name: 'Retry' })).toBeInTheDocument()
    await userEvent.setup().click(screen.getByRole('button', { name: 'Retry' }))
    await waitFor(() => expect(fetchMock).toHaveBeenCalledTimes(5))
  })
})
