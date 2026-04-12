using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Api.Extensions;
using PersonalFinance.Application.DTO;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request, CancellationToken ct)
    {
        var result = await transactionService.CreateAsync(request, ct);
        
        return result.Match(
            transaction => CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction),
            this.Problem);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await transactionService.GetByIdAsync(id, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<TransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery, Range(1, int.MaxValue, ErrorMessage = "Номер страницы должен быть больше 0.")] int page = 1, 
        [FromQuery, Range(1, 100, ErrorMessage = "Размер страницы должен быть от 1 до 100.")] int size = 10, 
        CancellationToken ct = default)
    {
        var result = await transactionService.GetAllAsync(page, size, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpGet("by-month")]
    [ProducesResponseType(typeof(PagedResponse<TransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByMonth(
        [FromQuery, Range(2000, 2100, ErrorMessage = "Год должен быть в диапазоне от 2000 до 2100.")] int year, 
        [FromQuery, Range(1, 12, ErrorMessage = "Месяц должен быть от 1 до 12.")] int month, 
        [FromQuery, Range(1, int.MaxValue, ErrorMessage = "Номер страницы должен быть больше 0.")] int page = 1, 
        [FromQuery, Range(1, 100, ErrorMessage = "Размер страницы должен быть от 1 до 100.")] int size = 10, 
        CancellationToken ct = default)
    {
        var result = await transactionService.GetByMonthAsync(year, month, page, size, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpGet("day-status")]
    [ProducesResponseType(typeof(DayStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDayStatus([FromQuery] DateOnly date, CancellationToken ct)
    {
        var result = await transactionService.GetDayStatusAsync(date, ct);
        return result.Match(Ok, this.Problem);
    }
    
    [HttpGet("by-date")]
    [ProducesResponseType(typeof(PagedResponse<TransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDate(
        [FromQuery] DateOnly date, 
        [FromQuery, Range(1, int.MaxValue, ErrorMessage = "Номер страницы должен быть больше 0.")] int page = 1, 
        [FromQuery, Range(1, 100, ErrorMessage = "Размер страницы должен быть от 1 до 100.")] int size = 10, 
        CancellationToken ct = default)
    {
        var result = await transactionService.GetByDateAsync(date, page, size, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransactionRequest request, CancellationToken ct)
    {
        var result = await transactionService.UpdateAsync(id, request, ct);
        return result.Match(Ok, this.Problem);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await transactionService.DeleteAsync(id, ct);
        return result.Match(_ => NoContent(), this.Problem); 
    }
}
