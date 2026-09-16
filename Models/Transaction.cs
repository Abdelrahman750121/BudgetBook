using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetBook.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, 999999999)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;
        
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }
}

