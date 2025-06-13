using PropFinderApi.Interfaces;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Mappers
{
    public class ApiResponseMapper : IApiResponseMapper
    {
        public ApiResponse<T> MapToOkResponse<T>(string message, T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public ApiResponse<object> MapToOkResponse(string message)
        {
            return new ApiResponse<object>
            {
                Success = true,
                Message = message,
                Data = null,
                Errors = null
            };
        }
        public ApiResponse<T> MapToOkResponse<T>(string message, T data, PaginationInfoDto pagination)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Pagination = pagination,
                Errors = null
            };
        }
    }

}
