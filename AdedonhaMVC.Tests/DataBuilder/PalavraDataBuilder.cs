using AdedonhaMVC.Features.Categorias;
using AdedonhaMVC.Features.Palavras;
using Bogus;

namespace AdedonhaMVC.Tests.DataBuilder
{
    public class PalavraDataBuilder
    {
        private readonly Palavra _instance;

        public PalavraDataBuilder()
        {
            var faker = new Faker<Palavra>("pt_BR")
                .RuleFor(p => p.Descricao, f => f.Lorem.Word())
                .RuleFor(p => p.Informacao, f => f.Lorem.Sentence())
                .RuleFor(p => p.ListaDeCategorias, f => new List<Categoria>());

            _instance = faker.Generate();
        }

        public static PalavraDataBuilder Create() => new();
        public Palavra Build() => _instance;
        public static implicit operator Palavra(PalavraDataBuilder builder) => builder.Build();

        public static List<Palavra> AsList(int count)
        {
            var list = new List<Palavra>();
            for (int i = 0; i < count; i++) list.Add(Create().WithId(i + 1).Build());
            return list;
        }

        public PalavraDataBuilder WithId(int id) { _instance.Id = id; return this; }
        public PalavraDataBuilder WithDescricao(string descricao) { _instance.Descricao = descricao; return this; }
        public PalavraDataBuilder WithInformacao(string? informacao) { _instance.Informacao = informacao; return this; }
        public PalavraDataBuilder WithListaDeCategorias(List<Categoria> categorias) { _instance.ListaDeCategorias = categorias; return this; }
    }
}
