// Models/Event.cs

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity; // Organizer ilişkisi için IdentityUser kullanılacak

public class Event
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime Date { get; set; }

    [Required]
    [Range(1, 10000)] // Kontenjan 1'den az olamaz.
    public int Capacity { get; set; }

    // Kontenjan bilgisini tutacak:
    public int RegisteredCount { get; set; } = 0;


    // --- FOREIGN KEY İLİŞKİLERİ ---

    // 1. Organizatör İlişkisi (One-to-Many): Bir organizatör birden fazla etkinlik oluşturur.
    public string OrganizerId { get; set; }
    // IdentityUser sınıfını kullanıyorsanız, burayı da IdentityUser olarak ayarlayın
    public IdentityUser Organizer { get; set; }

    // 2. Kategori İlişkisi (One-to-Many)
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    // 3. Yer (Venue) İlişkisi (One-to-Many)
    public int VenueId { get; set; }
    public Venue Venue { get; set; }

    // 4. Navigation Property: Kayıt İlişkisi (Many-to-Many için)
    // Bir etkinliğe birden fazla kullanıcı kayıt olabilir.
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}