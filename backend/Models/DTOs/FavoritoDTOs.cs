using System.ComponentModel.DataAnnotations;

namespace CorretoraJenissonLuckwuAPI.Models.DTOs
{
    public class CreateFavoritoRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ImovelId inválido")]
        public int ImovelId { get; set; }
    }
}

