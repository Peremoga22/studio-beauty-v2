using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using webStudioBlazor.EntityModels;

namespace webStudioBlazor.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Master> Masters => Set<Master>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<PageTherapy> PageTherapyies => Set<PageTherapy>();
        public DbSet<TherapyCard> TherapyCards => Set<TherapyCard>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<AppointmentService> AppointmentServices => Set<AppointmentService>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MasterConfig());
            modelBuilder.ApplyConfiguration(new CategoryConfig());
            modelBuilder.ApplyConfiguration(new PageTherapyConfig());
            modelBuilder.ApplyConfiguration(new TherapyCardConfig());
            modelBuilder.ApplyConfiguration(new AppointmentConfig());
            modelBuilder.ApplyConfiguration(new AppointmentServiceConfig());
        }
    }
    

    public class MasterConfig : IEntityTypeConfiguration<Master>
    {
        public void Configure(EntityTypeBuilder<Master> b)
        {
            b.ToTable("Masters");

            b.HasKey(x => x.Id);

            b.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(32);
                       
            b.HasMany(x => x.Categories)
             .WithOne(x => x.Masters) 
             .HasForeignKey(x => x.MasterId)
             .OnDelete(DeleteBehavior.Restrict); 
        }
    }

    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> b)
        {
            b.ToTable("Categories");

            b.HasKey(x => x.Id);

            b.Property(x => x.NameCategory)
                .IsRequired()
                .HasMaxLength(100);

            b.Property(x => x.MasterId)
                .IsRequired();

            // many (Category) : 1 (Master)
            b.HasOne(x => x.Masters)
             .WithMany(x => x.Categories)
             .HasForeignKey(x => x.MasterId)
             .OnDelete(DeleteBehavior.Restrict);

            // 1 (Category) : many (PageTherapy)
            b.HasMany(x => x.TherapyCards)
             .WithOne(x => x.Categories) 
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Cascade);
                       
            b.HasIndex(x => new { x.MasterId, x.NameCategory })
             .IsUnique();
        }
    }

    public class PageTherapyConfig : IEntityTypeConfiguration<PageTherapy>
    {
        public void Configure(EntityTypeBuilder<PageTherapy> b)
        {
            b.ToTable("PageTherapies");

            b.HasKey(x => x.Id);

            b.Property(x => x.TitlePage)
                .IsRequired()
                .HasMaxLength(160);

            b.Property(x => x.DescriptionPage)
                .IsRequired()
                .HasMaxLength(4000);

            b.Property(x => x.CategoryId)
                .IsRequired();

            // many (PageTherapy) : 1 (Category)
            b.HasOne(x => x.Categories)
             .WithMany(x => x.PageTherapy)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class TherapyCardConfig : IEntityTypeConfiguration<TherapyCard>
    {
        public void Configure(EntityTypeBuilder<TherapyCard> b)
        {
            b.ToTable("TherapyCards");

            b.HasKey(x => x.Id);

            b.Property(x => x.TitleCard)
                .IsRequired()
                .HasMaxLength(160);

            b.Property(x => x.DescriptionCard)
                .IsRequired()
                .HasMaxLength(4000);

            b.Property(x => x.ImagePath)
                .HasMaxLength(500);

            b.Property(x => x.Price)
                .IsRequired()
                .HasMaxLength(50);

            b.Property(x => x.CategoryId)
                .IsRequired();

            // many (TherapyCard) : 1 (Category)
            b.HasOne(x => x.Categories)
             .WithMany(x => x.TherapyCards)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class AppointmentConfig : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> b)
        {
            b.ToTable("Appointments");

            b.HasKey(x => x.Id);

            b.Property(x => x.ClientName)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.ClientPhone)
                .IsRequired()
                .HasMaxLength(32);

            b.Property(x => x.AppointmentDate)
                .IsRequired();

            b.Property(x => x.SetHour)
                .HasConversion<TimeSpan>(
                    v => v.ToTimeSpan(),
                    v => TimeOnly.FromTimeSpan(v))
                .HasColumnType("time")
                .IsRequired();

            b.Property(x => x.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            b.Property(x => x.IsCompleted)
                .HasDefaultValue(false);

            // many (Appointment) : 1 (Master)
            b.HasOne(x => x.Master)
                .WithMany()
                .HasForeignKey(x => x.MasterId)
                .OnDelete(DeleteBehavior.Restrict);

            // many (Appointment) : 1 (Category)
            b.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // many (Appointment) : 1 (TherapyCard)
            b.HasOne(x => x.TherapyCard)
                .WithMany()
                .HasForeignKey(x => x.TherapyId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1 (Appointment) : many (AppointmentService)
            b.HasMany(x => x.AppointmentServices)
                .WithOne(x => x.Appointment)
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class AppointmentServiceConfig : IEntityTypeConfiguration<AppointmentService>
    {
        public void Configure(EntityTypeBuilder<AppointmentService> b)
        {
            b.ToTable("AppointmentServices");

            b.HasKey(x => x.Id);

            b.Property(x => x.AppointmentId).IsRequired();
            b.Property(x => x.CategoryId).IsRequired();
            b.Property(x => x.TherapyId).IsRequired();

            // many (AppointmentService) : 1 (Appointment)
            b.HasOne(x => x.Appointment)
                .WithMany(x => x.AppointmentServices)
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // many (AppointmentService) : 1 (Category)
            b.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // many (AppointmentService) : 1 (TherapyCard)
            b.HasOne(x => x.TherapyCard)
                .WithMany()
                .HasForeignKey(x => x.TherapyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
