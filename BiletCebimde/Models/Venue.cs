// Models/Venue.cs

using System.ComponentModel.DataAnnotations;

public class Venue
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; }

    [Required]
    public string Address { get; set; }

    // Navigation Property: Bir yerde birden fazla etkinlik düzenlenebilir.
    public ICollection<Event> Events { get; set; } = new List<Event>();
}