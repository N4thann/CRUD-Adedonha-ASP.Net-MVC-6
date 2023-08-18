using AdedonhaMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AdedonhaMVC.Data
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Palavra> Palavras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>()
                .HasMany(c => c.ListaDePalavras)
                .WithMany(p => p.ListaDeCategorias)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoriaPalavra",
                    j => j
                        .HasOne<Palavra>()
                        .WithMany()
                        .HasForeignKey("PalavraId")
                        .HasConstraintName("FK_CategoriaPalavra_Palavra_PalavraId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Categoria>()
                        .WithMany()
                        .HasForeignKey("CategoriaId")
                        .HasConstraintName("FK_CategoriaPalavra_Categoria_CategoriaId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("CategoriaId", "PalavraId");
                        j.ToTable("CategoriaPalavra");
                    }
                );
        }


    }
}
