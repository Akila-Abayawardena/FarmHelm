# Agricultural Core Application Layer

Application orchestrates Agricultural Core use cases. Domain owns intrinsic entity invariants, Batch mortality mechanics, stage history, removal, and calculated values. Application validates relationships that require more than one aggregate.

Repository and unit-of-work contracts are abstractions owned by Application. Infrastructure will implement them later. Business-code generation is also abstracted so Application does not decide persistence or concurrency mechanics. HTTP concerns remain outside Application.

Key cross-aggregate rules are:

```text
Batch Farm       <-> Variety Crop Farm
Batch Variety    <-> CropStage Crop
Batch Farm       <-> FarmLocation Farm
Batch Farm       <-> MortalityReason Farm
```

New operations cannot select inactive master data; existing historical records remain valid.
