namespace VanHanhCD1.Exceptions
{
    public enum ErrorCode
    {
        UNCATEGORIZED_EXCEPTION = 9999,
        VALIDATE_TIME_EXCEPTION = 1002,
    }
    public static class ErrorCodeExtentions
    {
        public static readonly Dictionary<ErrorCode, (int code, string message)> Extentions = new()
        {
            { ErrorCode.UNCATEGORIZED_EXCEPTION, (9999, "Uncategorized error") },
            { ErrorCode.VALIDATE_TIME_EXCEPTION, (1002, "Time Null") },
        };
    }
}
