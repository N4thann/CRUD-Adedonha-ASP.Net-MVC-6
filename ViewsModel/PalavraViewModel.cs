using System.ComponentModel.DataAnnotations.Schema;

namespace AdedonhaMVC.ViewsModel
{
    public class PalavraViewModel
    {
    
        public int Id { get; set; }

        public String Descricao { get; set; }

        public String? Informacao { get; set; }

        public int IdCategoria { get; set; }

        public List<CategoriaViewModel> Categorias { get; set; }

        public List<CategoriaViewModel> CategoriasDisponiveis { get; set; }

    }
}
