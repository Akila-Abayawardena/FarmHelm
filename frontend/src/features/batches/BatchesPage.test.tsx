import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { createMemoryRouter, MemoryRouter, RouterProvider } from 'react-router-dom'
import { afterEach, expect, vi } from 'vitest'
import { BatchesPage } from './BatchesPage'
import { batchSummary } from '../../test/batchFixtures'
import { batchDetails } from '../../test/batchFixtures'
import { routes } from '../../app/router'

const fetchMock = vi.fn()
afterEach(() => { fetchMock.mockReset(); vi.unstubAllGlobals() })

function renderPage(route = '/batches') { return render(<MemoryRouter initialEntries={[route]}><BatchesPage /></MemoryRouter>) }
function response(body: unknown) { return new Response(JSON.stringify(body), { status: 200, headers: { 'content-type': 'application/json' } }) }

describe('BatchesPage', () => {
  it('shows a loading state', () => {
    fetchMock.mockImplementation(() => new Promise(() => undefined)); vi.stubGlobal('fetch', fetchMock)
    renderPage()
    expect(screen.getByRole('status')).toHaveTextContent('Loading batches…')
  })

  it('renders a successful list and details link', async () => {
    fetchMock.mockResolvedValue(response([batchSummary])); vi.stubGlobal('fetch', fetchMock)
    renderPage()
    expect(await screen.findByRole('link', { name: 'BAT-0042' })).toHaveAttribute('href', `/batches/${batchSummary.batchId}`)
    expect(screen.getByText('Validation Variety')).toBeInTheDocument()
  })

  it('renders the unfiltered empty state', async () => {
    fetchMock.mockResolvedValue(response([])); vi.stubGlobal('fetch', fetchMock)
    renderPage()
    expect(await screen.findByText('No batches have been recorded yet.')).toBeInTheDocument()
  })

  it('shows an error and retries the current request', async () => {
    fetchMock.mockRejectedValueOnce(new Error('Offline')).mockResolvedValueOnce(response([])); vi.stubGlobal('fetch', fetchMock)
    renderPage()
    expect(await screen.findByRole('button', { name: 'Retry' })).toBeInTheDocument()
    await userEvent.setup().click(screen.getByRole('button', { name: 'Retry' }))
    await waitFor(() => expect(fetchMock).toHaveBeenCalledTimes(2))
  })

  it('uses status filters from the select and URL safely', async () => {
    fetchMock.mockResolvedValue(response([])); vi.stubGlobal('fetch', fetchMock)
    const user = userEvent.setup(); renderPage('/batches?status=invalid')
    expect((await screen.findByLabelText('Status') as HTMLSelectElement).value).toBe('')
    expect(fetchMock).toHaveBeenCalledWith('/api/batches', expect.anything())
    await user.selectOptions(screen.getByLabelText('Status'), 'Active')
    await waitFor(() => expect(fetchMock).toHaveBeenLastCalledWith('/api/batches?status=Active', expect.anything()))
    await user.selectOptions(screen.getByLabelText('Status'), 'Removed')
    await waitFor(() => expect(fetchMock).toHaveBeenLastCalledWith('/api/batches?status=Removed', expect.anything()))
  })

  it('navigates from a batch list link to batch details', async () => {
    fetchMock.mockResolvedValueOnce(response([batchSummary])).mockResolvedValueOnce(response(batchDetails)); vi.stubGlobal('fetch', fetchMock)
    const router = createMemoryRouter(routes, { initialEntries: ['/batches'] })
    render(<RouterProvider router={router} />)
    await userEvent.setup().click(await screen.findByRole('link', { name: 'BAT-0042' }))
    expect(await screen.findByRole('heading', { name: 'BAT-0042' })).toBeInTheDocument()
  })
})
