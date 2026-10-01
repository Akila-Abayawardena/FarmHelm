# Agricultural Core Domain

The Agricultural Core begins with a pure Domain model. It represents farm master data and the Batch production unit without persistence, API, or application-service concerns.

```text
Farm
|- FarmLocation
|- Crop
|  |- Variety
|  `- CropStage
|- MortalityReason
`- Batch
   |- Plant (optional)
   |- MortalityRecord
   `- BatchStageHistory
```

A Batch has an immutable original plant count. Its alive count, dead count, and survival rate are derived from retained mortality history; replacement plants are not added to restore the original count.

Individual plant tracking is optional. When enabled at Batch creation, the Batch owns one Plant record for each original plant and plant mortality is recorded against a specific active Plant. When disabled, mortality is quantity-based.

Growth stage and `BatchStatus` are separate concerns. Stage changes append to retained stage history, while removal changes the Batch lifecycle status without deleting historical information.

The Domain model cannot prove that a selected CropStage belongs to the same Crop as the Batch's Variety from identifiers alone. The Application layer will enforce that compatibility before calling the Batch stage-change behavior.
