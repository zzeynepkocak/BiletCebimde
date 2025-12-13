using BiletCebimde.Models; // Users, Category, Event vb. sınıflarınızın bulunduğu namespace
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata; // DeleteBehavior.Restrict için gerekebilir

namespace BiletCebimde.Data
{
    // IdentityDbContext<Users> kullandığınız için Users sınıfı otomatik yönetilir.
    // Bu nedenle AppDbContext içinde Users için DbSet tanımlaması YAPILMAZ!
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        // --- 1. Yeni Entity'ler (Varlıklar) için DbSet'ler ---

        // DİKKAT: Users sınıfı için burada DbSet yoktur!

        public DbSet<Category> Categories { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }

        // Many-to-Many bağlantı tablosu
        public DbSet<Registration> Registrations { get; set; }


        // --- 2. İlişkileri ve Anahtarları Yapılandırma ---

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Identity tablolarını doğru yapılandırmak için ZORUNLUDUR!
            base.OnModelCreating(builder);

            // 1. Registration (Kayıt) Tablosunun Composite Key'ini tanımlama
            builder.Entity<Registration>()
                .HasKey(r => new { r.UserId, r.EventId });

            // 2. Registration ve Users (Kullanıcı) İlişkisi (Restrict ile silme kısıtlaması)
            builder.Entity<Registration>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Event ve Organizatör (Users) İlişkisi (Restrict ile silme kısıtlaması)
            builder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany()
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Registration ve Event İlişkisi
            builder.Entity<Registration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId);

            // SQLite veya PostgreSQL kullanıyorsanız, Cascade Delete sorunlarını önlemek için 
            // varsayılan kısıtlamaları kaldırmak gerekebilir.
            // Örneğin:
            // foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            // {
            //     relationship.DeleteBehavior = DeleteBehavior.Restrict;
            // }
        }
    }
}