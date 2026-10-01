# Agricultural Core API

This document describes the first HTTP boundary for FarmHelm's Agricultural Core. Request and response contracts belong to the API layer; Domain entities are not returned directly.

| Method | Route | Success |
| --- | --- | --- |
| POST | `/api/farms` | 201 Created |
| POST | `/api/farms/{farmId}/locations` | 201 Created |
| POST | `/api/farms/{farmId}/crops` | 201 Created |
| POST | `/api/farms/{farmId}/mortality-reasons` | 201 Created |
| POST | `/api/crops/{cropId}/varieties` | 201 Created |
| POST | `/api/crops/{cropId}/stages` | 201 Created |
| POST | `/api/batches` | 201 Created |
| POST | `/api/batches/{batchId}/mortality` | 200 OK |
| POST | `/api/batches/{batchId}/plants/{plantId}/mortality` | 200 OK |
| POST | `/api/batches/{batchId}/stage-changes` | 204 No Content |
| POST | `/api/batches/{batchId}/removal` | 204 No Content |
| GET | `/api/batches/{batchId}` | 200 OK |
| GET | `/api/batches?farmId=&cropId=&varietyId=&status=` | 200 OK |

`status` accepts `Active` or `Removed`. Batch removal is a historical lifecycle action, not a delete endpoint.
Batch creation returns a normal 201 response body; it intentionally does not use an MVC generated-location route.

## Errors and validation

The API uses RFC 7807 Problem Details responses. Invalid transport input, Application validation failures, and Domain invariant violations return 400. Missing resources return 404. Application conflicts return 409. Unexpected exceptions return a generic 500 response without implementation or secret details.

There is no authentication, authorization, or API versioning in v1. Those concerns will be introduced only when a concrete requirement exists.
