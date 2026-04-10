namespace PersonalFinance.Domain.Entities;

public class ExpenseArticle
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public Guid CategoryId { get; set; } 
    public Category? Category { get; set; } 

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}