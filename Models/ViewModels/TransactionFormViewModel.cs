using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetBook.Models.ViewModels
{
    public class TransactionFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie einen Betrag ein.")]
        [Range(0.01, 999999999,
            ErrorMessage = "Der Betrag muss größer als 0 sein.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Bitte wählen Sie ein Datum aus.")]
        public DateTime? BookingDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Bitte wählen Sie einen Typ aus.")]
        public TransactionType? Type { get; set; }

        [Required(ErrorMessage = "Bitte wählen Sie eine Kategorie aus.")]
        public int? CategoryId { get; set; }

        [MaxLength(200,
            ErrorMessage = "Die Beschreibung darf maximal 200 Zeichen haben.")]
        public string? Description { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }
}