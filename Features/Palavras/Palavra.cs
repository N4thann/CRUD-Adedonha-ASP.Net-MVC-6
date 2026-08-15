using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdedonhaMVC.Features.Categorias;

namespace AdedonhaMVC.Features.Palavras
{
    [Table("Palavra")]
    public class Palavra
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Descricao")]
        public String Descricao { get; set; }

        [Column("Informacao")]
        public String? Informacao { get; set; }

        public virtual ICollection<Categoria> ListaDeCategorias { get; set; }
    }
}



