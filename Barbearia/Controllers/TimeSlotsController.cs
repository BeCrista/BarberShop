using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Barbearia.Data;
using Barbearia.Models;
using Microsoft.AspNetCore.Authorization;

namespace Barbearia.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TimeSlotsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeSlotsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TimeSlots
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AvailableTimeSlots.Include(a => a.Barber);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TimeSlots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availableTimeSlot = await _context.AvailableTimeSlots
                .Include(a => a.Barber)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (availableTimeSlot == null)
            {
                return NotFound();
            }

            return View(availableTimeSlot);
        }

        // GET: TimeSlots/Create
        public IActionResult Create()
        {
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "Nome");
            return View();
        }

        // POST: TimeSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BarberId,DataHoraInicio,DataHoraFim,Ocupado")] AvailableTimeSlot availableTimeSlot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(availableTimeSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "Nome", availableTimeSlot.BarberId);
            return View(availableTimeSlot);
        }

        // GET: TimeSlots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availableTimeSlot = await _context.AvailableTimeSlots.FindAsync(id);
            if (availableTimeSlot == null)
            {
                return NotFound();
            }
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "Nome", availableTimeSlot.BarberId);
            return View(availableTimeSlot);
        }

        // POST: TimeSlots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BarberId,DataHoraInicio,DataHoraFim,Ocupado")] AvailableTimeSlot availableTimeSlot)
        {
            if (id != availableTimeSlot.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(availableTimeSlot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvailableTimeSlotExists(availableTimeSlot.Id))
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
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "Nome", availableTimeSlot.BarberId);
            return View(availableTimeSlot);
        }

        // GET: TimeSlots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availableTimeSlot = await _context.AvailableTimeSlots
                .Include(a => a.Barber)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (availableTimeSlot == null)
            {
                return NotFound();
            }

            return View(availableTimeSlot);
        }

        // POST: TimeSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var availableTimeSlot = await _context.AvailableTimeSlots.FindAsync(id);
            if (availableTimeSlot != null)
            {
                _context.AvailableTimeSlots.Remove(availableTimeSlot);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvailableTimeSlotExists(int id)
        {
            return _context.AvailableTimeSlots.Any(e => e.Id == id);
        }
    }
}
