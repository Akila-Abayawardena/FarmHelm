namespace FarmHelm.Application.MasterData;

public sealed record FarmReadModel(Guid Id, string FarmCode, string Name, string DefaultCurrency, string TimeZone, string? Notes);
public sealed record FarmLocationReadModel(Guid Id, Guid FarmId, string LocationCode, string Name, string? Description, bool IsActive);
public sealed record CropReadModel(Guid Id, Guid FarmId, string CropCode, string Name, string? ScientificName, string? Description, bool IsActive);
public sealed record VarietyReadModel(Guid Id, Guid CropId, string VarietyCode, string Name, string? Description, bool IsActive);
public sealed record CropStageReadModel(Guid Id, Guid CropId, string Name, int DisplayOrder, bool IsActive);
public sealed record MortalityReasonReadModel(Guid Id, Guid FarmId, string Name, int DisplayOrder, bool IsActive);
