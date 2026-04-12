using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Api.Extensions;
using PersonalFinance.Application.DTO;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var result = await categoryService.CreateAsync(request, ct);
        
        return result.Match(
            category => CreatedAtAction(nameof(GetById), new { id = category.Id }, category),
            this.Problem);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await categoryService.GetByIdAsync(id, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery, Range(1, int.MaxValue, ErrorMessage = "Номер страницы должен быть больше 0.")] int page = 1, 
        [FromQuery, Range(1, 100, ErrorMessage = "Размер страницы должен быть от 1 до 100.")] int size = 10, 
        CancellationToken ct = default)
    {
        var result = await categoryService.GetAllAsync(page, size, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpGet("total-budget")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTotalBudget(CancellationToken ct)
    {
        var total = await categoryService.GetTotalBudgetAsync(ct);
        return Ok(new { TotalBudget = total });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id, 
        [FromBody] UpdateCategoryRequest request, 
        CancellationToken ct)
    {
        var result = await categoryService.UpdateAsync(id, request, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await categoryService.DeleteAsync(id, ct);
        return result.Match(_ => NoContent(), this.Problem);
    }
}
