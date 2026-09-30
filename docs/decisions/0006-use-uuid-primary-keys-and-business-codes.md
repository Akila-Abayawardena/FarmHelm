# Use UUID Primary Keys and Business Codes

Status: Accepted

## Context

FarmHelm needs stable internal identifiers and understandable identifiers for people using the system.

## Decision

Internal database identifiers use UUIDs. Human-readable business identifiers such as `FARM-0001`, `BAT-0001`, `HAR-0001`, and `SAL-0001` are separate from internal UUID primary keys. Business codes must not be used as database primary keys.

## Consequences

Database relationships will use UUID primary keys, while business codes can be generated and displayed independently.
