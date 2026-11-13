
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFundamentos.Api.Diagnostics;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, titulo, detalhe) = exception switch
        {
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Recurso não encontrado",
                "O recurso solicitado não existe."),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Requisição inválida",
                "Os dados informados são inválidos."),

            ConflitoNegocioException => (
                StatusCodes.Status409Conflict,
                "Conflito de negócio",
                "A operação não pode ser concluída devido a um conflito."),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Erro interno do servidor",
                "Ocorreu um erro inesperado.")
        };


        context.Response.StatusCode = status;


        if (status >= 500)
        {
            logger.LogError(
                exception,
                "Erro interno em {Metodo} {Caminho}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);
        }
        else
        {
            logger.LogWarning(
                "Falha tratada. HTTP {Status}. Tipo: {Tipo}. Caminho: {Caminho}. TraceId: {TraceId}",
                status,
                exception.GetType().Name,
                context.Request.Path,
                context.TraceIdentifier);
        }


        var problem = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalhe,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] =
            context.TraceIdentifier;

        var escrito = await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = context,
                Exception = exception,
                ProblemDetails = problem
            });

        if (!escrito)
        {
            context.Response.ContentType = "text/plain";

            await context.Response.WriteAsync(
                "Erro interno do servidor.",
                cancellationToken);
        }

        return true;
    }
}

