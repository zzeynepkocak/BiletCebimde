// Models/Category.cs

using System.ComponentModel.DataAnnotations; // Data Annotations için zorunlu

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")] // Zorunlu alan kuralı
    [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")] // Uzunluk kuralı
    public string Name { get; set; }

    // Navigation Property: EF Core için gereklidir
    public ICollection<Event> Events { get; set; } = new List<Event>();
}