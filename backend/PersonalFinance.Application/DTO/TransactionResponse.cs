namespace PersonalFinance.Application.DTO;

public record TransactionResponse(
    Guid Id,
    DateOnly Date,
    decimal Amount,
    string? Comment,
    Guid ArticleId,
    string ArticleName,
    string? Emoji
    
);
