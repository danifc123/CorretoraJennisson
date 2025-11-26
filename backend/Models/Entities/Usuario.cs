using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorretoraJenissonLuckwuAPI.Models.Entities
{
  public class Usuario
  {
    #region Id
    [Key]
    public int Id { get; set; }
    #endregion


    #region Properties
    [Required]
    public string? Nome { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Senha { get; set; }

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

    [Phone]
    public string? Telefone { get; set; }
    #endregion

    #region Generated Data
    public DateTime Created_at { get; set; } = DateTime.UtcNow;

    public DateTime Updated_at { get; set; } = DateTime.UtcNow;
    #endregion

    #region Navigation Properties
    public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
    #endregion
  }
}

