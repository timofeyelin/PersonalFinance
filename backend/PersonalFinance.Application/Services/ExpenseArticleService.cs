using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.DTO;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Services;

public class ExpenseArticleService(IFinanceDbContext context) : IExpenseArticleService
{
    public async Task<ErrorOr<ArticleResponse>> CreateAsync(
        CreateArticleRequest request, 
        CancellationToken cancellationToken = default)
    {
        var category = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Error.NotFound("Category.NotFound", "Указанная категория не найдена.");
        }

        var article = new ExpenseArticle
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CategoryId = request.CategoryId,
            IsActive = true
        };

        context.ExpenseArticles.Add(article);
        await context.SaveChangesAsync(cancellationToken);

        return new ArticleResponse(article.Id, article.Name, article.CategoryId, category.Name, article.IsActive);
    }

    public async Task<ErrorOr<ArticleResponse>> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var article = await context.ExpenseArticles
            .AsNoTracking()
            .Include(a => a.Category)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (article is null) return Error.NotFound("Article.NotFound", "Статья расходов не найдена.");

        return new ArticleResponse(article.Id, article.Name, article.CategoryId, article.Category.Name, article.IsActive);
    }

    public async Task<ErrorOr<PagedResponse<ArticleResponse>>> GetByCategoryIdAsync(
        Guid categoryId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var baseQuery = context.ExpenseArticles
            .AsNoTracking()
            .Where(a => a.CategoryId == categoryId);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Include(a => a.Category)
            .OrderBy(a => a.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var responses = items.Select(a => 
            new ArticleResponse(a.Id, a.Name, a.CategoryId, a.Category.Name, a.IsActive)).ToList();
        
        return new PagedResponse<ArticleResponse>(responses, totalCount);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await context.ExpenseArticles
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (article is null) return Error.NotFound("Article.NotFound", "Статья расходов не найдена.");
        
        var hasTransactions = await context.Transactions
            .AnyAsync(t => t.ArticleId == id, cancellationToken);

        if (hasTransactions)
        {
            return Error.Conflict("Article.HasTransactions", 
                "Невозможно удалить статью, так как по ней уже проведены транзакции.");
        }

        context.ExpenseArticles.Remove(article);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<PagedResponse<ArticleResponse>>> GetAllAsync(
        int page, 
        int size, 
        CancellationToken cancellationToken = default)
    {
        var baseQuery = context.ExpenseArticles.AsNoTracking();
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Include(a => a.Category)
            .OrderBy(a => a.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var responses = items.Select(a => 
            new ArticleResponse(a.Id, a.Name, a.CategoryId, a.Category.Name, a.IsActive)).ToList();

        return new PagedResponse<ArticleResponse>(responses, totalCount);
    }

    public async Task<ErrorOr<ArticleResponse>> UpdateAsync(
        Guid id, 
        UpdateArticleRequest request, 
        CancellationToken cancellationToken = default)
    {
        var article = await context.ExpenseArticles
            .Include(a => a.Category)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (article is null) return Error.NotFound("Article.NotFound", "Статья расходов не найдена.");

        string categoryName = article.Category.Name;

        if (article.CategoryId != request.CategoryId)
        {
            var newCategory = await context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

            if (newCategory is null) return Error.NotFound("Category.NotFound", "Новая категория не найдена.");

            article.CategoryId = request.CategoryId;
            categoryName = newCategory.Name;
        }

        article.Name = request.Name;
        article.IsActive = request.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return new ArticleResponse(article.Id, article.Name, article.CategoryId, categoryName, article.IsActive);
    }
}