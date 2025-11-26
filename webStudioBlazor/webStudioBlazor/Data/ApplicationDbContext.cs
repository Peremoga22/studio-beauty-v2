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
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<ClientOrders> ClientOrders { get; set; }
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<GiftCertificate> GiftCertificates => Set<GiftCertificate>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MasterConfig());
            modelBuilder.ApplyConfiguration(new CategoryConfig());
            modelBuilder.ApplyConfiguration(new PageTherapyConfig());
            modelBuilder.ApplyConfiguration(new TherapyCardConfig());
            modelBuilder.ApplyConfiguration(new AppointmentConfig());
            modelBuilder.ApplyConfiguration(new AppointmentServiceConfig());
            modelBuilder.ApplyConfiguration(new CartConfig());
            modelBuilder.ApplyConfiguration(new CartItemConfig());
            modelBuilder.ApplyConfiguration(new OrderConfig());
            modelBuilder.ApplyConfiguration(new OrderItemConfig());
            modelBuilder.ApplyConfiguration(new ClientOrdersConfig());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new GiftCertificateConfig());
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
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            b.Property(x => x.IsCompleted)
                .HasDefaultValue(false);

            b.Property(x => x.UserId)             
               .HasColumnType("nvarchar(450)");

            b.HasOne(x => x.User)
                .WithMany() //  .WithMany(u => u.Appointments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

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

    #region Cart
    public class CartConfig : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> b)
        {
            b.ToTable("Carts");

            b.HasKey(x => x.Id);

            b.Property(x => x.SessionKey)
             .IsRequired()
             .HasMaxLength(128);

            b.Property(x => x.CreatedAt)
             .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()");
                        
            b.Property(x => x.IsActive).IsRequired();                        

            b.HasMany(x => x.Items)
             .WithOne(i => i.Cart)
             .HasForeignKey(i => i.CartId)
             .OnDelete(DeleteBehavior.Cascade);
                       
            b.HasIndex(x => x.SessionKey)
             .HasDatabaseName("IX_Carts_Session_Active")
             .HasFilter("[IsActive] = 1")   // лишай, якщо лише SQL Server
             .IsUnique();
        }
    }

    public class CartItemConfig : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> b)
        {
            b.ToTable("CartItems");

            b.HasKey(x => x.Id);

            b.Property(x => x.Quantity)
                .HasDefaultValue(1);

            b.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");
                       
            b.HasIndex(x => new { x.CartId, x.TherapyId })
                .IsUnique();
           
            b.HasOne(x => x.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Cascade);
                        
            b.HasOne(x => x.Therapy)
                .WithMany() // або .WithMany(t => t.CartItems)
                .HasForeignKey(x => x.TherapyId)
                .OnDelete(DeleteBehavior.Restrict);
                      
            b.Ignore(x => x.TotalPrice);
        }
    }
    #endregion

    #region Order
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> b)
        {
            b.ToTable("Orders");

            b.HasKey(x => x.Id);

            b.Property(x => x.OrderDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            b.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.PaymentStatus)
                .HasMaxLength(32)
                .HasDefaultValue("Pending");

            b.Property(x => x.OrderStatus)
                .HasMaxLength(32)
                .HasDefaultValue("New");

            // Конкурентний токен
            b.Property<byte[]>("RowVersion")
                .IsRowVersion();

            // Order (1) → Items (many)
            b.HasMany(x => x.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                       
            b.HasOne(o => o.ClientOrder)
                .WithOne(co => co.Order)
                .HasForeignKey<ClientOrders>(co => co.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.SessionKey)
                .HasMaxLength(128);

            b.HasIndex(x => x.SessionKey);
        }
    }

    public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> b)
        {
            b.ToTable("OrderItems");

            b.HasKey(x => x.Id);

            b.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.Quantity)
                .HasDefaultValue(1);
                       
            b.HasIndex(x => new { x.OrderId, x.TherapyId })
                .IsUnique();
                       
            b.HasOne(x => x.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                       
            b.HasOne(x => x.Therapy)
                .WithMany() // або .WithMany(t => t.OrderItems)
                .HasForeignKey(x => x.TherapyId)
                .OnDelete(DeleteBehavior.Restrict);
            b.Ignore(x => x.TotalPrice);

            b.Property(x => x.IsShownInOrder)
          .HasDefaultValue(true);
        }
    }
    #endregion
    public class ClientOrdersConfig : IEntityTypeConfiguration<ClientOrders>
    {
        public void Configure(EntityTypeBuilder<ClientOrders> b)
        {
            b.ToTable("ClientOrders");                      
            b.HasKey(x => x.Id);
            
            b.Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.ClientFirstName)
                .HasMaxLength(64)
                .IsRequired();

            b.Property(x => x.ClientLastName)
                .HasMaxLength(64)
                .IsRequired();

            b.Property(x => x.ClientPhone)
                .HasMaxLength(20)
                .IsRequired();

            b.Property(x => x.City)
                .HasMaxLength(64)
                .IsRequired();

            b.Property(x => x.AddressNewPostOffice)
                .HasMaxLength(128)
                .IsRequired();
            b.Property(x => x.UserId)                
                 .HasColumnType("nvarchar(450)");

            b.HasOne(x => x.User)
                .WithMany() //  .WithMany(u => u.Appointments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.AppointmentDate)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v))
                .HasColumnType("date");

            b.HasOne(x => x.Order)
             .WithOne(o => o.ClientOrder) 
             .HasForeignKey<ClientOrders>(x => x.OrderId)
             .OnDelete(DeleteBehavior.Cascade);
                        
            b.HasIndex(x => x.OrderId).IsUnique();
                       
        }
    }

    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> b)
        {
            b.ToTable("Reviews");

            b.HasKey(x => x.Id);

            b.Property(x => x.Rating)
                .IsRequired();

            b.Property(x => x.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            b.Property(x => x.CreatedAt)
                .IsRequired();

            // 🔗 Review -> Appointment (1:N)
            b.HasOne(x => x.Appointment)
                .WithMany(a => a.Reviews)
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

           b.HasOne(x => x.Category)
              .WithMany(c => c.Reviews)
              .HasForeignKey(x => x.CategoryId)
              .OnDelete(DeleteBehavior.NoAction);

            // 🔗 Review -> TherapyCard (1:N)
            b.HasOne(x => x.TherapyCard)
                .WithMany()                // якщо в TherapyCard немає ICollection<Review>
                .HasForeignKey(x => x.TherapyId)
                .OnDelete(DeleteBehavior.NoAction); // можеш поставити Restrict/NoAction

            // 🔗 Review -> Master (1:N)
            b.HasOne(x => x.Master)
                .WithMany()                // або .WithMany(m => m.Reviews), якщо додаси навігацію
                .HasForeignKey(x => x.MasterId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔗 Review -> User (опційно)
            b.HasOne(x => x.User)
                .WithMany()                // можна зробити User.Reviews, якщо треба
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class GiftCertificateConfig : IEntityTypeConfiguration<GiftCertificate>
    {
        public void Configure(EntityTypeBuilder<GiftCertificate> b)
        {
            b.ToTable("GiftCertificates");

            b.HasKey(x => x.Id);

            b.Property(x => x.RecipientName)
                .IsRequired()
                .HasMaxLength(100);

            b.Property(x => x.Amount)
                .IsRequired();

            b.Property(x => x.CreatedAt)
                .IsRequired();                       

            b.Property(x => x.Message)
                .HasMaxLength(500);
                      
            b.Property(x => x.IsApproved)
                .IsRequired()
                .HasDefaultValue(false);
                    
            b.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450); 

            b.HasOne(x => x.User)
                .WithMany() // або .WithMany(u => u.GiftCertificates), 
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
