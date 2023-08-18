using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AdedonhaMVC.ViewsModel
{
    public class PalavraCreateViewModel
    {
        public int Id { get; set; }

        [Required]
        [Column("Descricao")]
        [MaxLength(30), MinLength(2)]
        [Display(Name = "Descrição")]
        public String Descricao { get; set; }

        [MaxLength(500), MinLength(5)]
        [Column("Informacao")]
        [Display(Name = "Informação sobre a palavra")]
        public String? Informacao { get; set; }

    }
}

