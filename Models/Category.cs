using System.ComponentModel.DataAnnotations;

namespace BudgetBook.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public TransactionType Type { get; set; }

        public bool IsActive { get; set; } = true;

        public List<Transaction> Transactions { get; set; } = new();
    }
}