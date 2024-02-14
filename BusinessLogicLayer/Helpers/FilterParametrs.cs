namespace BusinessLogicLayer.Helpers;

public record FilterParametrs (
    int pageSize,
    int pageNumber,
    string? title,
    decimal? minPrice,
    decimal? maxPrice,
    bool orderByTitle = true)
{
    public string Title = title ?? string.Empty;
    public decimal MinPrice = minPrice ?? 0;
    public decimal MaxPrice = maxPrice ?? decimal.MaxValue;
    public bool OrderByTitle = orderByTitle;
    public int PageNumber = pageNumber;
    public int PageSize = pageSize;
}