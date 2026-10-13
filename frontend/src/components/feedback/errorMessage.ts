import { ApiError } from '../../api/http/ApiError'

export function errorMessage(error: unknown): string {
  if (error instanceof ApiError) {
    const errors = error.problemDetails?.errors
    return errors ? Object.values(errors).flat().join(' ') || error.detail : error.detail
  }
  return 'The request could not be completed. Please try again.'
}
