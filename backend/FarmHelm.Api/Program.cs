using FarmHelm.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/health/database", async (FarmHelm.Infrastructure.Persistence.FarmHelmDbContext dbContext, CancellationToken cancellationToken) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        return canConnect
            ? Results.Ok(new { status = "ok", database = "reachable" })
            : Results.Json(new { status = "degraded", database = "unreachable" }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
    catch
    {
        return Results.Json(new { status = "degraded", database = "unreachable" }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

app.MapControllers();

app.Run();
