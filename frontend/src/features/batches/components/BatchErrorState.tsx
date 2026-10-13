interface BatchErrorStateProps {
  message: string
  onRetry: () => void
}

export function BatchErrorState({ message, onRetry }: BatchErrorStateProps) {
  return <section className="batch-state batch-error" aria-labelledby="batch-error-title"><h2 id="batch-error-title">Unable to load batches</h2><p>{message}</p><button type="button" onClick={onRetry}>Retry</button></section>
}
