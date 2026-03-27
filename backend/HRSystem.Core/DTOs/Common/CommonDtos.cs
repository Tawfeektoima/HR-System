namespace HRSystem.Core.DTOs.Common;

/// <summary>Generic paginated response wrapper</summary>
public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

/// <summary>Standard API response wrapper</summary>
public record ApiResponse<T>(
    bool Success,
    string? Message,
    T? Data,
    List<string>? Errors = null
)
{
    public static ApiResponse<T> Ok(T data, string? message = null)
        => new(true, message, data);

    public static ApiResponse<T> Fail(string message, List<string>? errors = null)
        => new(false, message, default, errors);
}

/// <summary>Pagination query parameters</summary>
public record PaginationParams
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public string? SortBy { get; init; }
    public bool SortDesc { get; init; } = false;
}

/// <summary>Filter for applications</summary>
public record ApplicationFilterParams : PaginationParams
{
    public string? Status { get; init; }
    public int? JobId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public decimal? MinScore { get; init; }
}
