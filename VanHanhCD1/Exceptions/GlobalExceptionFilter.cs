using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VanHanhCD1.DTOs.Response;

namespace VanHanhCD1.Exceptions
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var response = new ApiResponse();
            switch (context.Exception)
            {
                case AppException appException:
                    response.Code = ErrorCodeExtentions.Extentions[appException._errorCode].code;
                    response.Message = ErrorCodeExtentions.Extentions[appException._errorCode].message;
                    context.Result = new BadRequestObjectResult(response);
                    break;
                default:
                    response.Code = ErrorCodeExtentions.Extentions[ErrorCode.UNCATEGORIZED_EXCEPTION].code;
                    response.Message = ErrorCodeExtentions.Extentions[ErrorCode.UNCATEGORIZED_EXCEPTION].message;
                    context.Result = new BadRequestObjectResult(response);
                    break;
            }
            context.ExceptionHandled = true;
        }
    }
}
