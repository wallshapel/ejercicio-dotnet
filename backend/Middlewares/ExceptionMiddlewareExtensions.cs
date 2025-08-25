namespace backend.Middlewares
{
    /// <summary>
    /// Extension method to register the global exception middleware in a fluent manner.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
