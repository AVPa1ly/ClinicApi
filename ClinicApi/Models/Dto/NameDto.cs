using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Models.Dto
{
    public class NameDto
    {
        [Required]
        public Guid Id { get; set; }

        public string? Use { get; set; }

        [Required]
        public string? Family { get; set; }

        public string[] Given { get; set; } = Array.Empty<string>();
    }
}
