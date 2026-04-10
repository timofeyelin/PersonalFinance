using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces.Repositories;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure.Repositories;

public class ExpenseArticleRepository(FinanceDbContext context) : IExpenseArticleRepository
{
    public async Task<ExpenseArticle?> GetByIdAsync(Guid id)
    {
        return await context.ExpenseArticles.Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<(List<ExpenseArticle> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize
    )
    {
        var baseQuery = context.ExpenseArticles.AsQueryable();

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .Include(a => a.Category)
            .OrderBy(a => a.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<ExpenseArticle> Items, int TotalCount)> GetByCategoryIdAsync(
        Guid categoryId,
        int pageNumber,
        int pageSize
    )
    {
        var baseQuery = context.ExpenseArticles.Where(a => a.CategoryId == categoryId);

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .Include(a => a.Category)
            .OrderBy(a => a.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.ExpenseArticles.AnyAsync(a => a.Id == id);
    }

    public void Add(ExpenseArticle article)
    {
        context.ExpenseArticles.Add(article);
    }

    public void Update(ExpenseArticle article)
    {
        context.ExpenseArticles.Update(article);
    }

    public void Delete(ExpenseArticle article)
    {
        context.ExpenseArticles.Remove(article);
    }
}