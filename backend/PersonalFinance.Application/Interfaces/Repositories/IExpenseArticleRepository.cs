using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Interfaces.Repositories;

public interface IExpenseArticleRepository
{
    Task<ExpenseArticle?> GetByIdAsync(Guid id);
    
    Task<(List<ExpenseArticle> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task<(List<ExpenseArticle> Items, int TotalCount)> GetByCategoryIdAsync(Guid categoryId, int pageNumber, int pageSize);
    
    Task<bool> ExistsAsync(Guid id);

    void Add(ExpenseArticle article);
    void Update(ExpenseArticle article);
    void Delete(ExpenseArticle article);
}