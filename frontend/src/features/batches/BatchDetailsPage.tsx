import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getBatchDetails } from '../../api/batches/batchApi'
import type { BatchDetails } from '../../api/batches/batchTypes'
import { ApiError } from '../../api/http/ApiError'
import { BatchErrorState } from './components/BatchErrorState'
import { formatDateOnly, formatSurvivalRate } from './formatting'

const guidPattern = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i

function DetailRow({ label, children }: { label: string, children: React.ReactNode }) {
  return <div><dt>{label}</dt><dd>{children}</dd></div>
}

function DetailSection({ title, children }: { title: string, children: React.ReactNode }) {
  return <section className="batch-details-section"><h2>{title}</h2>{children}</section>
}

function date(value: string | null): React.ReactNode {
  return value ? <time dateTime={value}>{formatDateOnly(value)}</time> : 'Not recorded'
}

function BatchDetailsContent({ batch }: { batch: BatchDetails }) {
  return <>
    <div className="batch-details-header">
      <div><Link className="text-link back-link" to="/batches">Back to Batches</Link><h1 id="batch-details-title">{batch.batchCode}</h1></div>
      <span className={`status-badge status-badge-${batch.status.toLowerCase()}`}>{batch.status}</span>
    </div>
    <DetailSection title="Overview"><dl className="batch-details-grid">
      <DetailRow label="Farm">{batch.farmName}</DetailRow><DetailRow label="Crop">{batch.cropName}</DetailRow>
      <DetailRow label="Variety">{batch.varietyName}</DetailRow><DetailRow label="Location">{batch.locationName ?? 'Not assigned'}</DetailRow>
      <DetailRow label="Planting date">{date(batch.plantingDate)}</DetailRow><DetailRow label="Current stage">{batch.currentStageName}</DetailRow>
    </dl></DetailSection>
    <DetailSection title="Production"><dl className="batch-details-grid">
      <DetailRow label="Initial plants">{batch.initialPlantCount}</DetailRow><DetailRow label="Alive plants">{batch.alivePlantCount}</DetailRow>
      <DetailRow label="Dead plants">{batch.deadPlantCount}</DetailRow><DetailRow label="Survival rate">{formatSurvivalRate(batch.survivalRate)}</DetailRow>
      <DetailRow label="Individual tracking">{batch.individualTrackingEnabled ? 'Enabled' : 'Not enabled'}</DetailRow>
    </dl></DetailSection>
    <DetailSection title="Lifecycle"><dl className="batch-details-grid">
      <DetailRow label="Removed date">{date(batch.removedDate)}</DetailRow><DetailRow label="Notes">{batch.notes ?? 'No notes recorded.'}</DetailRow>
    </dl></DetailSection>
    <DetailSection title="Individual plants">
      {!batch.individualTrackingEnabled && <p>Individual plant tracking is not enabled for this batch.</p>}
      {batch.individualTrackingEnabled && batch.plants.length === 0 && <p>No individual plants are available.</p>}
      {batch.individualTrackingEnabled && batch.plants.length > 0 && <div className="batch-table-wrapper"><table className="batch-table"><thead><tr><th scope="col">Number</th><th scope="col">Plant code</th><th scope="col">Status</th><th scope="col">Notes</th></tr></thead><tbody>
        {batch.plants.map((plant) => <tr key={plant.plantId}><td>{plant.plantNumber}</td><td>{plant.plantCode}</td><td>{plant.status}</td><td>{plant.notes ?? '—'}</td></tr>)}
      </tbody></table></div>}
    </DetailSection>
    <DetailSection title="Mortality history">
      {batch.mortalityRecords.length === 0 && <p>No mortality recorded.</p>}
      {batch.mortalityRecords.length > 0 && <div className="batch-table-wrapper"><table className="batch-table"><thead><tr><th scope="col">Date</th><th scope="col">Quantity</th><th scope="col">Plant reference</th><th scope="col">Reason reference</th><th scope="col">Notes</th></tr></thead><tbody>
        {batch.mortalityRecords.map((record) => <tr key={record.mortalityRecordId}><td>{date(record.mortalityDate)}</td><td>{record.quantity}</td><td>{record.plantId ?? 'Batch-level'}</td><td>{record.reasonId}</td><td>{record.notes ?? '—'}</td></tr>)}
      </tbody></table></div>}
    </DetailSection>
    <DetailSection title="Stage history">
      {batch.stageHistory.length === 0 && <p>No stage history recorded.</p>}
      {batch.stageHistory.length > 0 && <div className="batch-table-wrapper"><table className="batch-table"><thead><tr><th scope="col">Stage</th><th scope="col">Effective date</th><th scope="col">Notes</th></tr></thead><tbody>
        {batch.stageHistory.map((entry) => <tr key={entry.historyId}><td>{entry.stageName ?? 'Stage unavailable'}</td><td>{date(entry.effectiveDate)}</td><td>{entry.notes ?? '—'}</td></tr>)}
      </tbody></table></div>}
    </DetailSection>
  </>
}

export function BatchDetailsPage() {
  const { batchId } = useParams()
  const [batch, setBatch] = useState<BatchDetails>()
  const [error, setError] = useState<string>()
  const [notFound, setNotFound] = useState(false)
  const [requestVersion, setRequestVersion] = useState(0)
  const isValidId = batchId !== undefined && guidPattern.test(batchId)

  useEffect(() => {
    if (!isValidId || !batchId) { setNotFound(true); return }
    const controller = new AbortController()
    setBatch(undefined); setError(undefined); setNotFound(false)
    void getBatchDetails(batchId, controller.signal)
      .then(setBatch)
      .catch((requestError: unknown) => {
        if (controller.signal.aborted) return
        if (requestError instanceof ApiError && requestError.status === 404) setNotFound(true)
        else setError(requestError instanceof ApiError ? requestError.detail : 'Please check your connection and try again.')
      })
    return () => controller.abort()
  }, [batchId, isValidId, requestVersion])

  if (notFound) return <section className="page batch-details" aria-labelledby="batch-not-found-title"><h1 id="batch-not-found-title">Batch not found.</h1><p>The requested batch is unavailable.</p><Link className="text-link" to="/batches">Back to Batches</Link></section>
  if (error) return <section className="page batch-details" aria-labelledby="batch-details-title"><h1 id="batch-details-title">Batch details</h1><BatchErrorState message={error} onRetry={() => setRequestVersion((version) => version + 1)} /></section>
  if (!batch) return <section className="page batch-details" aria-labelledby="batch-details-title"><h1 id="batch-details-title">Batch details</h1><p className="batch-state" role="status">Loading batch details…</p></section>
  return <section className="page batch-details" aria-labelledby="batch-details-title"><BatchDetailsContent batch={batch} /></section>
}
