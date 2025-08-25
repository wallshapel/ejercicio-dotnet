using System.Net;
using System.Text.Json;
using backend.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace backend.Middlewares
{
    /// <summary>
    /// Global exception middleware that converts exceptions into consistent JSON responses.
    /// Order matters: this should be placed early in the pipeline.
    /// </summary>
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log and write a normalized JSON error payload
                _logger.LogError(ex, "Unhandled exception");
                await WriteErrorAsync(context, ex);
            }
        }

        private static async Task WriteErrorAsync(HttpContext ctx, Exception ex)
        {
            var (status, payload) = Map(ex, ctx.TraceIdentifier);

            ctx.Response.StatusCode = (int)status;
            ctx.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(payload);
            await ctx.Response.WriteAsync(json);
        }

        private static (HttpStatusCode status, object payload) Map(Exception ex, string traceId)
        {
            // Map specific known exceptions first, then fall back to 500.
            return ex switch
            {
                // 404: no records or entity not found by criteria
                NotFoundException nf => (HttpStatusCode.NotFound,
                    Error("error", nf.Message, traceId)),

                // 400: malformed JSON / invalid request body
                JsonException => (HttpStatusCode.BadRequest,
                    Validation("Malformed JSON.", traceId, new Dictionary<string, string[]>
                    {
                        ["body"] = new[] { "Malformed JSON." }
                    })),

                // 400: ASP.NET Core bad request wrapper
                BadHttpRequestException brex => (HttpStatusCode.BadRequest,
                    Validation("Invalid request.", traceId, new Dictionary<string, string[]>
                    {
                        ["request"] = new[] { brex.Message }
                    })),

                // 409: unique constraint (SQL Server 2627/2601)
                DbUpdateException dbex when IsUniqueViolation(dbex) => (HttpStatusCode.Conflict,
                    Validation("Unique constraint violation.", traceId, new Dictionary<string, string[]>
                    {
                        ["unique"] = new[] { "Duplicate key or unique index violation." }
                    })),

                // 500: fallback
                _ => (HttpStatusCode.InternalServerError,
                    Error("error", "Unexpected server error.", traceId))
            };
        }

        private static bool IsUniqueViolation(DbUpdateException dbex)
        {
            // SQL Server throws SqlException with Number 2627 (unique constraint) or 2601 (duplicate key)
            if (dbex.InnerException is SqlException sql)
                return sql.Number == 2627 || sql.Number == 2601;
            return false;
        }

        private static object Validation(string message, string traceId, IDictionary<string, string[]> errors)
            => new { status = "error", message, traceId, errors };

        private static object Error(string status, string message, string traceId)
            => new { status, message, traceId };
    }
}
