using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Helpers;

public static class ApiResponseHelper
{
    public static ApiResponse<T> CreateSuccess<T>(T? data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
        };
    }

    public static ApiResponse<T> CreateFailure<T>(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
        };
    }

    public static ApiResponse<T> CreateNotFound<T>(string message = "Resource not found")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
        };
    }

    public static ApiResponse<T> CreateBadRequest<T>(string message = "Bad request")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
        };
    }

    public static ApiResponse<T> CreateUnauthorized<T>(string message = "Unauthorized")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
        };
    }
}
