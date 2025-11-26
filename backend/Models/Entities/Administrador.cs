using System.ComponentModel.DataAnnotations;

namespace CorretoraJenissonLuckwuAPI.Models.Entities
{
    public class Administrador
    {
        #region Key
        [Key]
        public int Id { get; set; }
        #endregion

        #region Properties
        [Required]
        public required string Nome { get; set; }

        [Phone]
        public string? Telefone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        /// <summary>
        /// Token para redefinição de senha (fluxo "Esqueci minha senha")
        /// </summary>
        public string? ResetPasswordToken { get; set; }

        /// <summary>
        /// Data/hora (UTC) de expiração do token de redefinição de senha
        /// </summary>
        public DateTime? ResetPasswordTokenExpiresAt { get; set; }

        // Stream_user_id comentado - será implementado no futuro para chatbot
        // [Required]
        public string? Stream_user_id { get; set; }

        public string? ID_PFPJ { get; set; }

        #endregion

        #region Generated Data (Backend logic)
        public DateTime Created_at { get; set; } = DateTime.UtcNow;

        public DateTime Updated_at { get; set; } = DateTime.UtcNow;
        #endregion
    }
}
