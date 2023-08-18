using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdedonhaMVC.Models
{
    [Table("Categoria")]
    public class Categoria
    {

        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Descricao")]
        public String Descricao { get; set; }

        [Column("TotalPalavras")]
        public int TotalPalavras { get; set; }

        public virtual ICollection<Palavra> ListaDePalavras { get; set; }

    }
}



