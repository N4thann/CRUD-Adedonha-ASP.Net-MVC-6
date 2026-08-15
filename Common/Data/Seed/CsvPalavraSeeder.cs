using AdedonhaMVC.Features.Categorias;
using AdedonhaMVC.Features.Palavras;
using Microsoft.EntityFrameworkCore;

namespace AdedonhaMVC.Common.Data.Seed
{
    public static class CsvPalavraSeeder
    {
        public static async Task SeedAsync(Contexto context, string csvFilePath)
        {
            var categorias = await context.Categorias.Include(c => c.ListaDePalavras)
                .ToDictionaryAsync(c => c.Descricao);
            var palavras = await context.Palavras.ToDictionaryAsync(p => p.Descricao);

            var linhas = await File.ReadAllLinesAsync(csvFilePath);

            for (int i = 1; i < linhas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(linhas[i]))
                {
                    continue;
                }

                var campos = linhas[i].Split(',');
                var categoriaDescricao = campos[1].Trim();
                var palavraDescricao = campos[3].Trim();

                if (!categorias.TryGetValue(categoriaDescricao, out var categoria))
                {
                    categoria = new Categoria
                    {
                        Descricao = categoriaDescricao,
                        TotalPalavras = 0,
                        ListaDePalavras = new List<Palavra>()
                    };
                    context.Categorias.Add(categoria);
                    categorias[categoriaDescricao] = categoria;
                }

                if (!palavras.TryGetValue(palavraDescricao, out var palavra))
                {
                    palavra = new Palavra { Descricao = palavraDescricao };
                    context.Palavras.Add(palavra);
                    palavras[palavraDescricao] = palavra;
                }

                if (!categoria.ListaDePalavras.Contains(palavra))
                {
                    categoria.ListaDePalavras.Add(palavra);
                    categoria.TotalPalavras += 1;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
