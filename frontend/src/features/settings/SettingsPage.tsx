import { Link } from 'react-router-dom'

export function SettingsPage() {
  return <section className="page" aria-labelledby="settings-title"><p className="eyebrow">Workspace</p><h1 id="settings-title">Settings</h1><p className="page-description">Agricultural setup data is managed in the focused areas below. Editing and deactivation workflows will follow in a later slice.</p><div className="setup-grid"><section className="setup-panel"><h2>Farm setup</h2><p>Manage farms, locations, and mortality reasons.</p><Link className="text-link" to="/farms">Open Farms</Link></section><section className="setup-panel"><h2>Crop setup</h2><p>Manage crops, varieties, and growth stages.</p><Link className="text-link" to="/crops">Open Crops</Link></section></div></section>
}
