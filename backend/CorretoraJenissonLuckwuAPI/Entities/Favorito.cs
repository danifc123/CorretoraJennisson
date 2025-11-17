using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorretoraJenissonLuckwuAPI.Entities
{
    public class Favorito
    {
        #region Id
        [Key]
        public int Id { get; set; }
        #endregion

        #region Properties
        [ForeignKey("Usuario")]
        public int Usuario_Id { get; set; }

        [ForeignKey("Imovel")]
        public int Imovel_Id { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Usuario Usuario { get; set; }
        public virtual Imovel Imovel { get; set; }
        #endregion
    }
}
