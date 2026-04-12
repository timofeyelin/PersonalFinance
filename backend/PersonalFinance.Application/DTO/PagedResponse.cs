namespace PersonalFinance.Application.DTO;

public record PagedResponse<T>(List<T> Items, int TotalCount);