namespace PersonalFinance.Application.DTO;

public record CategoryResponse(Guid Id, string Name, decimal MonthlyBudget, bool IsActive);