// Models/Registration.cs

using Microsoft.AspNetCore.Identity;

public class Registration
{
    // Composite Primary Key için anahtar alanları kullanacağız.

    // Foreign Key 1: Kullanıcı
    public string UserId { get; set; }
    public IdentityUser User { get; set; } // Hangi kullanıcının kayıt olduğunu tutar.

    // Foreign Key 2: Etkinlik
    public int EventId { get; set; }
    public Event Event { get; set; } // Hangi etkinliğe kayıt olduğunu tutar.

    public DateTime RegistrationDate { get; set; } = DateTime.Now;
}