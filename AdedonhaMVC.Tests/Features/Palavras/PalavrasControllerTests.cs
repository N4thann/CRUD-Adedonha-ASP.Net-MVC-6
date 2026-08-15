using AdedonhaMVC.Common.Data;
using AdedonhaMVC.Features.Categorias;
using AdedonhaMVC.Features.Palavras;
using AdedonhaMVC.Tests.DataBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace AdedonhaMVC.Tests.Features.Palavras
{
    public class PalavrasControllerTests
    {
        private readonly Contexto _context;
        private readonly PalavrasController _sut;

        public PalavrasControllerTests()
        {
            var options = new DbContextOptionsBuilder<Contexto>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new Contexto(options);
            _sut = new PalavrasController(_context);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a View com a lista completa de palavras")]
        public async Task Index_WhenChamado_ShouldReturnViewWithAllPalavras()
        {
            // Arrange
            var palavras = PalavraDataBuilder.AsList(3);
            _context.Palavras.AddRange(palavras);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.Index();

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var model = viewResult.Model.ShouldBeAssignableTo<IEnumerable<Palavra>>();
            model!.Count().ShouldBe(3);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a View com a palavra quando encontrada")]
        public async Task Details_WhenPalavraExiste_ShouldReturnViewWithPalavra()
        {
            // Arrange
            var palavra = PalavraDataBuilder.Create().WithId(1).Build();
            _context.Palavras.Add(palavra);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.Details(1);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var model = viewResult.Model.ShouldBeOfType<Palavra>();
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

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a palavra não existir")]
        public async Task Details_WhenPalavraNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.Details(999);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a View com a palavra")]
        public async Task Delete_WhenPalavraExiste_ShouldReturnViewWithPalavra()
        {
            // Arrange
            var palavra = PalavraDataBuilder.Create().WithId(1).Build();
            _context.Palavras.Add(palavra);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.Delete(1);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.Model.ShouldBeOfType<Palavra>();
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

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a palavra não existir")]
        public async Task Delete_WhenPalavraNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.Delete(999);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve remover a palavra e redirecionar para Index")]
        public async Task DeleteConfirmed_WhenPalavraExiste_ShouldRemovePalavraAndRedirectToIndex()
        {
            // Arrange
            var palavra = PalavraDataBuilder.Create().WithId(1).Build();
            _context.Palavras.Add(palavra);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.DeleteConfirmed(1);

            // Assert
            var redirect = result.ShouldBeOfType<RedirectToActionResult>();
            redirect.ActionName.ShouldBe("Index");
            (await _context.Palavras.FindAsync(1)).ShouldBeNull();
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Deve redirecionar para Index mesmo quando a palavra não existir")]
        public async Task DeleteConfirmed_WhenPalavraNaoExiste_ShouldRedirectToIndexWithoutError()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.DeleteConfirmed(999);

            // Assert
            var redirect = result.ShouldBeOfType<RedirectToActionResult>();
            redirect.ActionName.ShouldBe("Index");
        }

        [Fact(DisplayName = "SUCESSO - Deve criar a palavra com a Informacao informada e redirecionar para Index")]
        public async Task Create_WhenInformacaoIsProvided_ShouldCreatePalavraWithInformacaoAndRedirectToIndex()
        {
            // Arrange
            var palavraCVM = new PalavraCreateViewModel { Id = 1, Descricao = "Abacaxi", Informacao = "Fruta tropical" };

            // Act
            var result = await _sut.Create(palavraCVM);

            // Assert
            var redirect = result.ShouldBeOfType<RedirectToActionResult>();
            redirect.ActionName.ShouldBe("Index");
            var palavraSalva = await _context.Palavras.FindAsync(1);
            palavraSalva!.Informacao.ShouldBe("Fruta tropical");
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Deve gravar 'Sem Informação' quando Informacao não for informada")]
        public async Task Create_WhenInformacaoIsNull_ShouldSetInformacaoToSemInformacao()
        {
            // Arrange
            var palavraCVM = new PalavraCreateViewModel { Id = 1, Descricao = "Abacaxi", Informacao = null };

            // Act
            var result = await _sut.Create(palavraCVM);

            // Assert
            result.ShouldBeOfType<RedirectToActionResult>();
            var palavraSalva = await _context.Palavras.FindAsync(1);
            palavraSalva!.Informacao.ShouldBe("Sem Informação");
        }

        [Fact(DisplayName = "ERRO - Deve retornar a View com o model quando o ModelState for inválido")]
        public async Task Create_WhenModelStateIsInvalid_ShouldReturnViewWithModel()
        {
            // Arrange
            var palavraCVM = new PalavraCreateViewModel { Id = 1, Descricao = "A" };
            _sut.ModelState.AddModelError("Descricao", "Descrição deve ter ao menos 2 caracteres");

            // Act
            var result = await _sut.Create(palavraCVM);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.Model.ShouldBe(palavraCVM);
            (await _context.Palavras.CountAsync()).ShouldBe(0);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar o PalavraViewModel preenchido")]
        public async Task Edit_WhenPalavraExiste_ShouldReturnViewWithPalavraViewModel()
        {
            // Arrange
            var palavra = PalavraDataBuilder.Create().WithId(1).WithDescricao("Abacaxi").WithInformacao("Fruta").Build();
            _context.Palavras.Add(palavra);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.Edit(1);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var model = viewResult.Model.ShouldBeOfType<PalavraViewModel>();
            model.Id.ShouldBe(1);
            model.Descricao.ShouldBe("Abacaxi");
            model.Informacao.ShouldBe("Fruta");
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

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for válido mas a palavra não existir")]
        public async Task Edit_WhenPalavraNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.Edit(999);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve atualizar a palavra e redirecionar para Index quando o ModelState for válido")]
        public async Task Edit_WhenModelStateIsValid_ShouldUpdatePalavraAndRedirectToIndex()
        {
            // Arrange
            var palavra = PalavraDataBuilder.Create().WithId(1).WithDescricao("Abacaxi").Build();
            _context.Palavras.Add(palavra);
            await _context.SaveChangesAsync();
            _context.Entry(palavra).State = EntityState.Detached;
            var palavraVM = new PalavraViewModel { Id = 1, Descricao = "Abacaxi Atualizado", Informacao = "Nova info" };

            // Act
            var result = await _sut.Edit(palavraVM);

            // Assert
            var redirect = result.ShouldBeOfType<RedirectToActionResult>();
            redirect.ActionName.ShouldBe("Index");
            var palavraAtualizada = await _context.Palavras.FindAsync(1);
            palavraAtualizada!.Descricao.ShouldBe("Abacaxi Atualizado");
        }

        [Fact(DisplayName = "ERRO - Deve retornar a View com o model quando o ModelState for inválido")]
        public async Task Edit_WhenModelStateIsInvalid_ShouldReturnViewWithModel()
        {
            // Arrange
            var palavraVM = new PalavraViewModel { Id = 1, Descricao = "" };
            _sut.ModelState.AddModelError("Descricao", "Descrição é obrigatória");

            // Act
            var result = await _sut.Edit(palavraVM);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.Model.ShouldBe(palavraVM);
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Deve retornar NotFound quando o Id não existir mais no banco")]
        public async Task Edit_WhenPalavraNaoExisteMais_ShouldReturnNotFound()
        {
            // Arrange
            var palavraVM = new PalavraViewModel { Id = 999, Descricao = "Fantasma" };

            // Act
            var result = await _sut.Edit(palavraVM);

            // Assert
            // Mesma dependência do InMemory lançar DbUpdateConcurrencyException que o
            // Edit POST de CategoriasController.
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve separar as categorias já associadas das disponíveis")]
        public async Task AdicionarCategorias_WhenPalavraExiste_ShouldSeparateCategoriasAssociadasEDisponiveis()
        {
            // Arrange
            var palavra = PalavraDataBuilder.Create().WithId(1).Build();
            var categoriaAssociada = CategoriaDataBuilder.Create().WithId(1).WithDescricao("Frutas").Build();
            var categoriaDisponivel = CategoriaDataBuilder.Create().WithId(2).WithDescricao("Animais").Build();
            palavra.ListaDeCategorias = new List<Categoria> { categoriaAssociada };
            _context.Palavras.Add(palavra);
            _context.Categorias.Add(categoriaDisponivel);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.AdicionarCategorias(1);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var model = viewResult.Model.ShouldBeOfType<PalavraViewModel>();
            model.Categorias.ShouldHaveSingleItem();
            model.Categorias[0].Id.ShouldBe(1);
            model.CategoriasDisponiveis.ShouldHaveSingleItem();
            model.CategoriasDisponiveis[0].Id.ShouldBe(2);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for nulo")]
        public async Task AdicionarCategorias_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            // (nenhum dado necessário)

            // Act
            var result = await _sut.AdicionarCategorias(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id for válido mas a palavra não existir")]
        public async Task AdicionarCategorias_WhenPalavraNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            // (banco vazio)

            // Act
            var result = await _sut.AdicionarCategorias(999);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "SUCESSO - Deve incrementar TotalPalavras da categoria e associá-la à palavra")]
        public async Task SalvarCategorias_WhenCategoriaExiste_ShouldIncrementTotalPalavrasAndAddCategoria()
        {
            // Arrange
            var palavra = PalavraDataBuilder.Create().WithId(1).Build();
            var categoria = CategoriaDataBuilder.Create().WithId(1).WithTotalPalavras(0).Build();
            _context.Palavras.Add(palavra);
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            var palavraVM = new PalavraViewModel { Id = 1, IdCategoria = 1 };

            // Act
            var result = await _sut.SalvarCategorias(palavraVM);

            // Assert
            var redirect = result.ShouldBeOfType<RedirectToActionResult>();
            redirect.ActionName.ShouldBe("AdicionarCategorias");
            redirect.RouteValues!["id"].ShouldBe(1);
            var categoriaAtualizada = await _context.Categorias.FindAsync(1);
            categoriaAtualizada!.TotalPalavras.ShouldBe(1);
            var palavraAtualizada = await _context.Palavras.Include(p => p.ListaDeCategorias).FirstAsync(p => p.Id == 1);
            palavraAtualizada.ListaDeCategorias.ShouldContain(c => c.Id == 1);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a palavra não existir")]
        public async Task SalvarCategorias_WhenPalavraNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            var palavraVM = new PalavraViewModel { Id = 999, IdCategoria = 1 };

            // Act
            var result = await _sut.SalvarCategorias(palavraVM);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Não deve incrementar TotalPalavras quando a categoria não existir")]
        public async Task SalvarCategorias_WhenCategoriaNaoExiste_ShouldNotIncrementTotalPalavras()
        {
            // Arrange
            var palavra = PalavraDataBuilder.Create().WithId(1).Build();
            _context.Palavras.Add(palavra);
            await _context.SaveChangesAsync();
            var palavraVM = new PalavraViewModel { Id = 1, IdCategoria = 999 };

            // Act
            var result = await _sut.SalvarCategorias(palavraVM);

            // Assert
            result.ShouldBeOfType<RedirectToActionResult>();
            (await _context.Categorias.CountAsync()).ShouldBe(0);
        }

        [Fact(DisplayName = "SUCESSO - Deve decrementar TotalPalavras da categoria e desassociá-la da palavra")]
        public async Task RemoverCategorias_WhenCategoriaExiste_ShouldDecrementTotalPalavrasAndRemoveCategoria()
        {
            // Arrange
            var categoria = CategoriaDataBuilder.Create().WithId(1).WithTotalPalavras(1).Build();
            var palavra = PalavraDataBuilder.Create().WithId(1).Build();
            palavra.ListaDeCategorias = new List<Categoria> { categoria };
            _context.Palavras.Add(palavra);
            await _context.SaveChangesAsync();
            var palavraVM = new PalavraViewModel { Id = 1, IdCategoria = 1 };

            // Act
            var result = await _sut.RemoverCategorias(palavraVM);

            // Assert
            result.ShouldBeOfType<RedirectToActionResult>();
            var categoriaAtualizada = await _context.Categorias.FindAsync(1);
            categoriaAtualizada!.TotalPalavras.ShouldBe(0);
            var palavraAtualizada = await _context.Palavras.Include(p => p.ListaDeCategorias).FirstAsync(p => p.Id == 1);
            palavraAtualizada.ListaDeCategorias.ShouldNotContain(c => c.Id == 1);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a palavra não existir")]
        public async Task RemoverCategorias_WhenPalavraNaoExiste_ShouldReturnNotFound()
        {
            // Arrange
            var palavraVM = new PalavraViewModel { Id = 999, IdCategoria = 1 };

            // Act
            var result = await _sut.RemoverCategorias(palavraVM);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Não deve decrementar TotalPalavras quando a categoria não existir")]
        public async Task RemoverCategorias_WhenCategoriaNaoExiste_ShouldNotDecrementTotalPalavras()
        {
            // Arrange
            var palavra = PalavraDataBuilder.Create().WithId(1).Build();
            _context.Palavras.Add(palavra);
            await _context.SaveChangesAsync();
            var palavraVM = new PalavraViewModel { Id = 1, IdCategoria = 999 };

            // Act
            var result = await _sut.RemoverCategorias(palavraVM);

            // Assert
            result.ShouldBeOfType<RedirectToActionResult>();
        }
    }
}
