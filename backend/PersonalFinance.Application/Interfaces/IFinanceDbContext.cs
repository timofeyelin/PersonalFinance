using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Interfaces;

public interface IFinanceDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<ExpenseArticle> ExpenseArticles { get; }
    DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
