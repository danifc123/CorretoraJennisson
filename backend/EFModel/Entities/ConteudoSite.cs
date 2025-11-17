using System;
using System.ComponentModel.DataAnnotations;

namespace CorretoraJenissonLuckwuAPI.EFModel.Entities
{
  public class ConteudoSite
  {
    #region Key
    [Key]
    public int Id { get; set; }
    #endregion

    #region Properties
    [Required]
    [MaxLength(150)]
    public string? Chave { get; set; }

    [Required]
    [MaxLength(200)]
    public string? Titulo { get; set; }

    [MaxLength(400)]
    public string? Subtitulo { get; set; }

    [Required]
    public string? Descricao { get; set; }

    [MaxLength(500)]
    public string? ImagemUrl { get; set; }

    public int Ordem { get; set; } = 0;

    public bool Ativo { get; set; } = true;

    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    #endregion
  }
}

