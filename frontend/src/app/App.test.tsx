import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { createMemoryRouter, RouterProvider } from 'react-router-dom'
import { vi } from 'vitest'
import { routes } from './router'

vi.mock('../api/health/healthApi', () => ({ getHealth: vi.fn(() => new Promise(() => undefined)) }))

function renderRoute(path: string) {
  return render(<RouterProvider router={createMemoryRouter(routes, { initialEntries: [path] })} />)
}

describe('application shell', () => {
  it('renders FarmHelm and navigation', () => {
    renderRoute('/')
    expect(screen.getByText('FarmHelm')).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Dashboard' })).toBeInTheDocument()
    for (const label of ['Dashboard', 'Farms', 'Crops', 'Batches', 'Settings']) expect(screen.getByRole('link', { name: label })).toBeInTheDocument()
  })

  it('navigates to Batches', async () => {
    const user = userEvent.setup()
    renderRoute('/')
    await user.click(screen.getByRole('link', { name: 'Batches' }))
    expect(screen.getByRole('heading', { name: 'Batches' })).toBeInTheDocument()
  })

  it('renders Not Found for an unknown route', () => {
    renderRoute('/missing')
    expect(screen.getByRole('heading', { name: 'Page not found' })).toBeInTheDocument()
  })
})
