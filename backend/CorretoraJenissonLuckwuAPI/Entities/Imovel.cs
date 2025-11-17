using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorretoraJenissonLuckwuAPI.Entities
{
    public enum StatusImovel
{
    Disponivel,
    Vendido,
    Alugado
}

public enum TipoImovel
{
    Casa,
    Apartamento,
    Terreno
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

        public TipoImovel TipoImovel { get; set; }// Casa, Apartamento, Terreno
        [Required]
        public string Titulo { get; set; }

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
