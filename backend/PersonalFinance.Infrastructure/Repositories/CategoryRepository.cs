using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Interfaces.Repositories;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure.Repositories;

public class CategoryRepository(FinanceDbContext context) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await context.Categories
            .Include(c => c.Articles)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(List<Category> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize
    )
    {
        var baseQuery = context.Categories.AsQueryable();

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Categories.AnyAsync(c => c.Id == id);
    }

    public async Task<decimal> GetTotalBudgetAsync()
    {
        return await context.Categories.SumAsync(c => (decimal?)c.MonthlyBudget) ?? 0m;
    }

    public void Add(Category category)
    {
        context.Categories.Add(category);
    }

    public void Update(Category category)
    {
        context.Categories.Update(category);
    }

    public void Delete(Category category)
    {
        context.Categories.Remove(category);
    }
}