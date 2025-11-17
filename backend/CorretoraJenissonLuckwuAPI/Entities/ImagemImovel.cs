using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorretoraJenissonLuckwuAPI.Entities
{
    public class ImagemImovel
    {
        #region Key
        [Key]
        public int Id { get; set; }
        #endregion

        #region Properties
        [ForeignKey("Imovel")]
        public int Imovel_Id { get; set; }
        
        [Required]
        public string Url { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Imovel Imovel { get; set; }
        #endregion
    }
}
