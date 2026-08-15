using AdedonhaMVC.Features.Categorias;
using AdedonhaMVC.Features.Palavras;
using Bogus;

namespace AdedonhaMVC.Tests.DataBuilder
{
    public class CategoriaDataBuilder
    {
        private readonly Categoria _instance;

        public CategoriaDataBuilder()
        {
            var faker = new Faker<Categoria>("pt_BR")
                .RuleFor(c => c.Descricao, f => f.Commerce.Categories(1)[0])
                .RuleFor(c => c.TotalPalavras, f => 0)
                .RuleFor(c => c.ListaDePalavras, f => new List<Palavra>());

            _instance = faker.Generate();
        }

        public static CategoriaDataBuilder Create() => new();
        public Categoria Build() => _instance;
        public static implicit operator Categoria(CategoriaDataBuilder builder) => builder.Build();

        public static List<Categoria> AsList(int count)
        {
            var list = new List<Categoria>();
            for (int i = 0; i < count; i++) list.Add(Create().WithId(i + 1).Build());
            return list;
        }

        public CategoriaDataBuilder WithId(int id) { _instance.Id = id; return this; }
        public CategoriaDataBuilder WithDescricao(string descricao) { _instance.Descricao = descricao; return this; }
        public CategoriaDataBuilder WithTotalPalavras(int totalPalavras) { _instance.TotalPalavras = totalPalavras; return this; }
        public CategoriaDataBuilder WithListaDePalavras(List<Palavra> palavras) { _instance.ListaDePalavras = palavras; return this; }
    }
}
