using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Application.DTO;

public record UpdateTransactionRequest(
    DateOnly Date, 
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше нуля.")]
    decimal Amount, 
    
    [MaxLength(250, ErrorMessage = "Максимальная длина комментария 250 символов.")]
    [RegularExpression(@"^[^\r\n]*$", ErrorMessage = "Комментарий должен быть однострочным.")]
    string? Comment, 
    
    Guid ArticleId,
    string? Emoji
);
