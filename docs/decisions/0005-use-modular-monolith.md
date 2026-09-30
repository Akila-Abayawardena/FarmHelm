# Use a Modular Monolith

Status: Accepted

## Context

FarmHelm is initially a single-user local application and does not need distributed services.

## Decision

FarmHelm uses a modular monolith rather than microservices.

## Consequences

The application will keep clear internal module boundaries while remaining one deployable system.
