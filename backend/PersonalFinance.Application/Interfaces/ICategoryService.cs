using ErrorOr;
using PersonalFinance.Application.DTO;

namespace PersonalFinance.Application.Interfaces;

public interface ICategoryService
{
    Task<ErrorOr<CategoryResponse>> CreateAsync(
        CreateCategoryRequest request, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<CategoryResponse>> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<PagedResponse<CategoryResponse>>> GetAllAsync(
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalBudgetAsync(CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ErrorOr<CategoryResponse>> UpdateAsync(
        Guid id, 
        UpdateCategoryRequest request, 
        CancellationToken cancellationToken = default);
}