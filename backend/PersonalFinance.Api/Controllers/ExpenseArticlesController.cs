using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Api.Extensions;
using PersonalFinance.Application.DTO;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpenseArticlesController(IExpenseArticleService expenseArticleService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ArticleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateArticleRequest request, CancellationToken ct)
    {
        var result = await expenseArticleService.CreateAsync(request, ct);
        
        return result.Match(
            article => CreatedAtAction(nameof(GetById), new { id = article.Id }, article),
            this.Problem);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ArticleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await expenseArticleService.GetByIdAsync(id, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ArticleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery, Range(1, int.MaxValue, ErrorMessage = "Номер страницы должен быть больше 0.")] int page = 1, 
        [FromQuery, Range(1, 100, ErrorMessage = "Размер страницы должен быть от 1 до 100.")] int size = 10, 
        CancellationToken ct = default)
    {
        var result = await expenseArticleService.GetAllAsync(page, size, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpGet("by-category/{categoryId:guid}")]
    [ProducesResponseType(typeof(PagedResponse<ArticleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByCategoryId(
        Guid categoryId,
        [FromQuery, Range(1, int.MaxValue, ErrorMessage = "Номер страницы должен быть больше 0.")] int page = 1, 
        [FromQuery, Range(1, 100, ErrorMessage = "Размер страницы должен быть от 1 до 100.")] int size = 10, 
        CancellationToken ct = default)
    {
        var result = await expenseArticleService.GetByCategoryIdAsync(categoryId, page, size, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ArticleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id, 
        [FromBody] UpdateArticleRequest request, 
        CancellationToken ct)
    {
        var result = await expenseArticleService.UpdateAsync(id, request, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await expenseArticleService.DeleteAsync(id, ct);
        return result.Match(_ => NoContent(), this.Problem);
    }
}
