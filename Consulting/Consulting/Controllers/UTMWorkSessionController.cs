using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Consulting.Models;
using Microsoft.AspNetCore.Http;

namespace Consulting.Controllers
{
    public class UTMWorkSessionController : Controller
    {
        private readonly ConsultingContext _context;

        public UTMWorkSessionController(ConsultingContext context)
        {
            _context = context;
        }

        // GET: UTMWorkSession
        public async Task<IActionResult> Index(string ContractId)
        {
            if (!string.IsNullOrEmpty(ContractId))
            {
                Response.Cookies.Append("ContractId", ContractId);
                HttpContext.Session.SetString("ContractId", ContractId);
            }
            else if (Request.Query[ContractId].Any())
            {
                ContractId = Request.Query["ContractId"].ToString();
                Response.Cookies.Append("ContractId", ContractId);
                HttpContext.Session.SetString("ContractId", ContractId);
            }
            else if (Request.Cookies["ContractId"] != null)
            {
                ContractId = Request.Cookies["ContractId"].ToString();
            }
            else if (HttpContext.Session.GetString("ContractId") != null)
            {
                ContractId = HttpContext.Session.GetString("ContractId");
            }
            else
            {
                TempData["message"] = "Please select Contract";
                return RedirectToAction("Index", "Contract");
            }
            var consultingContext = _context.WorkSession.Include(w => w.Consultant).Include(w => w.Contract);
            return View(await consultingContext.ToListAsync());
        }

        // GET: UTMWorkSession/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSession = await _context.WorkSession
                .Include(w => w.Consultant)
                .Include(w => w.Contract)
                .FirstOrDefaultAsync(m => m.WorkSessionId == id);
            if (workSession == null)
            {
                return NotFound();
            }

            return View(workSession);
        }

        // GET: UTMWorkSession/Create
        public IActionResult Create()
        {
            ViewData["ConsultantId"] = new SelectList(_context.Consultant, "ConsultantId", "FirstName");
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Name");
            return View();
        }

        // POST: UTMWorkSession/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkSessionId,ContractId,DateWorked,ConsultantId,HoursWorked,WorkDescription,HourlyRate,ProvincialTax,TotalChargeBeforeTax")] WorkSession workSession)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsultantId"] = new SelectList(_context.Consultant, "ConsultantId", "FirstName", workSession.ConsultantId);
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Name", workSession.ContractId);
            return View(workSession);
        }

        // GET: UTMWorkSession/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSession = await _context.WorkSession.FindAsync(id);
            if (workSession == null)
            {
                return NotFound();
            }
            ViewData["ConsultantId"] = new SelectList(_context.Consultant, "ConsultantId", "FirstName", workSession.ConsultantId);
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Name", workSession.ContractId);
            return View(workSession);
        }

        // POST: UTMWorkSession/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkSessionId,ContractId,DateWorked,ConsultantId,HoursWorked,WorkDescription,HourlyRate,ProvincialTax,TotalChargeBeforeTax")] WorkSession workSession)
        {
            if (id != workSession.WorkSessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkSessionExists(workSession.WorkSessionId))
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
            ViewData["ConsultantId"] = new SelectList(_context.Consultant, "ConsultantId", "FirstName", workSession.ConsultantId);
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Name", workSession.ContractId);
            return View(workSession);
        }

        // GET: UTMWorkSession/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSession = await _context.WorkSession
                .Include(w => w.Consultant)
                .Include(w => w.Contract)
                .FirstOrDefaultAsync(m => m.WorkSessionId == id);
            if (workSession == null)
            {
                return NotFound();
            }

            return View(workSession);
        }

        // POST: UTMWorkSession/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workSession = await _context.WorkSession.FindAsync(id);
            _context.WorkSession.Remove(workSession);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkSessionExists(int id)
        {
            return _context.WorkSession.Any(e => e.WorkSessionId == id);
        }
    }
}
