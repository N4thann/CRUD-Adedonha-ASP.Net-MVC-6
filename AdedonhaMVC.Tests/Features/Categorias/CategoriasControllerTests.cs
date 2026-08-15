using AdedonhaMVC.Common.Data;
using AdedonhaMVC.Features.Categorias;
using AdedonhaMVC.Features.Palavras;
using AdedonhaMVC.Tests.DataBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace AdedonhaMVC.Tests.Features.Categorias
{
    public class CategoriasControllerTests
    {
        private readonly Contexto _context;
        private readonly CategoriasController _sut;

        public CategoriasControllerTests()
        {
            var options = new DbContextOptionsBuilder<Contexto>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new Contexto(options);
            _sut = new CategoriasController(_context);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a View com a lista completa de categorias")]
        public async Task Index_WhenChamado_ShouldReturnViewWithAllCategorias()
        {
            // Arrange
            var categorias = CategoriaDataBuilder.AsList(3);
            _context.Categorias.AddRange(categorias);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.Index();

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var model = viewResult.Model.ShouldBeAssignableTo<IEnumerable<Categoria>>();
            model!.Count().ShouldBe(3);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a View com a categoria quando encontrada")]
        public async Task Details_WhenCategoriaExiste_ShouldReturnViewWithCategoria()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).Build();
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.Details(1);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var model = viewResult.Model.ShouldBeOfType<Categoria>();
            model.Id.ShouldBe(1);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for nulo")]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            // (nenhum dado necessário)

            // Act
            var result = await _sut.Details(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria não existir")]
        public async Task Details_WhenCategoriaNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.Details(999);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a View com a categoria")]
        public async Task Delete_WhenCategoriaExiste_ShouldReturnViewWithCategoria()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).Build();
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.Delete(1);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.Model.ShouldBeOfType<Categoria>();
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for nulo")]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            // (nenhum dado necessário)

            // Act
            var result = await _sut.Delete(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria não existir")]
        public async Task Delete_WhenCategoriaNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.Delete(999);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve remover a categoria e redirecionar para Index")]
        public async Task DeleteConfirmed_WhenCategoriaExiste_ShouldRemoveCategoriaAndRedirectToIndex()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).Build();
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.DeleteConfirmed(1);

            // Assert
            var redirect = result.ShouldBeOfType<RedirectToActionResult>();
            redirect.ActionName.ShouldBe("Index");
            (await _context.Categorias.FindAsync(1)).ShouldBeNull();
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Deve redirecionar para Index mesmo quando a categoria não existir")]
        public async Task DeleteConfirmed_WhenCategoriaNaoExiste_ShouldRedirectToIndexWithoutError()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.DeleteConfirmed(999);

            // Assert
            var redirect = result.ShouldBeOfType<RedirectToActionResult>();
            redirect.ActionName.ShouldBe("Index");
        }

        [Fact(DisplayName = "SUCESSO - Deve criar a categoria e redirecionar para Index quando o ModelState for válido")]
        public async Task Create_WhenModelStateIsValid_ShouldCreateCategoriaAndRedirectToIndex()
        {
            // Arrange
            var categoriaVM = new CategoriaCreateViewModel { Id = 1, Descricao = "Frutas" };

            // Act
            var result = await _sut.Create(categoriaVM);

            // Assert
            var redirect = result.ShouldBeOfType<RedirectToActionResult>();
            redirect.ActionName.ShouldBe("Index");
            var categoriaSalva = await _context.Categorias.FindAsync(1);
            categoriaSalva.ShouldNotBeNull();
            categoriaSalva!.Descricao.ShouldBe("Frutas");
        }

        [Fact(DisplayName = "ERRO - Deve retornar a View com o model quando o ModelState for inválido")]
        public async Task Create_WhenModelStateIsInvalid_ShouldReturnViewWithModel()
        {
            // Arrange
            var categoriaVM = new CategoriaCreateViewModel { Id = 1, Descricao = "F" };
            _sut.ModelState.AddModelError("Descricao", "Descrição deve ter ao menos 2 caracteres");

            // Act
            var result = await _sut.Create(categoriaVM);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.Model.ShouldBe(categoriaVM);
            (await _context.Categorias.CountAsync()).ShouldBe(0);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar o CategoriaViewModel preenchido")]
        public async Task Edit_WhenCategoriaExiste_ShouldReturnViewWithCategoriaViewModel()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).WithDescricao("Frutas").WithTotalPalavras(2).Build();
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.Edit(1);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var model = viewResult.Model.ShouldBeOfType<CategoriaViewModel>();
            model.Id.ShouldBe(1);
            model.Descricao.ShouldBe("Frutas");
            model.TotalPalavras.ShouldBe(2);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for nulo")]
        public async Task Edit_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            // (nenhum dado necessário)

            // Act
            var result = await _sut.Edit((int?)null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for válido mas a categoria não existir")]
        public async Task Edit_WhenCategoriaNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.Edit(999);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve atualizar a categoria e redirecionar para Index quando o ModelState for válido")]
        public async Task Edit_WhenModelStateIsValid_ShouldUpdateCategoriaAndRedirectToIndex()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).WithDescricao("Frutas").Build();
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            _context.Entry(categoria).State = EntityState.Detached;
            var categoriaVM = new CategoriaViewModel { Id = 1, Descricao = "Frutas Atualizado" };

            // Act
            var result = await _sut.Edit(categoriaVM);

            // Assert
            var redirect = result.ShouldBeOfType<RedirectToActionResult>();
            redirect.ActionName.ShouldBe("Index");
            var categoriaAtualizada = await _context.Categorias.FindAsync(1);
            categoriaAtualizada!.Descricao.ShouldBe("Frutas Atualizado");
        }

        [Fact(DisplayName = "ERRO - Deve retornar a View com o model quando o ModelState for inválido")]
        public async Task Edit_WhenModelStateIsInvalid_ShouldReturnViewWithModel()
        {
            // Arrange
            var categoriaVM = new CategoriaViewModel { Id = 1, Descricao = "" };
            _sut.ModelState.AddModelError("Descricao", "Descrição é obrigatória");

            // Act
            var result = await _sut.Edit(categoriaVM);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.Model.ShouldBe(categoriaVM);
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Deve retornar NotFound quando o Id não existir mais no banco")]
        public async Task Edit_WhenCategoriaNaoExisteMais_ShouldReturnNotFound()
        {
            // Arrange
            var categoriaVM = new CategoriaViewModel { Id = 999, Descricao = "Fantasma" };

            // Act
            var result = await _sut.Edit(categoriaVM);

            // Assert
            // Depende do provider InMemory lançar DbUpdateConcurrencyException ao tentar
            // atualizar uma entidade com Id que não existe — o catch do controller então
            // consulta CategoriaExists(999) (false) e retorna NotFound.
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve calcular a contagem de palavras por letra corretamente")]
        public async Task Letras_WhenCategoriaExiste_ShouldCalculateSomatorioPorLetra()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).Build();
            categoria.ListaDePalavras = new List<Palavra>
            {
                PalavraDataBuilder.Create().WithId(1).WithDescricao("Abacaxi").Build(),
                PalavraDataBuilder.Create().WithId(2).WithDescricao("Amora").Build(),
                PalavraDataBuilder.Create().WithId(3).WithDescricao("Banana").Build(),
                PalavraDataBuilder.Create().WithId(4).WithDescricao("Zebra").Build()
            };
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.Letras(1);

            // Assert
            result.ShouldBeOfType<ViewResult>();
            ((int)_sut.ViewBag.Soma_A).ShouldBe(2);
            ((int)_sut.ViewBag.Soma_B).ShouldBe(1);
            ((int)_sut.ViewBag.Soma_Z).ShouldBe(1);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for nulo")]
        public async Task Letras_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            // (nenhum dado necessário)

            // Act
            var result = await _sut.Letras(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria não existir")]
        public async Task Letras_WhenCategoriaNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.Letras(999);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar somente as palavras que começam com a letra informada")]
        public async Task PalavrasPorLetra_WhenChamado_ShouldReturnOnlyPalavrasStartingWithLetra()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).Build();
            categoria.ListaDePalavras = new List<Palavra>
            {
                PalavraDataBuilder.Create().WithId(1).WithDescricao("Abacaxi").Build(),
                PalavraDataBuilder.Create().WithId(2).WithDescricao("amora").Build(),
                PalavraDataBuilder.Create().WithId(3).WithDescricao("Banana").Build()
            };
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.PalavrasPorLetra(1, 'a');

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var model = viewResult.Model.ShouldBeOfType<CategoriaViewModel>();
            model.Palavras.Count.ShouldBe(2);
            model.Palavras.ShouldAllBe(p => p.Descricao.StartsWith("a", StringComparison.OrdinalIgnoreCase));
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for nulo")]
        public async Task PalavrasPorLetra_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            // (nenhum dado necessário)

            // Act
            var result = await _sut.PalavrasPorLetra(null, 'a');

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria não existir")]
        public async Task PalavrasPorLetra_WhenCategoriaNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.PalavrasPorLetra(999, 'a');

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve separar as palavras já associadas das disponíveis")]
        public async Task AdicionarPalavras_WhenCategoriaExiste_ShouldSeparatePalavrasAssociadasEDisponiveis()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).Build();
            var palavraAssociada = PalavraDataBuilder.Create().WithId(1).WithDescricao("Abacaxi").Build();
            var palavraDisponivel = PalavraDataBuilder.Create().WithId(2).WithDescricao("Banana").Build();
            categoria.ListaDePalavras = new List<Palavra> { palavraAssociada };
            _context.Categorias.Add(categoria);
            _context.Palavras.Add(palavraDisponivel);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.AdicionarPalavras(1);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var model = viewResult.Model.ShouldBeOfType<CategoriaViewModel>();
            model.Palavras.ShouldHaveSingleItem();
            model.Palavras[0].Id.ShouldBe(1);
            model.PalavrasDisponiveis.ShouldHaveSingleItem();
            model.PalavrasDisponiveis[0].Id.ShouldBe(2);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for nulo")]
        public async Task AdicionarPalavras_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            // (nenhum dado necessário)

            // Act
            var result = await _sut.AdicionarPalavras(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for válido mas a categoria não existir")]
        public async Task AdicionarPalavras_WhenCategoriaNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.AdicionarPalavras(999);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve incrementar TotalPalavras e adicionar a palavra à categoria")]
        public async Task SalvarPalavras_WhenPalavraExiste_ShouldIncrementTotalPalavrasAndAddPalavra()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).WithTotalPalavras(0).Build();
            var palavra = PalavraDataBuilder.Create().WithId(1).Build();
            _context.Categorias.Add(categoria);
            _context.Palavras.Add(palavra);
            await _context.SaveChangesAsync();
            var categoriaVM = new CategoriaViewModel { Id = 1, IdPalavra = 1 };

            // Act
            var result = await _sut.SalvarPalavras(categoriaVM);

            // Assert
            var redirect = result.ShouldBeOfType<RedirectToActionResult>();
            redirect.ActionName.ShouldBe("AdicionarPalavras");
            redirect.RouteValues!["id"].ShouldBe(1);
            var categoriaAtualizada = await _context.Categorias.Include(c => c.ListaDePalavras).FirstAsync(c => c.Id == 1);
            categoriaAtualizada.TotalPalavras.ShouldBe(1);
            categoriaAtualizada.ListaDePalavras.ShouldContain(p => p.Id == 1);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria não existir")]
        public async Task SalvarPalavras_WhenCategoriaNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            var categoriaVM = new CategoriaViewModel { Id = 999, IdPalavra = 1 };

            // Act
            var result = await _sut.SalvarPalavras(categoriaVM);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Não deve incrementar TotalPalavras quando a palavra não existir")]
        public async Task SalvarPalavras_WhenPalavraNaoExiste_ShouldNotIncrementTotalPalavras()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).WithTotalPalavras(0).Build();
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            var categoriaVM = new CategoriaViewModel { Id = 1, IdPalavra = 999 };

            // Act
            var result = await _sut.SalvarPalavras(categoriaVM);

            // Assert
            result.ShouldBeOfType<RedirectToActionResult>();
            var categoriaAtualizada = await _context.Categorias.FindAsync(1);
            categoriaAtualizada!.TotalPalavras.ShouldBe(0);
        }

        [Fact(DisplayName = "SUCESSO - Deve decrementar TotalPalavras e remover a palavra da categoria")]
        public async Task RemoverPalavras_WhenPalavraExiste_ShouldDecrementTotalPalavrasAndRemovePalavra()
        {
            // Arrange
            var palavra = PalavraDataBuilder.Create().WithId(1).Build();
            var categoria = CategoriaDataBuilder.Create().WithId(1).WithTotalPalavras(1).Build();
            categoria.ListaDePalavras = new List<Palavra> { palavra };
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            var categoriaVM = new CategoriaViewModel { Id = 1, IdPalavra = 1 };

            // Act
            var result = await _sut.RemoverPalavras(categoriaVM);

            // Assert
            result.ShouldBeOfType<RedirectToActionResult>();
            var categoriaAtualizada = await _context.Categorias.Include(c => c.ListaDePalavras).FirstAsync(c => c.Id == 1);
            categoriaAtualizada.TotalPalavras.ShouldBe(0);
            categoriaAtualizada.ListaDePalavras.ShouldNotContain(p => p.Id == 1);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria não existir")]
        public async Task RemoverPalavras_WhenCategoriaNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            var categoriaVM = new CategoriaViewModel { Id = 999, IdPalavra = 1 };

            // Act
            var result = await _sut.RemoverPalavras(categoriaVM);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Não deve decrementar TotalPalavras quando a palavra não existir")]
        public async Task RemoverPalavras_WhenPalavraNaoExiste_ShouldNotDecrementTotalPalavras()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).WithTotalPalavras(1).Build();
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            var categoriaVM = new CategoriaViewModel { Id = 1, IdPalavra = 999 };

            // Act
            var result = await _sut.RemoverPalavras(categoriaVM);

            // Assert
            result.ShouldBeOfType<RedirectToActionResult>();
            var categoriaAtualizada = await _context.Categorias.FindAsync(1);
            categoriaAtualizada!.TotalPalavras.ShouldBe(1);
        }
    }
}
