namespace PersonalFinance.Application.DTO;

public record CreateArticleRequest(string Name, Guid CategoryId);