using System.ComponentModel.DataAnnotations;

namespace BiletCebimde.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Etkinlik başlığı zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        [Display(Name = "Etkinlik Başlığı")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tarih zorunludur.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Etkinlik Tarihi")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Kontenjan zorunludur.")]
        [Range(1, 10000, ErrorMessage = "Kontenjan 1 ile 10000 arasında olmalıdır.")]
        [Display(Name = "Kontenjan")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Yer seçimi zorunludur.")]
        [Display(Name = "Yer")]
        public int VenueId { get; set; }

        // Dropdown listeler için
        public List<CategoryViewModel>? Categories { get; set; }
        public List<VenueViewModel>? Venues { get; set; }

        // Display için
        public string? CategoryName { get; set; }
        public string? VenueName { get; set; }
        public string? OrganizerName { get; set; }
        public int RegisteredCount { get; set; }
        public bool IsRegistered { get; set; }
        public bool CanRegister { get; set; }
    }
}

