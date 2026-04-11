using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.DTO;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Services;

public class CategoryService(IFinanceDbContext context) : ICategoryService
{
    public async Task<ErrorOr<CategoryResponse>> CreateAsync(
        CreateCategoryRequest request, 
        CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            MonthlyBudget = request.MonthlyBudget,
            IsActive = true
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);

        return new CategoryResponse(category.Id, category.Name, category.MonthlyBudget, category.IsActive);
    }

    public async Task<ErrorOr<CategoryResponse>> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var category = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category is null) return Error.NotFound("Category.NotFound", "Категория не найдена.");

        return new CategoryResponse(category.Id, category.Name, category.MonthlyBudget, category.IsActive);
    }

    public async Task<ErrorOr<(List<CategoryResponse> Items, int TotalCount)>> GetAllAsync(
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var baseQuery = context.Categories.AsNoTracking();

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var responses = items.Select(c => 
            new CategoryResponse(c.Id, c.Name, c.MonthlyBudget, c.IsActive)).ToList();

        return (responses, totalCount);
    }

    public async Task<decimal> GetTotalBudgetAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .AsNoTracking()
            .SumAsync(c => (decimal?)c.MonthlyBudget, cancellationToken) ?? 0m;
    }

    public async Task<ErrorOr<Success>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category is null) return Error.NotFound("Category.NotFound", "Категория не найдена.");
        
        var hasArticles = await context.ExpenseArticles
            .AnyAsync(a => a.CategoryId == id, cancellationToken);

        if (hasArticles)
        {
            return Error.Conflict("Category.HasArticles", 
                "Невозможно удалить категорию, так как в ней есть статьи расходов. Сначала удалите их.");
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<CategoryResponse>> UpdateAsync(
        Guid id, 
        UpdateCategoryRequest request, 
        CancellationToken cancellationToken = default)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category is null) return Error.NotFound("Category.NotFound", "Категория не найдена.");

        category.Name = request.Name;
        category.MonthlyBudget = request.MonthlyBudget;
        category.IsActive = request.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return new CategoryResponse(category.Id, category.Name, category.MonthlyBudget, category.IsActive);
    }
}