import { Outlet, useLocation } from 'react-router-dom'
import { Header } from './Header'
import { Sidebar } from './Sidebar'

const sectionNames: Record<string, string> = {
  '/': 'Dashboard',
  '/farms': 'Farms',
  '/crops': 'Crops',
  '/batches': 'Batches',
  '/settings': 'Settings',
}

export function AppShell() {
  const location = useLocation()
  return (
    <div className="app-shell">
      <Sidebar />
      <div className="workspace">
        <Header sectionName={sectionNames[location.pathname] ?? 'FarmHelm'} />
        <main className="main-content"><Outlet /></main>
      </div>
    </div>
  )
}
