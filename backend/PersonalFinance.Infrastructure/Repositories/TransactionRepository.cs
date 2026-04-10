using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces.Repositories;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure.Repositories;

public class TransactionRepository(FinanceDbContext context) : ITransactionRepository
{
    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await context.Transactions
            .Include(t => t.Article)
            .ThenInclude(a => a!.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<(List<Transaction> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize
    )
    {
        var baseQuery = context.Transactions.AsQueryable();

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .Include(t => t.Article)
            .ThenInclude(a => a!.Category)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<Transaction> Items, int TotalCount)> GetByDateAsync(
        DateOnly date,
        int pageNumber,
        int pageSize
    )
    {
        var baseQuery = context.Transactions.Where(t => t.Date == date);

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .Include(t => t.Article)
            .ThenInclude(a => a!.Category)
            .OrderByDescending(t => t.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<Transaction> Items, int TotalCount)> GetByMonthAsync(
        int year,
        int month,
        int pageNumber,
        int pageSize
    )
    {
        var startDate = new DateOnly(year, month, 1);
        var endDate = startDate.AddMonths(1);

        var baseQuery = context.Transactions.Where(t => t.Date >= startDate && t.Date < endDate);

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .Include(t => t.Article)
            .ThenInclude(a => a!.Category)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<decimal> GetTotalAmountByDateAsync(DateOnly date)
    {
        return await context.Transactions.Where(t => t.Date == date).SumAsync(t => (decimal?)t.Amount)
               ?? 0m;
    }

    public void Add(Transaction transaction)
    {
        context.Transactions.Add(transaction);
    }

    public void Update(Transaction transaction)
    {
        context.Transactions.Update(transaction);
    }

    public void Delete(Transaction transaction)
    {
        context.Transactions.Remove(transaction);
    }
}