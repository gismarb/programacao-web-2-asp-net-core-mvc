using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projeto1_IF.Data;
using Projeto1_IF.Models;

// Gismar Pereira Barbosa

namespace Projeto1_IF.Controllers
{
    public class TbSuplementosController : Controller
    {
        private readonly db_IFContext _context;

        public TbSuplementosController(db_IFContext context)
        {
            _context = context;
        }

        // GET: TbSuplementos
        public async Task<IActionResult> Index()
        {
            return View(await _context.TbSuplementos.ToListAsync());
        }

        // GET: TbSuplementos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbSuplemento = await _context.TbSuplementos
                .FirstOrDefaultAsync(m => m.IdSuplemento == id);
            if (tbSuplemento == null)
            {
                return NotFound();
            }

            return View(tbSuplemento);
        }

        // GET: TbSuplementos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TbSuplementos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSuplemento,IdTipoQuantidade,Tipo,Nome,DoseMinima,DoseMaxima,Carboidrato,VitaminaA,VitaminaB")] TbSuplemento tbSuplemento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbSuplemento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tbSuplemento);
        }

        // GET: TbSuplementos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbSuplemento = await _context.TbSuplementos.FindAsync(id);
            if (tbSuplemento == null)
            {
                return NotFound();
            }
            return View(tbSuplemento);
        }

        // POST: TbSuplementos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSuplemento,IdTipoQuantidade,Tipo,Nome,DoseMinima,DoseMaxima,Carboidrato,VitaminaA,VitaminaB")] TbSuplemento tbSuplemento)
        {
            if (id != tbSuplemento.IdSuplemento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbSuplemento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbSuplementoExists(tbSuplemento.IdSuplemento))
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
            return View(tbSuplemento);
        }

        // GET: TbSuplementos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbSuplemento = await _context.TbSuplementos
                .FirstOrDefaultAsync(m => m.IdSuplemento == id);
            if (tbSuplemento == null)
            {
                return NotFound();
            }

            return View(tbSuplemento);
        }

        // POST: TbSuplementos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbSuplemento = await _context.TbSuplementos.FindAsync(id);
            if (tbSuplemento != null)
            {
                _context.TbSuplementos.Remove(tbSuplemento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbSuplementoExists(int id)
        {
            return _context.TbSuplementos.Any(e => e.IdSuplemento == id);
        }
    }
}
