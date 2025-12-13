using BiletCebimde.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq; // foreach döngüsü için gerekebilir

namespace BiletCebimde.Data
{
    // IdentityDbContext<Users>: Özel Users sınıfınızın yönetimi otomatik olarak sağlanır.
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- 1. Entity (Varlık) DbSet'leri ---
        // Users sınıfı için DbSet tanımlamasına GEREK YOKTUR!

        public DbSet<Category> Categories { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Registration> Registrations { get; set; }


        // --- 2. İlişkileri ve Anahtarları Yapılandırma ---
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // ❗ ZORUNLU: Identity tablolarını (Users, Roles, vs.) yapılandırmak için. 
            // Bu, 'Users' üzerinde anahtar tanımlanması hatasını önler.
            base.OnModelCreating(builder);

            // 1. Registration (Kayıt) Tablosunun Composite Key'ini tanımlama
            builder.Entity<Registration>()
                .HasKey(r => new { r.UserId, r.EventId });

            // 2. Registration ve Users İlişkisi (Many-to-Many - Kullanıcının Katıldığı Etkinlikler)
            builder.Entity<Registration>()
                .HasOne(r => r.User)
                // Users modelinde 'Registrations' koleksiyonunun olduğunu varsayıyoruz.
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.UserId)
                // Kısıtlama: Kullanıcıyı silmek için önce tüm kayıtları silmelisiniz.
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Event ve Organizatör (Users) İlişkisi (One-to-Many - Kullanıcının Düzenlediği Etkinlikler)
            builder.Entity<Event>()
                .HasOne(e => e.Organizer)
                // Users modelinde 'OrganizedEvents' koleksiyonunun olduğunu varsayıyoruz.
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                // Kısıtlama: Organizatörü silmek için önce düzenlediği tüm etkinlikleri silmelisiniz.
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Registration ve Event İlişkisi
            builder.Entity<Registration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId)
                // Mantıklı: Bir etkinlik silinirse, buna bağlı tüm kayıtlar da silinmeli.
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}