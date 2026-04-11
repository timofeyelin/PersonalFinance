using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.DTO;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Services;

public class TransactionService(IFinanceDbContext context) : ITransactionService
{
    private const decimal DailyLimit = 1_000_000m;

    public async Task<ErrorOr<TransactionResponse>> CreateAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var article = await context
            .ExpenseArticles.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.ArticleId, cancellationToken);

        if (article is null)
        {
            return Error.NotFound(
                "Article.NotFound",
                $"Статья расходов с ID '{request.ArticleId}' не найдена."
            );
        }

        var currentTotal =
            await context
                .Transactions.AsNoTracking()
                .Where(t => t.Date == request.Date)
                .SumAsync(t => (decimal?)t.Amount, cancellationToken)
            ?? 0m;

        if (currentTotal + request.Amount > DailyLimit)
        {
            return Error.Validation(
                "Transaction.LimitExceeded",
                "Превышен дневной лимит транзакций в 1 000 000 руб."
            );
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Date = request.Date,
            Amount = request.Amount,
            Comment = request.Comment,
            ArticleId = request.ArticleId,
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync(cancellationToken);

        return new TransactionResponse(
            transaction.Id,
            transaction.Date,
            transaction.Amount,
            transaction.Comment,
            transaction.ArticleId,
            article.Name
        );
    }

    public async Task<ErrorOr<TransactionResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var transaction = await context
            .Transactions.AsNoTracking()
            .Include(t => t.Article)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (transaction is null)
        {
            return Error.NotFound("Transaction.NotFound", "Транзакция не найдена.");
        }

        return new TransactionResponse(
            transaction.Id,
            transaction.Date,
            transaction.Amount,
            transaction.Comment,
            transaction.ArticleId,
            transaction.Article.Name
        );
    }

    public async Task<ErrorOr<(List<TransactionResponse> Items, int TotalCount)>> GetByMonthAsync(
        int year,
        int month,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var startDate = new DateOnly(year, month, 1);
        var endDate = startDate.AddMonths(1);

        var baseQuery = context
            .Transactions.AsNoTracking()
            .Where(t => t.Date >= startDate && t.Date < endDate);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Include(t => t.Article)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var responses = items
            .Select(t => new TransactionResponse(
                t.Id,
                t.Date,
                t.Amount,
                t.Comment,
                t.ArticleId,
                t.Article.Name
            ))
            .ToList();

        return (responses, totalCount);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var transaction = await context.Transactions.FirstOrDefaultAsync(
            t => t.Id == id,
            cancellationToken
        );

        if (transaction is null)
        {
            return Error.NotFound("Transaction.NotFound", "Транзакция не найдена.");
        }

        context.Transactions.Remove(transaction);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
