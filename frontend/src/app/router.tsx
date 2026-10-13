import type { RouteObject } from 'react-router-dom'
import { createBrowserRouter } from 'react-router-dom'
import { AppShell } from '../components/layout/AppShell'
import { BatchesPage } from '../features/batches/BatchesPage'
import { CropsPage } from '../features/crops/CropsPage'
import { DashboardPage } from '../features/dashboard/DashboardPage'
import { FarmsPage } from '../features/farms/FarmsPage'
import { SettingsPage } from '../features/settings/SettingsPage'
import { NotFoundPage } from '../pages/NotFoundPage'

export const routes: RouteObject[] = [
  {
    path: '/',
    element: <AppShell />,
    children: [
      { index: true, element: <DashboardPage /> },
      { path: 'farms', element: <FarmsPage /> },
      { path: 'crops', element: <CropsPage /> },
      { path: 'batches', element: <BatchesPage /> },
      { path: 'settings', element: <SettingsPage /> },
    ],
  },
  { path: '*', element: <NotFoundPage /> },
]

export const router = createBrowserRouter(routes)
