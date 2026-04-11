namespace PersonalFinance.Application.DTO;

public record UpdateArticleRequest(string Name, Guid CategoryId, bool IsActive);