namespace PersonalFinance.Application.DTO;

public record CreateTransactionRequest(
    DateOnly Date,
    decimal Amount,
    string? Comment,
    Guid ArticleId
);
