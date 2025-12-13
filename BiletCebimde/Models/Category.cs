
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Data Annotations için zorunlu


namespace BiletCebimde.Models // 
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        // Event sınıfı artık bu namespace içinde görülebilir.
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
} // 👈 BU SATIRI EKLEYİN (Kapanış küme parantezi)