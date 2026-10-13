import { useEffect, useState } from 'react'
import { listBatches } from '../../api/batches/batchApi'
import type { BatchSummary } from '../../api/batches/batchTypes'
import { getHealth } from '../../api/health/healthApi'
import { masterDataApi } from '../../api/masterData/masterDataApi'
import type { Farm } from '../../api/masterData/masterDataTypes'

type ConnectionStatus = 'checking' | 'connected' | 'unavailable'

export function DashboardPage() {
  const [connectionStatus, setConnectionStatus] = useState<ConnectionStatus>('checking')
  const [farms, setFarms] = useState<Farm[]>([])
  const [batches, setBatches] = useState<BatchSummary[]>([])
  useEffect(() => {
    const controller = new AbortController()
    void getHealth(controller.signal).then((health) => setConnectionStatus(health?.status === 'ok' ? 'connected' : 'unavailable')).catch((error: unknown) => { if (!(error instanceof DOMException && error.name === 'AbortError')) setConnectionStatus('unavailable') })
    void Promise.all([masterDataApi.listFarms(controller.signal), listBatches({}, controller.signal)]).then(([nextFarms, nextBatches]) => { setFarms(nextFarms); setBatches(nextBatches) }).catch(() => { /* Health remains useful if summary reads are temporarily unavailable. */ })
    return () => controller.abort()
  }, [])
  const activeBatches = batches.filter((batch) => batch.status === 'Active').length
  const removedBatches = batches.filter((batch) => batch.status === 'Removed').length
  const recent = [...batches].sort((left, right) => right.plantingDate.localeCompare(left.plantingDate)).slice(0, 5)
  return <section className="page" aria-labelledby="dashboard-title"><div className="page-introduction"><p className="eyebrow">Overview</p><h1 id="dashboard-title">Dashboard</h1><p>Start with setup, then monitor current batches and their agricultural lifecycle.</p></div>
    <div className="metric-grid"><section className="metric-card"><p>Farms</p><strong>{farms.length}</strong></section><section className="metric-card"><p>Active batches</p><strong>{activeBatches}</strong></section><section className="metric-card"><p>Removed batches</p><strong>{removedBatches}</strong></section><section className="metric-card"><p>Recorded batches</p><strong>{batches.length}</strong></section></div>
    <section className="status-card" aria-label="Backend connection status"><p className="status-card-label">Backend connection</p><p className={`connection-status is-${connectionStatus}`}>{connectionStatus === 'checking' ? 'Checking…' : connectionStatus === 'connected' ? 'Connected' : 'Unavailable'}</p></section>
    {farms.length === 0 ? <section className="setup-panel"><h2>Begin FarmHelm setup</h2><p>Create a farm, then configure crops, varieties, stages, locations, and mortality reasons before recording your first batch.</p><a className="button button-primary" href="/farms">Create your first farm</a></section> : <section className="dashboard-section"><div className="page-title-row"><h2>Recent batches</h2><a className="text-link" href="/batches">View all batches</a></div>{recent.length === 0 ? <p>No batches have been recorded yet. <a href="/batches/new">Create a batch</a> when crop setup is ready.</p> : <ul className="simple-list">{recent.map((batch) => <li key={batch.batchId}><a href={`/batches/${batch.batchId}`}>{batch.batchCode}</a> <span>{batch.varietyName} · {batch.plantingDate} · {batch.status}</span></li>)}</ul>}</section>}
  </section>
}
