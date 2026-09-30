using Microsoft.EntityFrameworkCore;

namespace FarmHelm.Infrastructure.Persistence;

public sealed class FarmHelmDbContext(DbContextOptions<FarmHelmDbContext> options) : DbContext(options)
{
}
