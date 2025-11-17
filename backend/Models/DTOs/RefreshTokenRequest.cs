using System.ComponentModel.DataAnnotations;

namespace CorretoraJenissonLuckwuAPI.Models.DTOs
{
    public class RefreshTokenRequest
    {
        #region Properties
        [Required]
        public string RefreshToken { get; set; }
        #endregion
    }
}

