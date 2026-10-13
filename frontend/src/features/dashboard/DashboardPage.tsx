import { useEffect, useState } from 'react'
import { getHealth } from '../../api/health/healthApi'

type ConnectionStatus = 'checking' | 'connected' | 'unavailable'

export function DashboardPage() {
  const [connectionStatus, setConnectionStatus] = useState<ConnectionStatus>('checking')

  useEffect(() => {
    const controller = new AbortController()
    void (async () => {
      try {
        const response = await getHealth(controller.signal)
        setConnectionStatus(response?.status === 'ok' ? 'connected' : 'unavailable')
      } catch (error: unknown) {
        if (!(error instanceof DOMException && error.name === 'AbortError')) setConnectionStatus('unavailable')
      }
    })()
    return () => controller.abort()
  }, [])

  return (
    <section className="page" aria-labelledby="dashboard-title">
      <div className="page-introduction">
        <p className="eyebrow">Overview</p>
        <h1 id="dashboard-title">Dashboard</h1>
        <p>FarmHelm will bring operational and business visibility into one connected local workspace.</p>
      </div>
      <section className="status-card" aria-label="Backend connection status">
        <p className="status-card-label">Backend connection</p>
        <p className={`connection-status is-${connectionStatus}`}>
          {connectionStatus === 'checking' ? 'Checking…' : connectionStatus === 'connected' ? 'Connected' : 'Unavailable'}
        </p>
      </section>
    </section>
  )
}
