using SmartAttend.Application.Interfaces;

namespace SmartAttend.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    // Change Task<int> to Task to match the Interface contract
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }
}