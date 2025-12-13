using System.ComponentModel.DataAnnotations;

namespace BiletCebimde.ViewModels
{
    public class EditEventViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Etkinlik başlığı zorunludur.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Başlık 3 ile 200 karakter arasında olmalıdır.")]
        [Display(Name = "Etkinlik Başlığı")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Açıklama 10 ile 2000 karakter arasında olmalıdır.")]
        [Display(Name = "Açıklama")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tarih zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Etkinlik Tarihi")]
        public DateTime Date { get; set; } = DateTime.Now.Date;

        [Required(ErrorMessage = "Saat zorunludur.")]
        [DataType(DataType.Time)]
        [Display(Name = "Etkinlik Saati")]
        public TimeSpan Time { get; set; } = DateTime.Now.TimeOfDay;

        [Required(ErrorMessage = "Kontenjan zorunludur.")]
        [Range(1, 10000, ErrorMessage = "Kontenjan 1 ile 10000 arasında olmalıdır.")]
        [Display(Name = "Kontenjan")]
        public int Capacity { get; set; } = 50;

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Yer seçimi zorunludur.")]
        [Display(Name = "Mekan")]
        public int VenueId { get; set; }

        // Dropdown listeler için
        public List<CategoryViewModel>? Categories { get; set; }
        public List<VenueViewModel>? Venues { get; set; }
    }
}

