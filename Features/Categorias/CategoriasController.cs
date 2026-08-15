using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdedonhaMVC.Common.Data;
using AdedonhaMVC.Features.Palavras;

namespace AdedonhaMVC.Features.Categorias
{
    public class CategoriasController : Controller
    {
        private readonly Contexto _context;

        public CategoriasController(Contexto context)
        {
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            return _context.Categorias != null ?
                        View(await _context.Categorias.AsNoTracking().ToListAsync()) :
                        Problem("Entity set 'Contexto.Categorias'  is null.");
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        public async Task<IActionResult> Letras(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.Include(c => c.ListaDePalavras)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            SomatorioDasPalavrasDeCadaLetra(categoria);


            return View(categoria);
        }

        public async Task<IActionResult> PalavrasPorLetra(int? id, char letra)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.Include(c => c.ListaDePalavras)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            var palavrasPorLetra = categoria.ListaDePalavras
            .Where(p => p.Descricao.StartsWith(letra.ToString(), StringComparison.OrdinalIgnoreCase))
            .ToList();

            var categoriaVM = new CategoriaViewModel();
            categoriaVM.Id = categoria.Id;
            categoriaVM.Descricao = categoria.Descricao;
            categoriaVM.TotalPalavras = categoria.TotalPalavras;

            categoriaVM.Palavras = new List<PalavraViewModel>();
            categoriaVM.PalavrasDisponiveis = new List<PalavraViewModel>();

            foreach (var item in palavrasPorLetra)
            {
                PalavraViewModel palavraVM = new PalavraViewModel();

                palavraVM.Id = item.Id;
                palavraVM.Descricao = item.Descricao;
                palavraVM.Informacao = item.Informacao;

                categoriaVM.Palavras.Add(palavraVM);
            }

            ViewBag.Letra = letra;

            return View(categoriaVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaCreateViewModel categoriaVM)
        {
            if (ModelState.IsValid)
            {
                Categoria categoria = new Categoria();
                categoria.Descricao = categoriaVM.Descricao;

                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaVM);
        }


        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.Include(p => p.ListaDePalavras)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            var categoriaVM = new CategoriaViewModel();
            categoriaVM.Id = categoria.Id;
            categoriaVM.Descricao = categoria.Descricao;
            categoriaVM.TotalPalavras = categoria.TotalPalavras;

            return View(categoriaVM);
        }

        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoriaViewModel categoriaVM)
        {

            ModelState.Remove("Palavras");
            ModelState.Remove("PalavrasDisponiveis");

            if (ModelState.IsValid)
            {
                var categoria = new Categoria
                {
                    Id = categoriaVM.Id,
                    Descricao = categoriaVM.Descricao
                };

                //O Entry faz parte do gerenciamento de entidades do Entity Framework
                // O método Entry permite acessar um objeto EntityEntry que representa
                //a entidade no contexto e fornece métodos para controlar seu estado.
                //Modified: A entidade já existe no banco de dados e foi modificada.
                _context.Entry(categoria).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaVM);
        }

        //GET: Categoria/AdicionarPalavras/5
        public async Task<IActionResult> AdicionarPalavras(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.Include(p => p.ListaDePalavras)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            var categoriaVM = new CategoriaViewModel();
            categoriaVM.Id = categoria.Id;
            categoriaVM.Descricao = categoria.Descricao;
            categoriaVM.TotalPalavras = categoria.TotalPalavras;

            categoriaVM.Palavras = new List<PalavraViewModel>();
            categoriaVM.PalavrasDisponiveis = new List<PalavraViewModel>();

            foreach (var item in categoria.ListaDePalavras)
            {
                PalavraViewModel palavraVM = new PalavraViewModel();

                palavraVM.Id = item.Id;
                palavraVM.Descricao = item.Descricao;
                palavraVM.Informacao = item.Informacao;

                categoriaVM.Palavras.Add(palavraVM);
            }

            var palavraArray = categoria.ListaDePalavras.Select(c => c.Id).ToArray();

            var palavrasDisponiveis = await _context.Palavras.AsNoTracking()
                .Where(c => !palavraArray.Contains(c.Id)).ToListAsync();

            foreach (var item in palavrasDisponiveis)
            {
                PalavraViewModel palavraVM = new PalavraViewModel();

                palavraVM.Id = item.Id;
                palavraVM.Descricao = item.Descricao;
                palavraVM.Informacao = item.Informacao;

                categoriaVM.PalavrasDisponiveis.Add(palavraVM);
            }

            SomatorioDasPalavrasDeCadaLetra(categoria);

            return View(categoriaVM);
        }

        private void SomatorioDasPalavrasDeCadaLetra(Categoria categoria)
        {
            char[] letras = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            var contagemPorLetra = categoria.ListaDePalavras
                .Where(p => !string.IsNullOrEmpty(p.Descricao))
                .GroupBy(p => char.ToUpperInvariant(p.Descricao[0]))
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var letra in letras)
            {
                ViewData[$"Soma_{letra}"] = contagemPorLetra.TryGetValue(letra, out var soma) ? soma : 0;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarPalavras(CategoriaViewModel categoriaVM)
        {
            var categoria = await _context.Categorias.Include(p => p.ListaDePalavras)
                .FirstOrDefaultAsync(g => g.Id == categoriaVM.Id);

            if (categoria == null)
            {
                return NotFound();
            }
            else
            {
                var palavra = await _context.Palavras.Include(c => c.ListaDeCategorias)
                    .FirstOrDefaultAsync(p => p.Id == categoriaVM.IdPalavra);

                if (palavra != null)
                {
                    categoria.TotalPalavras = categoria.TotalPalavras + 1;
                    categoria.ListaDePalavras.Add(palavra);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("AdicionarPalavras", new { id = categoriaVM.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverPalavras(CategoriaViewModel categoriaVM)
        {
            var categoria = await _context.Categorias.Include(p => p.ListaDePalavras)
                .FirstOrDefaultAsync(g => g.Id == categoriaVM.Id);


            if (categoria == null)
            {
                return NotFound();
            }
            else
            {

                var palavra = await _context.Palavras.Include(c => c.ListaDeCategorias)
                    .FirstOrDefaultAsync(p => p.Id == categoriaVM.IdPalavra);

                if (palavra != null)
                {
                    categoria.TotalPalavras = categoria.TotalPalavras - 1;
                    categoria.ListaDePalavras.Remove(palavra);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("AdicionarPalavras", new { id = categoriaVM.Id });

        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categorias == null)
            {
                return Problem("Entity set 'Contexto.Categorias'  is null.");
            }
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
            return (_context.Categorias?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
