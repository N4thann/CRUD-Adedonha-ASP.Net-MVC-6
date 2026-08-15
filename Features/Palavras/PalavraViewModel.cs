using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdedonhaMVC.Features.Categorias;

namespace AdedonhaMVC.Features.Palavras
{
    public class PalavraViewModel
    {

        public int Id { get; set; }

        [Required]
        [Column("Descricao")]
        [MaxLength(30), MinLength(2)]
        [Display(Name = "Descrição")]
        public String Descricao { get; set; }

        public String? Informacao { get; set; }

        public int IdCategoria { get; set; }

        public List<CategoriaViewModel> Categorias { get; set; }

        public List<CategoriaViewModel> CategoriasDisponiveis { get; set; }

    }
}
