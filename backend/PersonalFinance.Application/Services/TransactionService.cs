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
        CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0) return Error.Validation("Transaction.Amount", "Сумма транзакции должна быть положительной.");

        var article = await context.ExpenseArticles
            .AsNoTracking()
            .Include(a => a.Category)
            .FirstOrDefaultAsync(a => a.Id == request.ArticleId, cancellationToken);

        if (article is null) return Error.NotFound("Article.NotFound", "Статья не найдена.");
        if (!article.IsActive || !article.Category.IsActive) return Error.Validation("Article.Inactive", "Статья или категория неактивны.");

        await using var dbTransaction = await context.BeginTransactionAsync(cancellationToken);

        try
        {
            var currentTotal = await context.Transactions
                .FromSqlInterpolated($"SELECT * FROM Transactions WITH (UPDLOCK, HOLDLOCK) WHERE [Date] = {request.Date}")
                .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;

            if (currentTotal + request.Amount > DailyLimit)
            {
                return Error.Validation("Transaction.LimitExceeded", "Превышен дневной лимит транзакций в 1 000 000 руб.");
            }

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(), Date = request.Date, Amount = request.Amount,
                Comment = request.Comment, ArticleId = request.ArticleId, Emoji = request.Emoji,
            };

            context.Transactions.Add(transaction);
            await context.SaveChangesAsync(cancellationToken);
            
            await dbTransaction.CommitAsync(cancellationToken);

            return new TransactionResponse(transaction.Id, transaction.Date, transaction.Amount, transaction.Comment, transaction.ArticleId, article.Name, transaction.Emoji);
        }
        catch (Exception)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw;
        }
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
            transaction.Article.Name,
            transaction.Emoji
        );
    }

    public async Task<ErrorOr<PagedResponse<TransactionResponse>>> GetByMonthAsync(
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
                t.Article.Name,
                t.Emoji
            ))
            .ToList();

        return new PagedResponse<TransactionResponse>(responses, totalCount);
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

    public async Task<ErrorOr<PagedResponse<TransactionResponse>>> GetAllAsync(
        int page,
        int size,
        CancellationToken cancellationToken = default
    )
    {
        var baseQuery = context.Transactions.AsNoTracking();
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Include(t => t.Article)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var responses = items
            .Select(t => new TransactionResponse(
                t.Id,
                t.Date,
                t.Amount,
                t.Comment,
                t.ArticleId,
                t.Article.Name,
                t.Emoji
            ))
            .ToList();

        return new PagedResponse<TransactionResponse>(responses, totalCount);
    }

    public async Task<ErrorOr<PagedResponse<TransactionResponse>>> GetByDateAsync(
        DateOnly date,
        int page,
        int size,
        CancellationToken cancellationToken = default
    )
    {
        var baseQuery = context.Transactions.AsNoTracking().Where(t => t.Date == date);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Include(t => t.Article)
            .OrderByDescending(t => t.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var responses = items
            .Select(t => new TransactionResponse(
                t.Id,
                t.Date,
                t.Amount,
                t.Comment,
                t.ArticleId,
                t.Article.Name,
                t.Emoji
            ))
            .ToList();

        return new PagedResponse<TransactionResponse>(responses, totalCount);
    }

    public async Task<ErrorOr<TransactionResponse>> UpdateAsync(
        Guid id,
        UpdateTransactionRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (request.Amount <= 0)
        {
            return Error.Validation(
                "Transaction.Amount",
                "Сумма транзакции должна быть положительной."
            );
        }

        var transaction = await context
            .Transactions.Include(t => t.Article)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (transaction is null)
        {
            return Error.NotFound("Transaction.NotFound", "Транзакция не найдена.");
        }

        var currentTotalForDate =
            await context
                .Transactions.AsNoTracking()
                .Where(t => t.Date == request.Date && t.Id != id)
                .SumAsync(t => (decimal?)t.Amount, cancellationToken)
            ?? 0m;

        if (currentTotalForDate + request.Amount > DailyLimit)
        {
            return Error.Validation(
                "Transaction.LimitExceeded",
                "Превышен дневной лимит транзакций в 1 000 000 руб."
            );
        }

        var articleName = transaction.Article.Name;

        if (transaction.ArticleId != request.ArticleId)
        {
            if (!transaction.Article.IsActive)
            {
                return Error.Validation(
                    "Transaction.ArticleChangeForbidden",
                    "Нельзя менять статью расходов у транзакции, если текущая статья стала неактивной."
                );
            }

            var newArticle = await context
                .ExpenseArticles.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == request.ArticleId, cancellationToken);

            if (newArticle is null)
            {
                return Error.NotFound("Article.NotFound", "Новая статья расходов не найдена.");
            }

            if (!newArticle.IsActive)
            {
                return Error.Validation(
                    "Article.Inactive",
                    "Нельзя перенести транзакцию на неактивную статью."
                );
            }

            transaction.ArticleId = request.ArticleId;
            articleName = newArticle.Name;
        }

        transaction.Date = request.Date;
        transaction.Amount = request.Amount;
        transaction.Comment = request.Comment;
        transaction.Emoji = request.Emoji;

        await context.SaveChangesAsync(cancellationToken);

        return new TransactionResponse(
            transaction.Id,
            transaction.Date,
            transaction.Amount,
            transaction.Comment,
            transaction.ArticleId,
            articleName,
            transaction.Emoji
        );
    }

    public async Task<ErrorOr<DayStatusResponse>> GetDayStatusAsync(
        DateOnly date,
        CancellationToken cancellationToken = default
    )
    {
        var total =
            await context
                .Transactions.AsNoTracking()
                .Where(t => t.Date == date)
                .SumAsync(t => (decimal?)t.Amount, cancellationToken)
            ?? 0m;

        return total switch
        {
            < 500 => new DayStatusResponse(date, total, "Green", "Экономный день"),
            <= 2_000 => new DayStatusResponse(date, total, "Yellow", "Обычный уровень трат"),
            _ => new DayStatusResponse(date, total, "Red", "Высокие расходы"),
        };
    }
}
