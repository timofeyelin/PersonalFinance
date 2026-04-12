using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Application.DTO;

public record UpdateArticleRequest(
    [Required(ErrorMessage = "Название обязательно.")]
    [MaxLength(150, ErrorMessage = "Максимальная длина 150 символов.")]
    [RegularExpression(@"^[^\r\n]+$", ErrorMessage = "Название не должно содержать переносов строк.")]
    string Name, 
    
    Guid CategoryId, 
    
    bool IsActive
);
