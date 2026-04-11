using ErrorOr;
using PersonalFinance.Application.DTO;

namespace PersonalFinance.Application.Interfaces;

public interface IExpenseArticleService
{
    Task<ErrorOr<ArticleResponse>> CreateAsync(
        CreateArticleRequest request, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<ArticleResponse>> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<(List<ArticleResponse> Items, int TotalCount)>> GetByCategoryIdAsync(
        Guid categoryId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Success>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ErrorOr<(List<ArticleResponse> Items, int TotalCount)>> GetAllAsync(
        int page, 
        int size, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<ArticleResponse>> UpdateAsync(
        Guid id, 
        UpdateArticleRequest request, 
        CancellationToken cancellationToken = default);
}