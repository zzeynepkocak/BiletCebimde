using Microsoft.AspNetCore.Identity;
using System.Collections.Generic; // ICollection kullanmak için ekleyin

namespace BiletCebimde.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }

        // 1. Kullanıcının katıldığı etkinliklerin kayıtları (Registration ile ilişki)
        // Hata mesajınızın bahsettiği kısım burasıydı.
        public ICollection<Registration> Registrations { get; set; }

        // 2. Kullanıcının düzenlediği etkinlikler (Event ile ilişki)
        public ICollection<Event> OrganizedEvents { get; set; }
    }
}
