using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdedonhaMVC.ViewsModel
{
    public class CategoriaCreateViewModel
    {

        public int Id { get; set; }

        [Required]
        [Column("Descricao")]
        [MaxLength(30), MinLength(2)]
        [Display(Name = "Descrição")]
        public String Descricao { get; set; }

    }
}

