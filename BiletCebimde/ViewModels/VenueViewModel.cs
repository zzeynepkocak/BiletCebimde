using System.ComponentModel.DataAnnotations;

namespace BiletCebimde.ViewModels
{
    public class VenueViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yer adı zorunludur.")]
        [StringLength(150, ErrorMessage = "Yer adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Yer Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres zorunludur.")]
        [Display(Name = "Adres")]
        public string Address { get; set; } = string.Empty;

        public int EventCount { get; set; }
    }
}

