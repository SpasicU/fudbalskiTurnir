using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fudbalskiTurnir.Data;
using fudbalskiTurnir.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace fudbalskiTurnir.Controllers
{
    public class TimsController : Controller
    {
        private readonly FudbalskiTurnirContext _context;

        public TimsController(FudbalskiTurnirContext context)
        {
            _context = context;
        }

        // GET: Tims
        public async Task<IActionResult> Index()
        {
            ViewBag.Tim = (string)TempData["Message"];
            return _context.Tims != null ? 
                          View(await _context.Tims.OrderByDescending(i => i.Bodovi).ToListAsync()) :
                          Problem("Entity set 'FudbalskiTurnirContext.Tims'  is null.");

        }

        // GET: Tims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tims == null)
            {
                return NotFound();
            }

            var tim = await _context.Tims
                .FirstOrDefaultAsync(m => m.IdTima == id);
            if (tim == null)
            {
                return NotFound();
            }

            return View(tim);
        }

        [Authorize]
        // GET: Tims/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tims/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTima,Naziv,Bodovi")] Tim tim)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tim);
        }

        [Authorize]
        // GET: Tims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tims == null)
            {
                return NotFound();
            }

            var tim = await _context.Tims.FindAsync(id);
            if (tim == null)
            {
                return NotFound();
            }
            return View(tim);
        }

        // POST: Tims/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTima,Naziv,Bodovi")] Tim tim)
        {
            if (id != tim.IdTima)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimExists(tim.IdTima))
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
            return View(tim);
        }

        [Authorize]
        // GET: Tims/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tims == null)
            {
                return NotFound();
            }

            var tim = await _context.Tims
                .FirstOrDefaultAsync(m => m.IdTima == id);
            if (tim == null)
            {
                return NotFound();
            }

            return View(tim);
        }

        // POST: Tims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tims == null)
            {
                return Problem("Entity set 'FudbalskiTurnirContext.Tims'  is null.");
            }
            // uzimamo tim koji ima prosledjen ID
            var tim = await _context.Tims.FindAsync(id);


            if (tim != null)
            {
                // punimo listu sa rezultatima tima kojeg zelimo obrisati i onda brisemo te rezultate 
                List<Rezultati> listaRezultataSaId = await _context.Rezultatis.Where(p => p.Tim1Id == id || p.Tim2Id == id).ToListAsync();
                _context.Rezultatis.RemoveRange(listaRezultataSaId);
                // punimo listu sa igracima tima kojeg zelimo obrisati i onda brisemo te igrace 
                List<Igrac> igraciTima = await _context.Igracs.Include(i => i.IdTimaNavigation).Where(p => p.IdTima == id).ToListAsync();
                _context.Igracs.RemoveRange(igraciTima);
                // sada brisemo i sam tim
                _context.Tims.Remove(tim);
            }

            
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimExists(int id)
        {
          return (_context.Tims?.Any(e => e.IdTima == id)).GetValueOrDefault();
        }
    }
}
