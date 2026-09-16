using BudgetBook.Models;
using Microsoft.AspNetCore.Identity;

namespace BudgetBook.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            // Test-Zugangsdaten aus User Secrets holen
            var email = configuration["SeedUser:Email"];
            var password = configuration["SeedUser:Password"];

            // Falls keine Secrets vorhanden sind, nichts erstellen
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return;
            }

            // Testbenutzer suchen
            var user = await userManager.FindByEmailAsync(email);

            // Falls er noch nicht existiert, erstellen
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    return;
                }
            }

            // Kategorien erstellen, falls noch keine vorhanden sind
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category
                    {
                        Name = "Gehalt",
                        Type = TransactionType.INCOME,
                        IsActive = true
                    },

                    new Category
                    {
                        Name = "Sonstige Einnahmen",
                        Type = TransactionType.INCOME,
                        IsActive = true
                    },

                    new Category
                    {
                        Name = "Lebensmittel",
                        Type = TransactionType.EXPENSE,
                        IsActive = true
                    },

                    new Category
                    {
                        Name = "Freizeit",
                        Type = TransactionType.EXPENSE,
                        IsActive = true
                    }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Testbuchungen erstellen, falls noch keine vorhanden sind
            if (!context.Transactions.Any())
            {
                var gehalt = context.Categories
                    .First(c => c.Name == "Gehalt");

                var lebensmittel = context.Categories
                    .First(c => c.Name == "Lebensmittel");

                var transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        Amount = 1800.00m,
                        BookingDate = DateTime.Today,
                        Type = TransactionType.INCOME,
                        Description = "Monatsgehalt",
                        CategoryId = gehalt.Id,
                        UserId = user.Id
                    },

                    new Transaction
                    {
                        Amount = 45.90m,
                        BookingDate = DateTime.Today,
                        Type = TransactionType.EXPENSE,
                        Description = "Supermarkt",
                        CategoryId = lebensmittel.Id,
                        UserId = user.Id
                    }
                };

                context.Transactions.AddRange(transactions);
                await context.SaveChangesAsync();
            }
        }
    }
}