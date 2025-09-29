namespace VanHanhCD1.Exceptions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static IServiceCollection
        AddGlobalExceptionHandling(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });
            return services;
        }
    }
}
