using DataChangeTracker.Data.Models;
using DataChangeTracker.Data.Models.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataChangeTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AuditEntry> AuditEntries { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<User> Users { get; set; }

        private List<EntityAuditInformation> BeforeSaveChanges()
        {
            List<EntityAuditInformation> entityAuditInformation = new();

            foreach (EntityEntry entityEntry in ChangeTracker.BaseEntityEntries())
            {
                dynamic entity = entityEntry.Entity;
                bool isAdd = entityEntry.State == EntityState.Added;
                List<AuditEntry> changes = new();
                foreach (PropertyEntry property in entityEntry.Properties)
                {
                    if ((isAdd && property.CurrentValue != null) || (property.IsModified && !Object.Equals(property.CurrentValue, property.OriginalValue)))
                    {
                        if (property.Metadata.Name != "Id") // Do not track primary key values (never going to change)
                        {
                            changes.Add(new AuditEntry()
                            {
                                FieldName = property.Metadata.Name,
                                NewValue = property.CurrentValue?.ToString(),
                                OldValue = isAdd ? null : property.OriginalValue?.ToString()
                            });
                        }
                    }
                }
                PropertyEntry? IsDeletedPropertyEntry = entityEntry.Properties.FirstOrDefault(x => x.Metadata.Name == nameof(entity.IsDeleted));
                entityAuditInformation.Add(new EntityAuditInformation()
                {
                    Entity = entity,
                    TableName = entityEntry.Metadata?.GetTableName() ?? entity.GetType().Name,
                    State = entityEntry.State,
                    IsDeleteChanged = IsDeletedPropertyEntry != null && !object.Equals(IsDeletedPropertyEntry.CurrentValue, IsDeletedPropertyEntry.OriginalValue),
                    Changes = changes
                });
            }
            return entityAuditInformation;
        }
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entityAuditInformation = BeforeSaveChanges();
            int returnValue = 0;
            var userId = await Users.Select(x => x.Id).FirstOrDefaultAsync();
            returnValue = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            //iff all changes are saved then only create audit
            if (returnValue > 0)
            {
                foreach (EntityAuditInformation item in entityAuditInformation)
                {
                    dynamic entity = item.Entity;
                    List<AuditEntry> changes = item.Changes;
                    if (changes != null && changes.Any())
                    {
                        Audit audit = new()
                        {
                            TableName = item.TableName,
                            RecordId = entity.Id,
                            ChangeDate = DateTime.Now,
                            Operation = item.OperationType,
                            Changes = changes,
                            ChangedById = userId // LoggedIn user Id
                        };
                        _ = await AddAsync(audit, cancellationToken);
                    }
                }
                //Save audit data
                await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            return returnValue;
        }
    }

    public static class Ext
    {
        public static IEnumerable<EntityEntry> BaseEntityEntries(this ChangeTracker changeTracker, bool ignoreUnChanged = true)
        {
            IEnumerable<EntityEntry> lst = changeTracker.Entries();
            return ignoreUnChanged ? lst.Where(x => x.State != EntityState.Unchanged) : lst;
        }
    }
    public class EntityAuditInformation
    {
        public dynamic Entity { get; set; }
        public string TableName { get; set; }

        public EntityState State { get; set; }

        public List<AuditEntry> Changes { get; set; }

        public bool IsDeleteChanged { get; set; }

        public string OperationType
        {
            get
            {
                switch (State)
                {
                    case EntityState.Added:
                        return "Create";
                    case EntityState.Modified:
                        string deleteOrRestore = Entity.IsDeleted ? "Delete" : "Restore";
                        return IsDeleteChanged ? deleteOrRestore : "Update";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
