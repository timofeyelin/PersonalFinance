namespace PersonalFinance.Application.DTO;

public record UpdateCategoryRequest(string Name, decimal MonthlyBudget, bool IsActive);