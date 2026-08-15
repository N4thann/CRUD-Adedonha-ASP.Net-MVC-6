using AdedonhaMVC.Common.Data;
using AdedonhaMVC.Features.Categorias;
using AdedonhaMVC.Features.Home;
using AdedonhaMVC.Tests.DataBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace AdedonhaMVC.Tests.Features.Home
{
    public class HomeControllerTests
    {
        private readonly Contexto _context;
        private readonly HomeController _sut;

        public HomeControllerTests()
        {
            var options = new DbContextOptionsBuilder<Contexto>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new Contexto(options);
            var logger = Substitute.For<ILogger<HomeController>>();
            _sut = new HomeController(logger, _context);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a View com a lista completa de categorias")]
        public async Task Index_WhenChamado_ShouldReturnViewWithAllCategorias()
        {
            // Arrange
            var categorias = CategoriaDataBuilder.AsList(2);
            _context.Categorias.AddRange(categorias);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.Index();

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            var model = viewResult.Model.ShouldBeAssignableTo<IEnumerable<Categoria>>();
            model!.Count().ShouldBe(2);
        }
    }
}
