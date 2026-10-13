import type { BatchStatus } from '../../../api/batches/batchTypes'

export function BatchEmptyState({ status }: { status?: BatchStatus }) {
  return <p className="batch-state">{status ? `No ${status} batches were found.` : 'No batches have been recorded yet.'}</p>
}
