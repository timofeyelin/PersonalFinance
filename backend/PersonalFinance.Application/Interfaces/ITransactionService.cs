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

    Task<ErrorOr<(List<TransactionResponse> Items, int TotalCount)>> GetByMonthAsync(
        int year,
        int month,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    Task<ErrorOr<Success>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
