using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<(List<Category> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task<bool> ExistsAsync(Guid id);
    
    Task<decimal> GetTotalBudgetAsync();
    
    void Add(Category category);
    void Update(Category category);
    void Delete(Category category);
}