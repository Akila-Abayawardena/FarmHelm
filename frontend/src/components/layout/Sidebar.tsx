import { NavLink } from 'react-router-dom'

const navigationItems = [
  { to: '/', label: 'Dashboard', end: true },
  { to: '/farms', label: 'Farms' },
  { to: '/crops', label: 'Crops' },
  { to: '/batches', label: 'Batches' },
  { to: '/settings', label: 'Settings' },
]

export function Sidebar() {
  return (
    <aside className="sidebar">
      <NavLink className="product-name" to="/" aria-label="FarmHelm dashboard">FarmHelm</NavLink>
      <nav aria-label="Primary navigation">
        <ul className="navigation-list">
          {navigationItems.map((item) => (
            <li key={item.to}>
              <NavLink to={item.to} end={item.end} className={({ isActive }) => (isActive ? 'navigation-link is-active' : 'navigation-link')}>
                {item.label}
              </NavLink>
            </li>
          ))}
        </ul>
      </nav>
    </aside>
  )
}
