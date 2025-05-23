namespace Questao5.Infrastructure.Services.Middlewares
{
    public class ExceptionHandlingMiddleware
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
                _logger.LogError(ex, "Erro não tratado capturado pelo middleware.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Sucesso = false,
                    Erro = new
                    {
                        Tipo = "INTERNAL_ERROR",
                        Mensagem = "Ocorreu um erro inesperado. Tente novamente mais tarde."
                    }
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

}
