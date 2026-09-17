using BudgetBook.Data;
using BudgetBook.Models;
using BudgetBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BudgetBook.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================

        // Zeigt nur die Buchungen des angemeldeten Benutzers
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.BookingDate)
                .ToListAsync();

            return View(transactions);
        }

        // =========================
        // CREATE GET
        // =========================

        // Zeigt das Formular für eine neue Buchung
        [HttpGet]
        public IActionResult Create()
        {
            var model = new TransactionFormViewModel
            {
                BookingDate = DateTime.Today,

                Categories = _context.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList()
            };

            return View(model);
        }

        // =========================
        // CREATE POST
        // =========================

        // Speichert eine neue Buchung
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _context.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(model);
            }

            // Gewählte Kategorie prüfen
            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.Id == model.CategoryId &&
                    c.IsActive);

            if (category == null)
            {
                ModelState.AddModelError(
                    "CategoryId",
                    "Die gewählte Kategorie ist ungültig.");

                model.Categories = _context.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(model);
            }

            // Kategorie muss zum Buchungstyp passen
            if (category.Type != model.Type!.Value)
            {
                ModelState.AddModelError(
                    "CategoryId",
                    "Die Kategorie passt nicht zum gewählten Buchungstyp.");

                model.Categories = _context.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(model);
            }

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            // Neue Buchung erstellen
            var transaction = new Transaction
            {
                Amount = model.Amount,
               BookingDate = model.BookingDate!.Value,
                Type = model.Type!.Value,
                CategoryId = model.CategoryId!.Value,
                Description = model.Description,
                UserId = userId!,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================

        // Zeigt die Details einer Buchung
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            // Nur eigene Buchung laden
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.UserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // =========================
        // EDIT GET
        // =========================

        // Zeigt das Formular zum Bearbeiten
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            // Nur eigene Buchung suchen
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.UserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            // Daten ins ViewModel übernehmen
            var model = new TransactionFormViewModel
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                BookingDate = transaction.BookingDate,
                Type = transaction.Type,
                CategoryId = transaction.CategoryId,
                Description = transaction.Description,

                Categories = _context.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList()
            };

            return View(model);
        }

        // =========================
        // EDIT POST
        // =========================

        // Speichert die Änderungen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TransactionFormViewModel model)
        {
            if (model.Id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            // Nur eigene Buchung laden
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t =>
                    t.Id == model.Id &&
                    t.UserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            // Formulardaten prüfen
            if (!ModelState.IsValid)
            {
                model.Categories = _context.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(model);
            }

            // Kategorie prüfen
            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.Id == model.CategoryId &&
                    c.IsActive);

            if (category == null)
            {
                ModelState.AddModelError(
                    "CategoryId",
                    "Die gewählte Kategorie ist ungültig.");

                model.Categories = _context.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(model);
            }

            // Typ und Kategorie müssen zusammenpassen
            if (category.Type != model.Type!.Value)
            {
                ModelState.AddModelError(
                    "CategoryId",
                    "Die Kategorie passt nicht zum gewählten Buchungstyp.");

                model.Categories = _context.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(model);
            }

            // Bestehende Buchung aktualisieren
            transaction.Amount = model.Amount;
           transaction.BookingDate = model.BookingDate!.Value;
            transaction.Type = model.Type!.Value;
            transaction.CategoryId = model.CategoryId!.Value;
            transaction.Description = model.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE GET
        // =========================

        // Zeigt die Löschbestätigung
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            // Nur eigene Buchung suchen
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.UserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // =========================
        // DELETE POST
        // =========================

        // Löscht die Buchung nach der Bestätigung
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            // Nur eigene Buchung suchen
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.UserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            // Buchung löschen
            _context.Transactions.Remove(transaction);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}