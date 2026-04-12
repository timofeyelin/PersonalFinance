using ErrorOr;
using PersonalFinance.Application.DTO;

namespace PersonalFinance.Application.Interfaces;

public interface ITransactionService
{
    Task<ErrorOr<TransactionResponse>> CreateAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default
    );

    Task<ErrorOr<TransactionResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<ErrorOr<PagedResponse<TransactionResponse>>> GetByMonthAsync(
        int year,
        int month,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    Task<ErrorOr<Success>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<ErrorOr<PagedResponse<TransactionResponse>>> GetAllAsync(
        int page, 
        int size, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<PagedResponse<TransactionResponse>>> GetByDateAsync(
        DateOnly date, 
        int page, 
        int size, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<TransactionResponse>> UpdateAsync(
        Guid id, 
        UpdateTransactionRequest request, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<DayStatusResponse>> GetDayStatusAsync(
        DateOnly date, 
        CancellationToken cancellationToken = default);
}