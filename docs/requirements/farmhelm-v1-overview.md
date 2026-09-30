# FarmHelm V1 Requirements Overview

This preliminary document records the currently agreed V1 requirements and will evolve as development continues.

## Platform

- Windows 11 desktop application
- Local/offline operation
- Single user initially
- No authentication in V1
- PostgreSQL database
- English initially
- LKR initially

## Agricultural Core

- Crop-independent architecture
- Crop and Variety are separate concepts
- Batch is the primary planting/production unit
- One batch belongs to one variety
- Original batch plant count is fixed after creation
- Optional individual plant tracking
- Mortality records
- Growth-stage history
- Manual growth-stage changes

## Operations

- Crop treatments
- Farm tasks
- One-time schedules
- Recurring schedules
- Scheduled, Due, Overdue, Done, and Cancelled states
- Calendar with day, week, and month views

## Inventory

- Products
- Suppliers
- Purchases
- Inventory lots
- Stock movements
- Optional treatment usage deduction
- Physical stock reconciliation
- Expiry tracking
- Expired products cannot be selected for new treatments

## Harvest

- Multiple harvest events per batch
- Configurable harvest grades
- Wastage
- Personal use
- Analytical unsold quantity

## Sales

- Customers
- Sales with multiple items
- Optional batch traceability on sale items
- Partial payments
- Customer outstanding balances

## Finance

- Expenses
- Owner capital
- Owner withdrawals
- Loans
- Loan repayments
- Assets
- Asset maintenance

## System

- Dashboard
- Reports
- Global search
- Batch 360-degree view
- Backup and restore
