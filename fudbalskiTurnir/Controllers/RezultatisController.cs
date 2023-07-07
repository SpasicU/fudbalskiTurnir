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
    public class RezultatisController : Controller
    {
        private readonly FudbalskiTurnirContext _context;

        public RezultatisController(FudbalskiTurnirContext context)
        {
            _context = context;
        }

        // GET: Rezultatis
        public async Task<IActionResult> Index()
        {
            ViewBag.Rezultati = (string)TempData["Message"];
            var fudbalskiTurnirContext = _context.Rezultatis.Include(r => r.Tim1).Include(r => r.Tim2);
            return View(await fudbalskiTurnirContext.ToListAsync());
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GenerisiParove()
        {
            
            // punimo timove u listu
            var timovi = _context.Tims.ToList();
            var igraci = _context.Igracs.ToList();

            //punimo listu nasumicno
            var random = new Random();
            var randomTim = timovi.OrderBy(t => random.Next()).ToList();

            var paroviTima = new List<(Tim tim1, Tim tim2)>();

            foreach (var tim in timovi)
            {
                bool imaIgrace = igraci.Any(i => i.IdTima.Equals(tim.IdTima));
                if (!imaIgrace)
                {
                    TempData["Message"] = "Neuspeh! Tim: '" + tim.Naziv + "' nema nijednog igraca. Ubacite barem jednog igraca ili obrisite tim da bi igrali turnir!";
                    return RedirectToAction("Index", "Rezultatis");
                }
            }

            // Dve for petlje da bi uzeli dva razlicita tima
            for (int i = 0; i < randomTim.Count - 1; i++)
            {
                for (int j = i + 1; j < randomTim.Count; j++)
                {
                    var tim1 = randomTim[i];
                    var tim2 = randomTim[j];

                    // Da li su ova dva tima igrala utakmicu?
                    bool igraliPre = _context.Rezultatis.Any(r =>
                        ((r.Tim1Id == tim1.IdTima && r.Tim2Id == tim2.IdTima) ||
                        (r.Tim1Id == tim2.IdTima && r.Tim2Id == tim1.IdTima))
                    );

                    if (!igraliPre)
                    {
                        paroviTima.Add((tim1, tim2));
                    }
                }
            }

            // Uzimamo svaki par i upisujemo ga u bazu da bi ga mogli ispisati, odnosno Dbcontext
            foreach (var (timJedan, timDva) in paroviTima)
            {
                int tim1golovi = random.Next(0, 6);
                int maxGolovi = 5 - tim1golovi;
                int tim2golovi = random.Next(0, maxGolovi + 1);

                //S obzirom da je kod za dodavanje poena mali, upisao sam ga ovde umesto pravljenja nove void funkcije
                if (tim1golovi > tim2golovi)
                {
                    timJedan.Bodovi += 3; // Tim1 je pobednik
                }
                else if (tim1golovi == tim2golovi)
                {
                    timJedan.Bodovi += 1; // Nereseno
                    timDva.Bodovi += 1;
                }
                else
                {
                    timDva.Bodovi += 3; // Tim2 je pobednik
                }

                Rezultati rezultati = new Rezultati();
                rezultati.Tim1Id = timJedan.IdTima;
                rezultati.Tim2Id = timDva.IdTima;
                rezultati.Tim1Golovi = tim1golovi;
                rezultati.Tim2Golovi = tim2golovi;

                _context.Add(rezultati);
                await _context.SaveChangesAsync();
            }


            ViewData["Tim1Id"] = new SelectList(_context.Tims, "IdTima", "Naziv");
            ViewData["Tim2Id"] = new SelectList(_context.Tims, "IdTima", "Naziv");
            return RedirectToAction("Index", "Rezultatis");
            //return View();
        }



        // GET: Rezultatis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rezultatis == null)
            {
                return NotFound();
            }

            var rezultati = await _context.Rezultatis
                .Include(r => r.Tim1)
                .Include(r => r.Tim2)
                .FirstOrDefaultAsync(m => m.IdRez == id);
            if (rezultati == null)
            {
                return NotFound();
            }

            return View(rezultati);
        }

        [Authorize]
        // GET: Rezultatis/Create
        public IActionResult Create()
        {
            ViewData["Tim1Id"] = new SelectList(_context.Tims, "IdTima", "Naziv");
            ViewData["Tim2Id"] = new SelectList(_context.Tims, "IdTima", "Naziv");
            return View();
        }

        // POST: Rezultatis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRez,Tim1Id,Tim2Id,Tim1Golovi,Tim2Golovi")] Rezultati rezultati)
        {
            //Ovde stavljamo ogranicenje na max 5 golova po utakmici i upisujemo ga u ViewBag
            // koji ispisujemo u Views/Rezultatis/Create kada se okine if petlja
            int max = rezultati.Tim1Golovi + rezultati.Tim2Golovi;
            if(max > 5)
            {
                ViewBag.Poruka = "Zbir golova na utakmici mora biti manji od 5!";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _context.Add(rezultati);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["Tim1Id"] = new SelectList(_context.Tims, "IdTima", "Naziv", rezultati.Tim1Id);
            ViewData["Tim2Id"] = new SelectList(_context.Tims, "IdTima", "Naziv", rezultati.Tim2Id);
            return View(rezultati);
        }

        [Authorize]
        // GET: Rezultatis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rezultatis == null)
            {
                return NotFound();
            }

            var rezultati = await _context.Rezultatis.FindAsync(id);
            if (rezultati == null)
            {
                return NotFound();
            }
            ViewData["Tim1Id"] = new SelectList(_context.Tims, "IdTima", "Naziv", rezultati.Tim1Id);
            ViewData["Tim2Id"] = new SelectList(_context.Tims, "IdTima", "Naziv", rezultati.Tim2Id);
            return View(rezultati);
        }

        // POST: Rezultatis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRez,Tim1Id,Tim2Id,Tim1Golovi,Tim2Golovi")] Rezultati rezultati)
        {
            if (id != rezultati.IdRez)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rezultati);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RezultatiExists(rezultati.IdRez))
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
            ViewData["Tim1Id"] = new SelectList(_context.Tims, "IdTima", "Naziv", rezultati.Tim1Id);
            ViewData["Tim2Id"] = new SelectList(_context.Tims, "IdTima", "Naziv", rezultati.Tim2Id);
            return View(rezultati);
        }

        [Authorize]
        // GET: Rezultatis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rezultatis == null)
            {
                return NotFound();
            }

            var rezultati = await _context.Rezultatis
                .Include(r => r.Tim1)
                .Include(r => r.Tim2)
                .FirstOrDefaultAsync(m => m.IdRez == id);
            if (rezultati == null)
            {
                return NotFound();
            }

            return View(rezultati);
        }
        // POST: Rezultatis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rezultatis == null)
            {
                return Problem("Entity set 'FudbalskiTurnirContext.Rezultatis'  is null.");
            }
            var rezultati = await _context.Rezultatis.FindAsync(id);
            var tim1 = rezultati.Tim1Id;
            var tim2 = rezultati.Tim2Id;
  
            var timJedan = _context.Tims.Where(i => i.IdTima == tim1).SingleOrDefault();
            var timDva = _context.Tims.Where(i => i.IdTima == tim2).SingleOrDefault();

            if (rezultati != null)
            {
                if (rezultati.Tim1Golovi > rezultati.Tim2Golovi)
                {
                    timJedan.Bodovi -= 3; 
                }
                else if (rezultati.Tim1Golovi == rezultati.Tim2Golovi)
                {
                    timJedan.Bodovi -= 1; 
                    timDva.Bodovi -= 1;
                }
                else
                {
                    timDva.Bodovi -= 3; 
                }

                _context.Rezultatis.Remove(rezultati);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool RezultatiExists(int id)
        {
          return (_context.Rezultatis?.Any(e => e.IdRez == id)).GetValueOrDefault();
        }
    }
}
