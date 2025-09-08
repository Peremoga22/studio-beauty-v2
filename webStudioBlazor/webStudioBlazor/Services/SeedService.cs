using Microsoft.EntityFrameworkCore;

using webStudioBlazor.Data;
using webStudioBlazor.EntityModels;

namespace webStudioBlazor.Services
{
    public class SeedService
    {
        private readonly ApplicationDbContext _db;

        public SeedService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddSeedAsync(Master master, Category category, PageTherapy pageTherapy)
        {
            if (master.Id == 0)
            {
                var result = _db.Masters.Add(master);
                if (result.State == EntityState.Added)
                {
                  await  _db.SaveChangesAsync();
                }
            }
            else 
            {
                var masterInDbEntuty = _db.Masters.Find(category.Id);
                if (masterInDbEntuty != null)
                {
                    _db.Masters.Update(master);
                    await _db.SaveChangesAsync();
                }

                await _db.SaveChangesAsync();
            }

            if (category.Id == 0)
            {
                category.MasterId = master.Id;
                var result = _db.Categories.Add(category);
                if (result.State == EntityState.Added)
                {
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                var categoryInDbEntuty = _db.Categories.Find(category.Id);
                if (categoryInDbEntuty != null)
                {
                    _db.Categories.Update(category);
                    await _db.SaveChangesAsync();
                }

                await _db.SaveChangesAsync();
            }

            if (pageTherapy.Id == 0)
            {
                pageTherapy.CategoryId = category.Id;
                var result = _db.PageTherapyies.Add(pageTherapy);
                if (result.State == EntityState.Added)
                {
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                var pageTherapyInDbEntuty = _db.PageTherapyies.Find(pageTherapy.Id);
                if (pageTherapyInDbEntuty != null)
                {
                    _db.PageTherapyies.Update(pageTherapy);
                    await _db.SaveChangesAsync();
                }

                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Master>> GetAllMasterListAsync()
        {
            var result = await _db.Masters.ToListAsync();
            return result;
        }

        public async Task<List<Category>> GetAllCategoryListAsync()
        {
            var result = await _db.Categories.ToListAsync();
            return result;
        }

        public async Task<List<PageTherapy>> GetAllPageTherapyListAsync()
        {
            var result = await _db.PageTherapyies.ToListAsync();
            return result;
        }
    }
}
