using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VanHanhCD1.DTOs.Response;

namespace VanHanhCD1.Exceptions
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Luôn dùng ApiResponse<object> cho trường hợp lỗi (Data = null)
            ApiResponse<object> response;

            switch (context.Exception)
            {
                case AppException appEx:
                    var error = ErrorCodeExtentions.Extentions[appEx._errorCode];
                    response = new ApiResponse<object>(error.code, error.message, default);
                    context.Result = new BadRequestObjectResult(response);
                    break;

                // Có thể thêm các case đặc biệt khác nếu cần
                // case ValidationException valEx:
                // case UnauthorizedAccessException:
                // case ...

                default:
                    var defaultError = ErrorCodeExtentions.Extentions[ErrorCode.UNCATEGORIZED_EXCEPTION];
                    response = new ApiResponse<object>(
                        defaultError.code,
                        defaultError.message,
                        null
                    );
                    context.Result = new BadRequestObjectResult(response);
                    // Nếu muốn trả 500 cho lỗi thật sự nghiêm trọng:
                    // context.Result = new ObjectResult(response) { StatusCode = 500 };
                    break;
            }

            context.ExceptionHandled = true;
        }
    }
}