using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Models.Dto
{
    public class PatientDto
    {
        [Required]
        public NameDto Name { get; set; }

        [MaxLength(7)]
        public string? Gender { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public bool? Active { get; set; }
    }
}
