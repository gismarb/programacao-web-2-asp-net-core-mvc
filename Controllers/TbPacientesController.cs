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
    public class TbPacientesController : Controller
    {
        private readonly db_IFContext _context;

        public TbPacientesController(db_IFContext context)
        {
            _context = context;
        }

        // GET: TbPacientes
        public async Task<IActionResult> Index()
        {
            var db_IFContext = _context.TbPacientes.Include(t => t.IdCidadeNavigation);
            return View(await db_IFContext.ToListAsync());
        }

        // GET: TbPacientes/Details/5
        // Gismar Pereira Barbosa
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbPaciente = await _context.TbPacientes
                .Include(t => t.IdCidadeNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdPaciente == id);
            if (tbPaciente == null)
            {
                return NotFound();
            }

            return View(tbPaciente);
        }

        // GET: TbPacientes/Create
        // Gismar Pereira Barbosa
        public IActionResult Create()
        {
            ViewData["IdCidade"] = new SelectList(_context.TbCidades, "IdCidade", "Nome");
            return View();
        }

        // POST: TbPacientes/Create
        // Gismar Pereira Barbosa
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPaciente,Nome,Rg,Cpf,DataNascimento,NomeResponsavel,Sexo,Etnia,Endereco,Bairro,IdCidade,TelResidencial,TelComercial,TelCelular,Profissao,FlgAtleta,FlgGestante")] TbPaciente tbPaciente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbPaciente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCidade"] = new SelectList(_context.TbCidades, "IdCidade", "Nome", tbPaciente.IdCidade);
            return View(tbPaciente);
        }

        // GET: TbPacientes/Edit/5
        // Gismar Pereira Barbosa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbPaciente = await _context.TbPacientes.FindAsync(id);
            if (tbPaciente == null)
            {
                return NotFound();
            }
            ViewData["IdCidade"] = new SelectList(_context.TbCidades, "IdCidade", "Nome", tbPaciente.IdCidade);
            return View(tbPaciente);
        }

        // POST: TbPacientes/Edit/5
        // Gismar Pereira Barbosa
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pacienteToUpdate = await _context.TbPacientes
                .FirstOrDefaultAsync(p => p.IdPaciente == id);

            if (pacienteToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<TbPaciente>(
                pacienteToUpdate,
                "",
                p => p.Nome,
                p => p.Rg,
                p => p.Cpf,
                p => p.DataNascimento,
                p => p.NomeResponsavel,
                p => p.Sexo,
                p => p.Etnia,
                p => p.Endereco,
                p => p.Bairro,
                p => p.IdCidade,
                p => p.TelResidencial,
                p => p.TelComercial,
                p => p.TelCelular,
                p => p.Profissao,
                p => p.FlgAtleta,
                p => p.FlgGestante))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Não foi possível salvar as alterações. Tente novamente e, se o problema persistir, entre em contato com o suporte.");
                }
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidades, "IdCidade", "Nome", pacienteToUpdate.IdCidade);
            return View(pacienteToUpdate);
        }

        // GET: TbPacientes/Delete/5
        // Gismar Pereira Barbosa
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbPaciente = await _context.TbPacientes
                .Include(t => t.IdCidadeNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdPaciente == id);

            if (tbPaciente == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Não foi possível excluir o registro. Tente novamente e, se o problema persistir, entre em contato com o suporte.";
            }

            return View(tbPaciente);
        }

        // POST: TbPacientes/Delete/5
        // Gismar Pereira Barbosa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbPaciente = await _context.TbPacientes.FindAsync(id);

            if (tbPaciente == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.TbPacientes.Remove(tbPaciente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool TbPacienteExists(int id)
        {
            return _context.TbPacientes.Any(e => e.IdPaciente == id);
        }
    }
}
