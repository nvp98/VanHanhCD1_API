namespace VanHanhCD1.DTOs.Response
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public ApiResponse(int code, string message, T? data) {
            Code = code;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> SuccessResponse(T Data, string message = "Successful")
        {
            return new ApiResponse<T>(200, message, Data);
        }

    }
}
