namespace VanHanhCD1.Exceptions
{
    public class AppException: Exception
    {
        public ErrorCode _errorCode { get; }
        public AppException(ErrorCode errorCode) :
        base(ErrorCodeExtentions.Extentions[errorCode].message)
        { _errorCode = errorCode; }
    }
}
