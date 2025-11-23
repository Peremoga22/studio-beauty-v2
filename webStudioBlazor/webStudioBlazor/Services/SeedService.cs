using Microsoft.EntityFrameworkCore;

using System.Diagnostics.Metrics;

using webStudioBlazor.BL;
using webStudioBlazor.Data;
using webStudioBlazor.EntityModels;

namespace webStudioBlazor.Services
{
    using Microsoft.EntityFrameworkCore;

    using Telegram.Bot.Types;

    using webStudioBlazor.ModelDTOs;

    public class SeedService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public SeedService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // ===== CREATE / UPDATE =====

        public async Task SaveSeedMasterAsync(Master master, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            if (master.Id == 0)
            {
                await db.Masters.AddAsync(master, ct);
            }
            else
            {                
                db.Masters.Update(master);
                               
                // var entity = await db.Masters.FindAsync(new object[] { master.Id }, ct);
                // if (entity is null) return;
                // entity.Name = master.Name;
            }

            await db.SaveChangesAsync(ct);
        }

        public async Task SaveSeedCategoryAsync(Category category, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            if (category.Id == 0)
            {
                await db.Categories.AddAsync(category, ct);
            }
            else
            {
                db.Categories.Update(category);
            }

            await db.SaveChangesAsync(ct);
        }

        public async Task SaveSeedPageTherapyAsync(PageTherapy pageTherapy, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            if (pageTherapy.Id == 0)
            {
                await db.PageTherapyies.AddAsync(pageTherapy, ct);
            }
            else
            {
                db.PageTherapyies.Update(pageTherapy);
            }

            await db.SaveChangesAsync(ct);
        }

        public async Task SaveCosmetologyTherapyCardAsync(TherapyCard therapyCard, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            if (therapyCard.Id == 0)
            {
                await db.TherapyCards.AddAsync(therapyCard, ct);
            }
            else
            {
                db.TherapyCards.Update(therapyCard);
            }

            await db.SaveChangesAsync(ct);
        }

        public async Task<int> SaveClientAsync(Appointment appointmentUser, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            if (appointmentUser.Id == 0)
            {
                await db.Appointments.AddAsync(appointmentUser, ct);
            }
            else
            {
                db.Appointments.Update(appointmentUser);
            }

            await db.SaveChangesAsync(ct);
            return appointmentUser.Id;
        }

        public async Task SaveClientServiceAsync(AppointmentService appointmentUser, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            if (appointmentUser.Id == 0)
            {
                await db.AppointmentServices.AddAsync(appointmentUser, ct);
            }
            else
            {
                db.AppointmentServices.Update(appointmentUser);
            }

            await db.SaveChangesAsync(ct);
        }

        // ===== EDIT (READ SINGLE) =====

        public async Task<Master?> EditMaster(int masterId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.Masters
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == masterId, ct);
        }

        public async Task<Category?> EditCategory(int categoryId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == categoryId, ct);
        }

        public async Task<PageTherapy?> EditPageTherapy(int pageTherapyId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.PageTherapyies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == pageTherapyId, ct);
        }

        public async Task<TherapyCard?> EditTherapyCard(int therapyCardId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.TherapyCards
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == therapyCardId, ct);
        }

        // ===== DELETE =====

        public async Task DeleteMasterAsync(int masterId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var entity = await db.Masters.SingleOrDefaultAsync(c => c.Id == masterId, ct);
            if (entity is null) return;
            db.Masters.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteCategoryAsync(int categoryId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var entity = await db.Categories.SingleOrDefaultAsync(c => c.Id == categoryId, ct);
            if (entity is null) return;
            db.Categories.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeletePageTherapyAsync(int pageTherapyId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var entity = await db.PageTherapyies.SingleOrDefaultAsync(c => c.Id == pageTherapyId, ct);
            if (entity is null) return;
            db.PageTherapyies.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteTherapyCardAsync(int cardId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var entity = await db.TherapyCards.SingleOrDefaultAsync(e => e.Id == cardId, ct);
            if (entity is null) return;
            db.TherapyCards.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        // ===== LISTS =====

        public async Task<List<Master>> GetAllMasterListAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.Masters
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync(ct);
        }

        public async Task<List<Category>> GetAllCategoryListAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.Categories
                .AsNoTracking()
                .Include(c => c.Masters)
                .OrderBy(x => x.Id)
                .ToListAsync(ct);
        }

        public async Task<List<PageTherapy>> GetAllPageTherapyListAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.PageTherapyies
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync(ct);
        }

        public async Task<List<TherapyCard>> GetAllTherapyCardListAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.TherapyCards
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync(ct);
        }

        public async Task<List<Appointment>> GetAllClientListAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.Appointments
                .AsNoTracking()
                .OrderByDescending(x => x.AppointmentDate)
                .ToListAsync(ct);
        }

        public async Task<Appointment?> GetClientAppointmentId(int clientId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.Appointments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == clientId, ct);
        }

        public async Task<Appointment?> GetClientAppointmentUserId(string clientId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.Appointments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId== clientId, ct);
        }

        public async Task<List<AppointmentService>> ListForCalendarAsync(
            bool onlyCompleted = false,
            CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var q = db.AppointmentServices
                .AsNoTracking()
                .Include(x => x.Appointment)
                .Include(x => x.Category)
                .Include(x => x.TherapyCard)
                .AsQueryable();

            if (onlyCompleted) q = q.Where(x => x.Appointment.IsCompleted);

            return await q
                .OrderBy(x => x.Appointment.AppointmentDate)
                .ThenBy(x => x.Appointment.SetHour)
                .ToListAsync(ct);
        }

        public async Task DeleteAdminClientCardAsync(int appointmentServiceId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.AppointmentServices
                .FirstOrDefaultAsync(x => x.Id == appointmentServiceId, ct);

            if (entity is null) return;

            db.AppointmentServices.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAppointmentAsync(int appointmentId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
                        
            var appointment = await db.Appointments              
                .FirstOrDefaultAsync(a => a.Id == appointmentId, ct);

            if (appointment is null)
                return;                      
           
            db.Appointments.Remove(appointment);
            await db.SaveChangesAsync(ct);
        }

        public async Task<List<AppointmentWithDetailsDto>> GetAppointmentsByUserIdAsync(string aspUserId,CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Appointments
                .AsNoTracking()
                .Where(a => a.UserId == aspUserId)
                .Include(a => a.TherapyCard)
                .Include(a => a.Master) 
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentWithDetailsDto
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    MasterId = a.MasterId,
                    CategoryId = a.CategoryId,
                    TherapyId = a.TherapyId,

                    ImageUrl = a.TherapyCard.ImagePath,
                    ServiceName = a.TherapyCard.TitleCard,
                    Description = a.TherapyCard.DescriptionCard,
                                        
                    MasterFullName = a.Master.FullName,
                    IsVideo = a.CategoryId == 2
                }).ToListAsync(ct);
        }

        public async Task<List<ClientOrderWithDetailsDto>> GetClientOrdersByUserIdAsync(string aspUserId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.ClientOrders
                 .AsNoTracking()
                 .Where(co => co.UserId == aspUserId)
                 .Include(co => co.Order)
                     .ThenInclude(o => o.Items)
                         .ThenInclude(oi => oi.Therapy)
                 .OrderByDescending(co => co.AppointmentDate)
                 .Select(co => new ClientOrderWithDetailsDto
                 {
                     Id = co.Id,
                     AppointmentDate = co.AppointmentDate,
                     FullName = co.ClientFirstName + " " + co.ClientLastName,
                     Phone = co.ClientPhone,
                     City = co.City,
                     Address = co.AddressNewPostOffice,
                     Price = co.Price,

                     ServiceName = co.Order.Items.Count > 0 && co.Order.Items.FirstOrDefault() != null && co.Order.Items.FirstOrDefault().Therapy != null
                         ? co.Order.Items.FirstOrDefault().Therapy.TitleCard
                         : string.Empty,
                     ImageUrl = co.Order.Items.Count > 0 && co.Order.Items.FirstOrDefault() != null && co.Order.Items.FirstOrDefault().Therapy != null
                         ? co.Order.Items.FirstOrDefault().Therapy.ImagePath
                         : string.Empty,
                     ServiceDescription = co.Order.Items.Count > 0 && co.Order.Items.FirstOrDefault() != null && co.Order.Items.FirstOrDefault().Therapy != null
                         ? co.Order.Items.FirstOrDefault().Therapy.DescriptionCard : string.Empty,
                 })
                 .ToListAsync(ct);
        }

        public async Task<List<Review>> GetReviewsForUserAsync(string userId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Reviews
                .AsNoTracking()
                .Include(r => r.TherapyCard)
                .Include(r => r.Master)
                .Include(r =>r.Appointment)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);
        }
        public async Task AddReviewAsync(Review review, CancellationToken ct = default)
        {            
            if (review == null)
                throw new ArgumentNullException(nameof(review));
                       
            if (review.CreatedAt == default)
                review.CreatedAt = DateTime.UtcNow;

            await using var db = await _dbFactory.CreateDbContextAsync(ct);                    
            db.Entry(review).State = EntityState.Added;

            await db.SaveChangesAsync(ct);
        }

    }
}
