using FarmHelm.Application.Abstractions.Persistence;
using FarmHelm.Application.Batches;
using Microsoft.EntityFrameworkCore;

namespace FarmHelm.Infrastructure.Persistence.Repositories;

public sealed class BatchReadRepository(FarmHelmDbContext context) : IBatchReadRepository
{
    public async Task<BatchDetails?> GetDetailsByIdAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        var batch = await (
                from candidate in context.Batches.AsNoTracking()
                join farm in context.Farms.AsNoTracking() on candidate.FarmId equals farm.Id
                join variety in context.Varieties.AsNoTracking() on candidate.VarietyId equals variety.Id
                join crop in context.Crops.AsNoTracking() on variety.CropId equals crop.Id
                join stage in context.CropStages.AsNoTracking() on candidate.CurrentStageId equals stage.Id
                join location in context.FarmLocations.AsNoTracking() on candidate.LocationId equals (Guid?)location.Id into locations
                from location in locations.DefaultIfEmpty()
                where candidate.Id == batchId
                select new BatchMetadata(
                    candidate.Id,
                    candidate.BatchCode,
                    crop.Id,
                    crop.Name,
                    variety.Id,
                    variety.Name,
                    farm.Id,
                    farm.Name,
                    candidate.LocationId,
                    location == null ? null : location.Name,
                    candidate.PlantingDate,
                    candidate.InitialPlantCount,
                    candidate.IndividualTrackingEnabled,
                    candidate.CurrentStageId,
                    stage.Name,
                    candidate.Status,
                    candidate.RemovedDate,
                    candidate.Notes))
            .SingleOrDefaultAsync(cancellationToken);

        if (batch is null)
        {
            return null;
        }

        var plants = await context.Plants
            .AsNoTracking()
            .Where(plant => plant.BatchId == batchId)
            .OrderBy(plant => plant.PlantNumber)
            .ThenBy(plant => plant.Id)
            .Select(plant => new PlantDetails(plant.Id, plant.PlantNumber, plant.PlantCode, plant.Status, plant.Notes))
            .ToListAsync(cancellationToken);
        var mortalityRecords = await context.MortalityRecords
            .AsNoTracking()
            .Where(record => record.BatchId == batchId)
            .OrderBy(record => record.MortalityDate)
            .ThenBy(record => record.Id)
            .Select(record => new MortalityRecordDetails(record.Id, record.PlantId, record.MortalityDate, record.Quantity, record.ReasonId, record.Notes))
            .ToListAsync(cancellationToken);
        var stageHistory = await (
                from history in context.BatchStageHistories.AsNoTracking()
                join stage in context.CropStages.AsNoTracking() on history.StageId equals stage.Id
                where history.BatchId == batchId
                orderby history.EffectiveDate, history.Id
                select new BatchStageHistoryDetails(history.Id, history.StageId, stage.Name, history.EffectiveDate, history.Notes))
            .ToListAsync(cancellationToken);

        var deadPlantCount = mortalityRecords.Sum(record => record.Quantity);
        return new BatchDetails(
            batch.BatchId,
            batch.BatchCode,
            batch.CropId,
            batch.CropName,
            batch.VarietyId,
            batch.VarietyName,
            batch.FarmId,
            batch.FarmName,
            batch.LocationId,
            batch.LocationName,
            batch.PlantingDate,
            batch.InitialPlantCount,
            batch.InitialPlantCount - deadPlantCount,
            deadPlantCount,
            batch.InitialPlantCount == 0 ? 0m : (batch.InitialPlantCount - deadPlantCount) * 100m / batch.InitialPlantCount,
            batch.IndividualTrackingEnabled,
            batch.CurrentStageId,
            batch.CurrentStageName,
            batch.Status,
            batch.RemovedDate,
            batch.Notes,
            plants,
            mortalityRecords,
            stageHistory);
    }

    public async Task<IReadOnlyList<BatchSummary>> ListAsync(BatchListFilter filter, CancellationToken cancellationToken = default)
    {
        var query =
            from batch in context.Batches.AsNoTracking()
            join variety in context.Varieties.AsNoTracking() on batch.VarietyId equals variety.Id
            join crop in context.Crops.AsNoTracking() on variety.CropId equals crop.Id
            select new { Batch = batch, Variety = variety, Crop = crop };

        if (filter.FarmId.HasValue)
        {
            query = query.Where(item => item.Batch.FarmId == filter.FarmId.Value);
        }

        if (filter.CropId.HasValue)
        {
            query = query.Where(item => item.Crop.Id == filter.CropId.Value);
        }

        if (filter.VarietyId.HasValue)
        {
            query = query.Where(item => item.Variety.Id == filter.VarietyId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(item => item.Batch.Status == filter.Status.Value);
        }

        return await query
            .OrderByDescending(item => item.Batch.PlantingDate)
            .ThenBy(item => item.Batch.BatchCode)
            .Select(item => new BatchSummary(
                item.Batch.Id,
                item.Batch.BatchCode,
                item.Batch.FarmId,
                item.Crop.Id,
                item.Variety.Id,
                item.Variety.Name,
                item.Batch.PlantingDate,
                item.Batch.Status))
            .ToListAsync(cancellationToken);
    }

    private sealed record BatchMetadata(
        Guid BatchId,
        string BatchCode,
        Guid CropId,
        string CropName,
        Guid VarietyId,
        string VarietyName,
        Guid FarmId,
        string FarmName,
        Guid? LocationId,
        string? LocationName,
        DateOnly PlantingDate,
        int InitialPlantCount,
        bool IndividualTrackingEnabled,
        Guid CurrentStageId,
        string CurrentStageName,
        FarmHelm.Domain.Crops.BatchStatus Status,
        DateOnly? RemovedDate,
        string? Notes);
}
