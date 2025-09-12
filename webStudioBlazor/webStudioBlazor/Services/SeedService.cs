using Microsoft.EntityFrameworkCore;

using System.Diagnostics.Metrics;

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

        public async Task SaveSeedMasterAsync(Master master)
        {
            if (master.Id == 0)
            {
                var result = _db.Masters.Add(master);
                if (result.State == EntityState.Added)
                {
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                var masterInDbEntuty = _db.Masters.Find(master.Id);
                if (masterInDbEntuty != null)
                {
                    _db.Masters.Update(master);
                    await _db.SaveChangesAsync();
                }                               
            }
        }

        public async Task SaveSeedCategoryAsync(Category category)
        {
            if (category.Id == 0)
            {               
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
            }
        }


        public async Task SaveSeedPageTherapyAsync(PageTherapy pageTherapy)
        { 
            if (pageTherapy.Id == 0)
            {               
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
            }
        }

        public async Task SaveCosmetologyTherapyCard(TherapyCard therapyCard)
        {
            if (therapyCard.Id == 0)
            {
                var result = _db.TherapyCards.Add(therapyCard);
                if (result.State == EntityState.Added)
                {
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var therapyCardInDbEntuty = _db.TherapyCards.Find(therapyCard.Id);
                    if (therapyCardInDbEntuty != null)
                    {
                        _db.TherapyCards.Update(therapyCard);
                        await _db.SaveChangesAsync();
                    }
                }
            }
        }

        public int SaveClient(Appointment appointmentUser)
        {

            if (appointmentUser.Id == 0)
            {
                var result = _db.Appointments.Add(appointmentUser);
                if (result.State == EntityState.Added)
                {
                    _db.Appointments.Add(appointmentUser);
                    _db.SaveChanges();
                }
            }
            else
            {
                var categoryInDbEntuty = _db.Appointments.Find(appointmentUser.Id);
                if (categoryInDbEntuty != null)
                {
                    _db.Appointments.Update(appointmentUser);
                    _db.SaveChanges();
                }
            }

            return appointmentUser.Id;
        }

        public void SaveClientService(AppointmentService appointmentUser)
        {

            if (appointmentUser.Id == 0)
            {
                var result = _db.AppointmentServices.Add(appointmentUser);
                if (result.State == EntityState.Added)
                {
                    _db.AppointmentServices.Add(appointmentUser);
                    _db.SaveChanges();
                }
            }
            else
            {
                var categoryInDbEntuty = _db.AppointmentServices.Find(appointmentUser.Id);
                if (categoryInDbEntuty != null)
                {
                    _db.AppointmentServices.Update(appointmentUser);
                    _db.SaveChanges();
                }
            }                      
        }


        public async Task<Master> EditMaster(int masterId)
        {
            var result = await _db.Masters.FirstOrDefaultAsync(c => c.Id == masterId);
            return result;
        }

        public async Task<Category> EditCategory(int categoryId)
        {
            var result = await _db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            return result;
        }

        public async Task<PageTherapy> EditPageTherapy(int pageTherapyId)
        {
            var result = await _db.PageTherapyies.FirstOrDefaultAsync(c => c.Id == pageTherapyId);
            return result;
        }

        public async Task<TherapyCard> EditTherapyCard(int therapyCardId)
        {
            var result = _db.TherapyCards.FirstOrDefault(e => e.Id == therapyCardId);
            return await Task.FromResult(result);
        }

        public async Task DeleteMasterAsync(int masterId)
        {
            var result = await _db.Masters.SingleOrDefaultAsync(c => c.Id == masterId);
            if (result == null)
            {
                return;
            }

            _db.Masters.Remove(result);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var result = await _db.Categories.SingleOrDefaultAsync(c => c.Id == categoryId);
            if (result == null)
            {
                return;
            }

            _db.Categories.Remove(result);
            await _db.SaveChangesAsync();
        }

        public async Task DeletePageTherapyAsync(int pageTherapyId)
        {
            var result = await _db.PageTherapyies.SingleOrDefaultAsync(c => c.Id == pageTherapyId);
            if (result == null)
            {
                return;
            }

            _db.PageTherapyies.Remove(result);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteTherapyCardAsync(int cardId)
        {
            var result = await _db.TherapyCards.SingleOrDefaultAsync(e => e.Id == cardId);
            if (result == null)
            {
                return;
            }

            _db.TherapyCards.Remove(result);
            await _db.SaveChangesAsync();
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

        public async Task<List<TherapyCard>> GetAllTherapyCardListAsync()
        {
            var result = await _db.TherapyCards.ToListAsync();
            return result;
        }

        public async Task<List<Appointment>> GetAllClientListAsync()
        {
            var result = await _db.Appointments.ToListAsync();
            return result;
        }

        public async Task<Appointment> GetClientAppointmentId(int clientId)
        {
            var result = await _db.Appointments.FirstOrDefaultAsync(c => c.Id == clientId);
            return result;
        }
    }
}
