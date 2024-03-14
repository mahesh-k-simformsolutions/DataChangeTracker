using DataChangeTracker.Data;
using DataChangeTracker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DataChangeTracker.Service
{
    public class CategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAll()
        {
            return await _context.Categories.Include(x => x.Products).Where(x => x.IsDeleted == false).ToListAsync();
        }
        public async Task<Category> GetById(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<bool> Delete(int id)
        {
            var record = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (record != null)
            {
                record.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> Save(Category model)
        {
            if (model.Id == 0)
            {
                _context.Categories.Add(model);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                var record = await _context.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (record != null)
                {
                    record.Name = model.Name;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }
    }
}
