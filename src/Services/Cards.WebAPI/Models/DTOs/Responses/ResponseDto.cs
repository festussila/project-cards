namespace Cards.WebAPI.Models.DTOs.Responses;

/// <summary>
/// A complex generic base object encapsulating a response object
/// </summary>  
public class ResponseDto<T>
{
    /// <summary>
    /// A message to be displayed to end-user after evaluating status code
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// Generic object containing invoked method's response
    /// </summary>
    public IEnumerable<T> Data { get; set; }

    /// <summary>
    /// Contains pagination details for paginated collections
    /// </summary>
    public Pagination Pagination { get; set; }

    public ResponseDto()
    {
        Message = "Request processed successfully";
        Data = Enumerable.Empty<T>();
        Pagination = new();
    }
}

/// <summary>
/// Pagination
/// </summary>
public class Pagination
{
    /// <summary>
    /// Current page served
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of elements in a page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total count of all elements based on filters
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total count of pages based on filters
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Flag for a next page
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Flag for a previous page
    /// </summary>
    public bool HasPreviousPage { get; set; }
}
