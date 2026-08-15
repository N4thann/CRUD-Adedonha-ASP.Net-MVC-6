using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdedonhaMVC.Features.Palavras;

namespace AdedonhaMVC.Features.Categorias
{
    public class CategoriaViewModel
    {
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("Descricao")]
        [MaxLength(30), MinLength(2)]
        [Display(Name = "Descrição")]
        public String Descricao { get; set; }

        public int TotalPalavras { get; set; }

        public Char IdLetra { get; set; }

        public int IdPalavra { get; set; }

        public List<PalavraViewModel> Palavras { get; set; }

        public List<PalavraViewModel> PalavrasDisponiveis { get; set; }

    }
}
