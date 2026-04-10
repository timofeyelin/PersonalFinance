using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    
    Task<(List<Transaction> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task<(List<Transaction> Items, int TotalCount)> GetByDateAsync(DateOnly date, int pageNumber, int pageSize);
    Task<(List<Transaction> Items, int TotalCount)> GetByMonthAsync(int year, int month, int pageNumber, int pageSize);
    
    Task<decimal> GetTotalAmountByDateAsync(DateOnly date);

    void Add(Transaction transaction);
    void Update(Transaction transaction);
    void Delete(Transaction transaction);
}