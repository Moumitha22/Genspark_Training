using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{

    public interface IApiResponseMapper
    {
        ApiResponse<T> MapToOkResponse<T>(string message, T data);
        ApiResponse<object> MapToOkResponse(string message);
        ApiResponse<T> MapToErrorResponse<T>(string message, T data);
    }
}