using AdedonhaMVC.Common.Data;
using AdedonhaMVC.Common.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace AdedonhaMVC.Tests.Common.Data.Seed
{
    public class CsvPalavraSeederTests : IDisposable
    {
        private readonly Contexto _context;
        private readonly string _csvFilePath;

        public CsvPalavraSeederTests()
        {
            var options = new DbContextOptionsBuilder<Contexto>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new Contexto(options);

            _csvFilePath = Path.GetTempFileName();
            File.WriteAllText(_csvFilePath,
                "Id,Categoria,Letra,Palavra\n" +
                "1,Comida,A,Arroz\n" +
                "2,Comida,B,Batata\n" +
                "3,Fruta,A,Arroz\n");
        }

        public void Dispose()
        {
            if (File.Exists(_csvFilePath))
            {
                File.Delete(_csvFilePath);
            }
        }

        [Fact(DisplayName = "SUCESSO - Deve criar Categorias, Palavras e associações a partir do CSV")]
        public async Task SeedAsync_WhenBancoVazio_ShouldCreateCategoriasPalavrasEAssociacoes()
        {
            // Arrange
            // (banco vazio, CSV de fixture criado no construtor)

            // Act
            await CsvPalavraSeeder.SeedAsync(_context, _csvFilePath);

            // Assert
            (await _context.Categorias.CountAsync()).ShouldBe(2);
            (await _context.Palavras.CountAsync()).ShouldBe(2);

            var comida = await _context.Categorias.Include(c => c.ListaDePalavras)
                .FirstAsync(c => c.Descricao == "Comida");
            comida.TotalPalavras.ShouldBe(2);
            comida.ListaDePalavras.ShouldContain(p => p.Descricao == "Arroz");
            comida.ListaDePalavras.ShouldContain(p => p.Descricao == "Batata");

            var fruta = await _context.Categorias.Include(c => c.ListaDePalavras)
                .FirstAsync(c => c.Descricao == "Fruta");
            fruta.TotalPalavras.ShouldBe(1);
            fruta.ListaDePalavras.ShouldContain(p => p.Descricao == "Arroz");
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Deve reaproveitar a mesma Palavra entre Categorias diferentes")]
        public async Task SeedAsync_WhenPalavraApareceEmDuasCategorias_ShouldReuseSamePalavra()
        {
            // Arrange
            // (banco vazio, CSV de fixture com "Arroz" em Comida e Fruta)

            // Act
            await CsvPalavraSeeder.SeedAsync(_context, _csvFilePath);

            // Assert
            var arrozCount = await _context.Palavras.CountAsync(p => p.Descricao == "Arroz");
            arrozCount.ShouldBe(1);
        }

        [Fact(DisplayName = "REGRA DE NEGÓCIO - Deve ser idempotente ao rodar duas vezes seguidas")]
        public async Task SeedAsync_WhenChamadoDuasVezes_ShouldNotDuplicateAnything()
        {
            // Arrange
            await CsvPalavraSeeder.SeedAsync(_context, _csvFilePath);

            // Act
            await CsvPalavraSeeder.SeedAsync(_context, _csvFilePath);

            // Assert
            (await _context.Categorias.CountAsync()).ShouldBe(2);
            (await _context.Palavras.CountAsync()).ShouldBe(2);

            var comida = await _context.Categorias.FirstAsync(c => c.Descricao == "Comida");
            comida.TotalPalavras.ShouldBe(2);

            var fruta = await _context.Categorias.FirstAsync(c => c.Descricao == "Fruta");
            fruta.TotalPalavras.ShouldBe(1);
        }
    }
}
