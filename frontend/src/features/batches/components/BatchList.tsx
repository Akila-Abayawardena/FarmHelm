import { Link } from 'react-router-dom'
import type { BatchSummary } from '../../../api/batches/batchTypes'
import { formatDateOnly } from '../formatting'

export function BatchList({ batches }: { batches: BatchSummary[] }) {
  return (
    <div className="batch-table-wrapper">
      <table className="batch-table">
        <thead>
          <tr><th scope="col">Batch</th><th scope="col">Variety</th><th scope="col">Planting date</th><th scope="col">Status</th></tr>
        </thead>
        <tbody>
          {batches.map((batch) => (
            <tr key={batch.batchId}>
              <th scope="row"><Link to={`/batches/${batch.batchId}`}>{batch.batchCode}</Link></th>
              <td>{batch.varietyName}</td>
              <td><time dateTime={batch.plantingDate}>{formatDateOnly(batch.plantingDate)}</time></td>
              <td><span className={`status-badge status-badge-${batch.status.toLowerCase()}`}>{batch.status}</span></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
