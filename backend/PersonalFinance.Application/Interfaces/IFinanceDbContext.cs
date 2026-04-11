using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Interfaces;

public interface IFinanceDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<ExpenseArticle> ExpenseArticles { get; }
    DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
