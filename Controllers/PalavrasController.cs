using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdedonhaMVC.Data;
using AdedonhaMVC.Models;
using AdedonhaMVC.ViewsModel;

namespace AdedonhaMVC.Controllers
{
    public class PalavrasController : Controller
    {
        private readonly Contexto _context;

        public PalavrasController(Contexto context)
        {
            _context = context;
        }

        // GET: Palavras
        public async Task<IActionResult> Index()
        {
            return _context.Palavras != null ?
                        View(await _context.Palavras.ToListAsync()) :
                        Problem("Entity set 'Contexto.Palavras'  is null.");
        }

        // GET: Palavras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Palavras == null)
            {
                return NotFound();
            }

            var palavra = await _context.Palavras
                .FirstOrDefaultAsync(m => m.Id == id);
            if (palavra == null)
            {
                return NotFound();
            }

            return View(palavra);
        }

        // GET: Palavras/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Palavras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PalavraCreateViewModel palavraCVM)
        {

            if (ModelState.IsValid)
            {
                Palavra palavra = new Palavra();
                palavra.Id = palavraCVM.Id;
                palavra.Descricao = palavraCVM.Descricao;
                
                if(palavraCVM.Informacao == null)
                {
                    palavra.Informacao = "Sem Informação";
                }
                else
                {
                    palavra.Informacao = palavraCVM.Informacao;
                }

                _context.Add(palavra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(palavraCVM);
        }

        // GET: Palavras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var palavra = _context.Palavras.Include(p => p.ListaDeCategorias)
                .ToList().FirstOrDefault(g => g.Id == id);

            if (id == null)
            {
                return NotFound();
            }

            var palavraVM = new PalavraViewModel();
            palavraVM.Id = palavra.Id;
            palavraVM.Descricao = palavra.Descricao;
            palavraVM.Informacao = palavra.Informacao;

            return View(palavraVM);
        }

        // POST: Palavras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PalavraViewModel palavraVM)
        {
            var palavra = await _context.Palavras.
                FirstOrDefaultAsync(m => m.Id == palavraVM.Id);


            if (palavra == null)
            {
                return NotFound();
            }
            else
            {
                palavra.Descricao = palavraVM.Descricao;
                palavra.Informacao = palavraVM.Informacao;
            }

            ModelState.Remove("Categorias");
            ModelState.Remove("CategoriasDisponiveis");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(palavra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PalavraExists(palavra.Id))
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
            return View(palavraVM);
        }

        public async Task<IActionResult> AdicionarCategorias(int? id)
        {
            var palavra = _context.Palavras.Include(p => p.ListaDeCategorias)
                .ToList().FirstOrDefault(g => g.Id == id);

            if (id == null)
            {
                return NotFound();
            }

            var palavraVM = new PalavraViewModel();
            palavraVM.Id = palavra.Id;
            palavraVM.Descricao = palavra.Descricao;
            palavraVM.Informacao = palavra.Informacao;

            palavraVM.Categorias = new List<CategoriaViewModel>();
            palavraVM.CategoriasDisponiveis = new List<CategoriaViewModel>();

            foreach (var item in palavra.ListaDeCategorias)
            {
                CategoriaViewModel categoriaVM = new CategoriaViewModel();

                categoriaVM.Id = item.Id;
                categoriaVM.Descricao = item.Descricao;
                categoriaVM.TotalPalavras = item.TotalPalavras;

                palavraVM.Categorias.Add(categoriaVM);
            }

            var categoriaArray = palavra.ListaDeCategorias.Select(c => c.Id).ToArray();

            var categoriasDisponiveis = _context.Categorias.
                Where(c => !categoriaArray.Contains(c.Id)).ToList();

            foreach (var item in categoriasDisponiveis)
            {
                CategoriaViewModel categoriaVM = new CategoriaViewModel();

                categoriaVM.Id = item.Id;
                categoriaVM.Descricao = item.Descricao;
                categoriaVM.TotalPalavras = item.TotalPalavras;

                palavraVM.CategoriasDisponiveis.Add(categoriaVM);
            }

            return View(palavraVM);
        }

        public async Task<IActionResult> SalvarCategorias(PalavraViewModel palavraVM)
        {
            var palavra = _context.Palavras.Include(p => p.ListaDeCategorias)
                .ToList().FirstOrDefault(g => g.Id == palavraVM.Id);


            if (palavra == null)
            {
                return NotFound();
            }
            else
            {

                var categoria = _context.Categorias.Include(c => c.ListaDePalavras).
                    FirstOrDefault(p => p.Id == palavraVM.IdCategoria);

                if (categoria != null)
                {
                    categoria.TotalPalavras = categoria.TotalPalavras + 1;
                    palavra.ListaDeCategorias.Add(categoria);                   
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("AdicionarCategorias", new { id = palavraVM.Id });

        }

        public async Task<IActionResult> RemoverCategorias(PalavraViewModel palavraVM)
        {
            var palavra = _context.Palavras.Include(p => p.ListaDeCategorias)
                .ToList().FirstOrDefault(g => g.Id == palavraVM.Id);

            if (palavra == null)
            {
                return NotFound();
            }
            else
            {
                var categoria = _context.Categorias.Include(c => c.ListaDePalavras).
                    FirstOrDefault(p => p.Id == palavraVM.IdCategoria);

                if (categoria != null)
                {
                    categoria.TotalPalavras = categoria.TotalPalavras - 1;
                    palavra.ListaDeCategorias.Remove(categoria);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("AdicionarCategorias", new { id = palavraVM.Id });

        }


        // GET: Palavras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Palavras == null)
            {
                return NotFound();
            }

            var palavra = await _context.Palavras
                .FirstOrDefaultAsync(m => m.Id == id);

            if (palavra == null)
            {
                return NotFound();
            }

            return View(palavra);
        }

        // POST: Palavras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Palavras == null)
            {
                return Problem("Entity set 'Contexto.Palavras'  is null.");
            }
            var palavra = await _context.Palavras.FindAsync(id);
            if (palavra != null)
            {
                _context.Palavras.Remove(palavra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PalavraExists(int id)
        {
            return (_context.Palavras?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
