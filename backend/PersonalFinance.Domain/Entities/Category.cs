namespace PersonalFinance.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal MonthlyBudget { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<ExpenseArticle> Articles { get; set; } = new List<ExpenseArticle>();
}