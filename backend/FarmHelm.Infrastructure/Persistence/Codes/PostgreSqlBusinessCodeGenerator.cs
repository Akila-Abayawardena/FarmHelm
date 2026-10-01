using System.Data;
using System.Globalization;
using FarmHelm.Application.Abstractions.Codes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FarmHelm.Infrastructure.Persistence.Codes;

public sealed class PostgreSqlBusinessCodeGenerator(FarmHelmDbContext context) : IBusinessCodeGenerator
{
    public async Task<string> GenerateAsync(BusinessCodeType codeType, CancellationToken cancellationToken = default)
    {
        var definition = BusinessCodeMetadata.Get(codeType);
        var connection = context.Database.GetDbConnection();
        var openedConnection = connection.State != ConnectionState.Open;

        if (openedConnection)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            // The sequence identifier originates only from the exhaustive metadata switch above.
            command.CommandText = $"SELECT nextval('{definition.SequenceName}'::regclass);";
            command.Transaction = context.Database.CurrentTransaction?.GetDbTransaction();

            var value = await command.ExecuteScalarAsync(cancellationToken);
            var sequenceValue = Convert.ToInt64(value, CultureInfo.InvariantCulture);
            return definition.Format(sequenceValue);
        }
        finally
        {
            if (openedConnection)
            {
                await connection.CloseAsync();
            }
        }
    }
}
