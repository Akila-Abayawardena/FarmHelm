import type { ProblemDetails } from './problemDetails'

export class ApiError extends Error {
  public readonly status: number
  public readonly title: string
  public readonly detail: string
  public readonly problemDetails?: ProblemDetails

  public constructor(status: number, title: string, detail: string, problemDetails?: ProblemDetails) {
    super(detail || title)
    this.name = 'ApiError'
    this.status = status
    this.title = title
    this.detail = detail
    this.problemDetails = problemDetails
  }
}
