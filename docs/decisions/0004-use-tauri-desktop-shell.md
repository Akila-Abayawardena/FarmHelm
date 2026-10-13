# Use Tauri as the Desktop Shell

Status: Accepted

## Context

FarmHelm is a Windows 11 desktop application and needs a desktop application shell.

## Decision

Tauri is the desktop application shell.

## Consequences

Tauri bundles the React user interface and hosts the local desktop application. The ASP.NET Core API runs as a self-contained sidecar managed by Tauri, while PostgreSQL remains an external local service.
