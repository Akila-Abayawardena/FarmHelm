export function formatDateOnly(value: string): string {
  const parts = value.split('-').map(Number)
  if (parts.length !== 3 || parts.some(Number.isNaN)) return value
  const [year, month, day] = parts
  return new Intl.DateTimeFormat(undefined, { day: 'numeric', month: 'short', year: 'numeric' }).format(new Date(year, month - 1, day))
}

export function formatSurvivalRate(value: number): string {
  return `${value.toFixed(2)}%`
}
