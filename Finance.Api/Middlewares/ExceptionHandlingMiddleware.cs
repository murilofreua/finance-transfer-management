using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var statusCode = ex switch
                {
                    DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    InvalidOperationException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                var message = statusCode == StatusCodes.Status500InternalServerError
                    ? "Ocorreu um erro interno no servidor."
                    : ex.Message;

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    message
                });
            }
        }
    }
}
