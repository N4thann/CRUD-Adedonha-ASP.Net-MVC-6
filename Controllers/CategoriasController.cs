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
                        View(await _context.Categorias.ToListAsync()) :
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
                categoria.Id = categoriaVM.Id;
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
            var categoria = _context.Categorias.Include(p => p.ListaDePalavras)
                .ToList().FirstOrDefault(g => g.Id == id);

            if (id == null)
            {
                return NotFound();
            }

            var categoriaVM = new CategoriaViewModel();
            categoriaVM.Id = categoria.Id;
            categoriaVM.Descricao = categoria.Descricao;
            categoriaVM.TotalPalavras = categoria.TotalPalavras;

            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoriaVM);
        }

        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoriaViewModel categoriaVM)
        {
            var categoria = await _context.Categorias.
                FirstOrDefaultAsync(m => m.Id == categoriaVM.Id);

            if (categoria == null)
            {
                return NotFound();
            }
            else
            {
                categoria.Descricao = categoriaVM.Descricao;
            }

            ModelState.Remove("Palavras");
            ModelState.Remove("PalavrasDisponiveis");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
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
            var categoria = _context.Categorias.Include(p => p.ListaDePalavras)
                .ToList().FirstOrDefault(g => g.Id == id);

            if (id == null)
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

            var palavrasDisponiveis = _context.Palavras.
                Where(c => !palavraArray.Contains(c.Id)).ToList();

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
            int somatorioPalavrasComA = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("A", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_A = somatorioPalavrasComA;

            int somatorioPalavrasComB = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("B", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_B = somatorioPalavrasComB;

            int somatorioPalavrasComC = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("C", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_C = somatorioPalavrasComC;

            int somatorioPalavrasComD = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("D", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_D = somatorioPalavrasComD;

            int somatorioPalavrasComE = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("E", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_E = somatorioPalavrasComE;

            int somatorioPalavrasComF = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("F", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_F = somatorioPalavrasComF;

            int somatorioPalavrasComG = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("G", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_G = somatorioPalavrasComG;

            int somatorioPalavrasComH = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("H", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_H = somatorioPalavrasComH;

            int somatorioPalavrasComI = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("I", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_I = somatorioPalavrasComI;

            int somatorioPalavrasComJ = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("J", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_J = somatorioPalavrasComJ;

            int somatorioPalavrasComK = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("K", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_K = somatorioPalavrasComK;

            int somatorioPalavrasComL = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("L", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_L = somatorioPalavrasComL;

            int somatorioPalavrasComM = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("M", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_M = somatorioPalavrasComM;

            int somatorioPalavrasComN = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("N", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_N = somatorioPalavrasComN;

            int somatorioPalavrasComO = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("O", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_O = somatorioPalavrasComO;

            int somatorioPalavrasComP = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("P", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_P = somatorioPalavrasComP;

            int somatorioPalavrasComQ = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("Q", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_Q = somatorioPalavrasComQ;

            int somatorioPalavrasComR = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("R", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_R = somatorioPalavrasComR;

            int somatorioPalavrasComS = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("S", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_S = somatorioPalavrasComS;

            int somatorioPalavrasComT = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("T", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_T = somatorioPalavrasComT;

            int somatorioPalavrasComU = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("U", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_U = somatorioPalavrasComU;

            int somatorioPalavrasComV = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("V", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_V = somatorioPalavrasComV;

            int somatorioPalavrasComY = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("Y", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_Y = somatorioPalavrasComY;

            int somatorioPalavrasComZ = categoria.ListaDePalavras.
                Count(p => p.Descricao.StartsWith("Z", StringComparison.OrdinalIgnoreCase));
            ViewBag.Soma_Z = somatorioPalavrasComZ;

        }

        public async Task<IActionResult> SalvarPalavras(CategoriaViewModel categoriaVM)
        {
            var categoria = _context.Categorias.Include(p => p.ListaDePalavras)
                .ToList().FirstOrDefault(g => g.Id == categoriaVM.Id);

            if (categoria == null)
            {
                return NotFound();
            }
            else
            {
                var palavra = _context.Palavras.Include(c => c.ListaDeCategorias).
                    FirstOrDefault(p => p.Id == categoriaVM.IdPalavra);

                if (palavra != null)
                {
                    categoria.TotalPalavras = categoria.TotalPalavras + 1;
                    categoria.ListaDePalavras.Add(palavra);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("AdicionarPalavras", new { id = categoriaVM.Id });
        }

        public async Task<IActionResult> RemoverPalavras(CategoriaViewModel categoriaVM)
        {
            var categoria = _context.Categorias.Include(p => p.ListaDePalavras)
                .ToList().FirstOrDefault(g => g.Id == categoriaVM.Id);


            if (categoria == null)
            {
                return NotFound();
            }
            else
            {

                var palavra = _context.Palavras.Include(c => c.ListaDeCategorias).
                    FirstOrDefault(p => p.Id == categoriaVM.IdPalavra);

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
