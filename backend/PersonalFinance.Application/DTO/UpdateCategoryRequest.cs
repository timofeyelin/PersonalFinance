using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Application.DTO;

public record UpdateCategoryRequest(
    [Required(ErrorMessage = "Название обязательно.")]
    [MaxLength(100, ErrorMessage = "Максимальная длина 100 символов.")]
    [RegularExpression(@"^[^\r\n]+$", ErrorMessage = "Название не должно содержать переносов строк.")]
    string Name,

    [Range(0.01, double.MaxValue, ErrorMessage = "Бюджет должен быть больше нуля.")]
    decimal MonthlyBudget,

    bool IsActive
);
