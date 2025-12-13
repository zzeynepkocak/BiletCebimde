
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // ICollection ve List için bu using satırı da gerekebilir.
// using BiletCebimde.Models; // Bu satırı kaldırabiliriz/bırakabiliriz, ancak sınıfı namespace içine almak daha önemlidir.

namespace BiletCebimde.Models // 👈 BU SATIRI EKLEYİN
{
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
} // 👈 BU SATIRI EKLEYİN (Kapanış küme parantezi)