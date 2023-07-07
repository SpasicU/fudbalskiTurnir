using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fudbalskiTurnir.Data;
using fudbalskiTurnir.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace fudbalskiTurnir.Controllers
{
    public class IgracsController : Controller
    {
        private readonly FudbalskiTurnirContext _context;

        public IgracsController(FudbalskiTurnirContext context)
        {
            _context = context;
        }

        // GET: Igracs
        public async Task<IActionResult> Index()
        {
            var fudbalskiTurnirContext = _context.Igracs.Include(i => i.IdTimaNavigation).OrderBy(i => i.IdTima);
            return View(await fudbalskiTurnirContext.ToListAsync());
        }

        // GET: Igracs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Igracs == null)
            {
                return NotFound();
            }

            var igrac = await _context.Igracs
                .Include(i => i.IdTimaNavigation)
                .FirstOrDefaultAsync(m => m.IdIgrac == id);
            if (igrac == null)
            {
                return NotFound();
            }

            return View(igrac);
        }
        [Authorize]
        // GET: Igracs/Create
        public IActionResult Create()
        {
            ViewData["IdTima"] = new SelectList(_context.Tims, "IdTima", "Naziv"); //OVDE ID
            return View();
        }

        // POST: Igracs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdIgrac,ImeIgraca,IdTima")] Igrac igrac)
        {
            if (ModelState.IsValid)
            {
                _context.Add(igrac);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTima"] = new SelectList(_context.Tims, "IdTima", "Naziv", igrac.IdTima); //OVDE
            return View(igrac);
        }
        [Authorize]
        // GET: Igracs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Igracs == null)
            {
                return NotFound();
            }

            var igrac = await _context.Igracs.FindAsync(id);
            if (igrac == null)
            {
                return NotFound();
            }
            ViewData["IdTima"] = new SelectList(_context.Tims, "IdTima", "Naziv", igrac.IdTima);
            return View(igrac);
        }

        // POST: Igracs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdIgrac,ImeIgraca,IdTima")] Igrac igrac)
        {
            if (id != igrac.IdIgrac)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(igrac);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IgracExists(igrac.IdIgrac))
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
            ViewData["IdTima"] = new SelectList(_context.Tims, "IdTima", "Naziv", igrac.IdTima);
            return View(igrac);
        }

        [Authorize]
        // GET: Igracs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Igracs == null)
            {
                return NotFound();
            }

            var igrac = await _context.Igracs
                .Include(i => i.IdTimaNavigation)
                .FirstOrDefaultAsync(m => m.IdIgrac == id);
            if (igrac == null)
            {
                return NotFound();
            }

            return View(igrac);
        }

        // POST: Igracs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Igracs == null)
            {
                return Problem("Entity set 'FudbalskiTurnirContext.Igracs'  is null.");
            }
            var igrac = await _context.Igracs.FindAsync(id);
            if (igrac != null)
            {
                _context.Igracs.Remove(igrac);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IgracExists(int id)
        {
          return (_context.Igracs?.Any(e => e.IdIgrac == id)).GetValueOrDefault();
        }
    }
}
