using DataChangeTracker.Data;
using DataChangeTracker.Data.Models.Tracking;
using Microsoft.EntityFrameworkCore;

namespace DataChangeTracker.Service
{
    public class AuditService
    {
        private readonly AppDbContext _context;
        public AuditService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Audit>> GetAll(string entityName)
        {
            return await _context.Audits.Where(x => x.TableName == entityName).Include(x => x.Changes).ToListAsync();
        }
        public async Task<List<Audit>> GetAuditsAsync(string entityName, int entityId)
        {
            return await _context.Audits.Where(x => x.TableName == entityName && x.RecordId == entityId)
                .Include(x => x.ChangedBy).Include(x => x.Changes).OrderByDescending(x => x.Id).ToListAsync();
        }
        public async Task<List<AuditEntry>> GetChangesByAuditId(int auditId)
        {
            return await _context.AuditEntries.Where(x => x.AuditId == auditId).ToListAsync();
        }

        public async Task<List<int>> GetIdList(string entityName)
        {
            switch (entityName)
            {
                case "Products":
                    return await _context.Products.Select(x => x.Id).ToListAsync();
                case "Categories":
                default:
                    return await _context.Categories.Select(x => x.Id).ToListAsync();
            }
        }
    }
}
