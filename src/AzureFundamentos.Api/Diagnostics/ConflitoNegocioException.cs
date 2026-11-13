
namespace AzureFundamentos.Api.Diagnostics;

public sealed class ConflitoNegocioException : Exception
{
    public ConflitoNegocioException(string mensagem)
        : base(mensagem)
    {
    }
}
