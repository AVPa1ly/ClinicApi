using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }

        public string? Use { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Family { get; set; }

        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        [MaxLength(7)]
        public string? Gender { get; set; }

        [Required]
        public DateTime BirthDate {get; set; }

        public bool? Active { get; set; }
    }
}
