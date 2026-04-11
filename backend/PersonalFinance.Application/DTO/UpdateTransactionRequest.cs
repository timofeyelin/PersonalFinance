namespace PersonalFinance.Application.DTO;

public record UpdateTransactionRequest(DateOnly Date, decimal Amount, string? Comment, Guid ArticleId);