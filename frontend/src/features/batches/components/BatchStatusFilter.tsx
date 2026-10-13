import type { BatchStatus } from '../../../api/batches/batchTypes'

interface BatchStatusFilterProps {
  value?: BatchStatus
  onChange: (status?: BatchStatus) => void
}

export function BatchStatusFilter({ value, onChange }: BatchStatusFilterProps) {
  return (
    <div className="batch-filter">
      <label htmlFor="batch-status">Status</label>
      <select id="batch-status" value={value ?? ''} onChange={(event) => onChange(event.target.value === '' ? undefined : event.target.value as BatchStatus)}>
        <option value="">All</option>
        <option value="Active">Active</option>
        <option value="Removed">Removed</option>
      </select>
    </div>
  )
}
