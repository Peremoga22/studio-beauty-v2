﻿using Microsoft.EntityFrameworkCore;

using System.Diagnostics.Metrics;

using webStudioBlazor.BL;
using webStudioBlazor.Data;
using webStudioBlazor.EntityModels;

namespace webStudioBlazor.Services
{
    using Microsoft.EntityFrameworkCore;

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
    }

}
