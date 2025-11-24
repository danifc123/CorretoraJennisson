using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorretoraJenissonLuckwuAPI.Models.Entities
{
    public enum StatusImovel
    {
    Disponivel,
    Vendido,
    Alugado
}

    public class Imovel
    {
        #region Key
        [Key]
        public int Id { get; set; }
        #endregion

        #region Attributes
        [Required]
        public string Cidade { get; set; }
        
        [Required]
        public string Descricao { get; set; }

        [Required]
        public string Estado { get; set; }

        [Required]
        public string Endereco { get; set; }

        [Required]
        public decimal Preco { get; set; }

        [Required]
        public StatusImovel Status { get; set; }// Disponível, Vendido, Alugado

        [Required]
        public string TipoImovel { get; set; } = string.Empty;// Casa, Apartamento, Terreno ou definido pelo admin
        public string Titulo { get; set; }

        public bool Ativo { get; set; } = true;

        // Cômodos do imóvel
        public int? Salas { get; set; }
        public int? Cozinhas { get; set; }
        public int? Banheiros { get; set; }
        public int? Suites { get; set; }
        public int? Lavabos { get; set; }
        public int? GaragemDescoberta { get; set; }
        public int? GaragemCoberta { get; set; }

        #endregion

        #region Generated Data (Backend logic)
        public DateTime Created_at { get; set; } = DateTime.UtcNow;

        public DateTime Updated_at { get; set; } = DateTime.UtcNow;
        #endregion

        #region Navigation Properties
        public virtual ICollection<ImagemImovel> Imagens { get; set; } = new List<ImagemImovel>();
        public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
        #endregion
    }
}

