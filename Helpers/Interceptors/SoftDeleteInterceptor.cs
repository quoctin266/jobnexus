using JobNexus.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Threading;

namespace JobNexus.Helpers.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
        {
            DeleteEntity(eventData);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
        {
            DeleteEntity(eventData);

            return base.SavingChangesAsync(eventData, result, cancellationToken);   
        }

        public void DeleteEntity(DbContextEventData eventData)
        {
            var context = eventData.Context;

            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry is { State: EntityState.Deleted, Entity: ISoftDelete entity })
                {
                    entry.State = EntityState.Modified;
                    entity.IsDeleted = true;
                    entity.DeletedAt = DateTimeOffset.UtcNow;
                }
            }
        }
    }
}
