using System.ComponentModel.DataAnnotations;

namespace CorretoraJenissonLuckwuAPI.Models.DTOs
{
    public class LoginRequest
    {
        #region Properties
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }
        #endregion
    }
}

