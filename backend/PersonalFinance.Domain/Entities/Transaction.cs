namespace PersonalFinance.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string? Comment { get; set; }
    public string? Emoji { get; set; }
    
    public Guid ArticleId { get; set; }
    public ExpenseArticle? Article { get; set; }
}
