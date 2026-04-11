namespace PersonalFinance.Application.DTO;

public record DayStatusResponse(DateOnly Date, decimal TotalAmount, string StatusColor, string Label);