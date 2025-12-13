

using System;
// Users, Event ve diğer modellerinizin bulunduğu namespace
using BiletCebimde.Models; 

namespace BiletCebimde.Models
{
    // Registration (Kayıt) varlığı, Many-to-Many ilişkisini temsil eder.
    public class Registration
    {
        // AppDbContext'te bu ikili (UserId, EventId) Composite Key olarak tanımlanmıştır.

        // Foreign Key 1: Kullanıcı
        public string UserId { get; set; }

        // Navigasyon Özelliği: IdentityUser yerine özel Users sınıfınızı kullanıyoruz.
        public Users User { get; set; }

        // Foreign Key 2: Etkinlik
        public int EventId { get; set; }

        // Navigasyon Özelliği
        public Event Event { get; set; }

        // Kayıt Tarihi (Varsayılan olarak nesne oluşturulduğu anı alır)
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}