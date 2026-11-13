
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AzureFundamentos.Api.Tests;

public class GlobalExceptionHandlerTests
{
    [Theory]
    [InlineData("/api/diagnostico/requisicao-invalida",
        400, "Requisição inválida")]
    [InlineData("/api/diagnostico/nao-encontrado",
        404, "Recurso não encontrado")]
    [InlineData("/api/diagnostico/conflito",
        409, "Conflito de negócio")]
    [InlineData("/api/diagnostico/excecao",
        500, "Erro interno do servidor")]
    public async Task DeveRetornarProblemDetails(
        string rota,
        int statusEsperado,
        string tituloEsperado)
    {
        // ARRANGE
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
            });

        using var client = factory.CreateClient();

        // ACT
        var response = await client.GetAsync(rota);

        // ASSERT
        Assert.Equal(statusEsperado, (int)response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var root = document.RootElement;

        Assert.Equal(
            statusEsperado,
            root.GetProperty("status").GetInt32());

        Assert.Equal(
            tituloEsperado,
            root.GetProperty("title").GetString());

        Assert.False(
            string.IsNullOrWhiteSpace(
                root.GetProperty("traceId").GetString()));

        Assert.DoesNotContain(
            "System.InvalidOperationException",
            json);
    }
}

