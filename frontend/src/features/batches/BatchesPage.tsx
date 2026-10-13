import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { listBatches } from '../../api/batches/batchApi'
import type { BatchStatus, BatchSummary } from '../../api/batches/batchTypes'
import { ApiError } from '../../api/http/ApiError'
import { BatchEmptyState } from './components/BatchEmptyState'
import { BatchErrorState } from './components/BatchErrorState'
import { BatchList } from './components/BatchList'
import { BatchStatusFilter } from './components/BatchStatusFilter'

function getStatus(value: string | null): BatchStatus | undefined {
  return value === 'Active' || value === 'Removed' ? value : undefined
}

function getErrorMessage(error: unknown): string {
  return error instanceof ApiError ? error.detail : 'Please check your connection and try again.'
}

export function BatchesPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const status = getStatus(searchParams.get('status'))
  const [batches, setBatches] = useState<BatchSummary[] | undefined>()
  const [error, setError] = useState<string>()
  const [requestVersion, setRequestVersion] = useState(0)

  useEffect(() => {
    const controller = new AbortController()
    setBatches(undefined)
    setError(undefined)
    void listBatches({ status }, controller.signal)
      .then(setBatches)
      .catch((requestError: unknown) => { if (!controller.signal.aborted) setError(getErrorMessage(requestError)) })
    return () => controller.abort()
  }, [status, requestVersion])

  function changeStatus(nextStatus?: BatchStatus) {
    setSearchParams((current) => {
      const next = new URLSearchParams(current)
      if (nextStatus) next.set('status', nextStatus)
      else next.delete('status')
      return next
    })
  }

  return (
    <section className="page" aria-labelledby="batches-title">
      <p className="eyebrow">Agricultural core</p><h1 id="batches-title">Batches</h1>
      <p className="page-description">Review recorded batches and their production lifecycle.</p>
      <BatchStatusFilter value={status} onChange={changeStatus} />
      {batches === undefined && error === undefined && <p className="batch-state" role="status">Loading batches…</p>}
      {error && <BatchErrorState message={error} onRetry={() => setRequestVersion((version) => version + 1)} />}
      {batches && batches.length === 0 && <BatchEmptyState status={status} />}
      {batches && batches.length > 0 && <BatchList batches={batches} />}
    </section>
  )
}
