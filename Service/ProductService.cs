using DataChangeTracker.Data;
using DataChangeTracker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DataChangeTracker.Service
{
    public class ProductService
    {
        private readonly AppDbContext _context;
        public ProductService(AppDbContext context)
        {
            _context = context;       
        }
        public async Task<List<Product>> GetAll()
        {
            return await _context.Products.Include(x => x.Category).Where(x => x.IsDeleted == false).ToListAsync();
        }
        public async Task<Product> GetById(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<bool> Delete(int id)
        {
            var record = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (record != null)
            {
                record.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> Save(Product model)
        {
            if(model.Id == 0)
            {
                _context.Products.Add(model);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                var record = await _context.Products.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (record != null)
                {
                    record.Name = model.Name;
                    record.Description = model.Description;
                    record.Price = model.Price;
                    record.CategoryId = model.CategoryId;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }
    }
}
