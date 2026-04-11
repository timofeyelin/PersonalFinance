namespace PersonalFinance.Application.DTO;

public record CreateCategoryRequest(string Name, decimal MonthlyBudget);