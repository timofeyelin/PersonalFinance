namespace PersonalFinance.Application.DTO;

public record ArticleResponse(Guid Id, string Name, Guid CategoryId, string CategoryName, bool IsActive);