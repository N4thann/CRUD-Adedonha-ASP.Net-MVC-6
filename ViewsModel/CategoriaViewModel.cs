using System.ComponentModel.DataAnnotations.Schema;

namespace AdedonhaMVC.ViewsModel
{
    public class CategoriaViewModel
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Descricao")]
        public String Descricao { get; set; }

        public int TotalPalavras { get; set; }

        public Char IdLetra { get; set; }

        public int IdPalavra { get; set; }

        public List<PalavraViewModel> Palavras { get; set; }

        public List<PalavraViewModel> PalavrasDisponiveis { get; set; }

    }
}
