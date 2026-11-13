
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace AzureFundamentos.Api.Tests;

public class PacientesIntegrationTests
{
    [Fact]
    public async Task ListarPacientes_DeveRetornarHttp200()
    {
        // ARRANGE
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
            });

        using var client = factory.CreateClient();

        // ACT
        var response = await client.GetAsync("/api/pacientes");

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        Assert.Equal(
            JsonValueKind.Array,
            document.RootElement.ValueKind);
    }


    [Fact]
    public async Task CadastrarPaciente_DeveRetornar201_EPermitirConsulta()
    {
        // ARRANGE
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
            });

        using var client = factory.CreateClient();

        var paciente = new
        {
            nome = "Paciente Integracao",
            email = "integracao@exemplo.invalid"
        };

        // ACT - Cadastrar paciente
        var response = await client.PostAsJsonAsync(
            "/api/pacientes", paciente);

        // ASSERT - Validar cadastro
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        using var cadastro = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        int id = cadastro.RootElement
            .GetProperty("id").GetInt32();

        Assert.True(id > 0);

        Assert.Equal(
            $"/api/pacientes/{id}",
            response.Headers.Location?.ToString());

        // ACT - Consultar paciente cadastrado
        var consulta = await client.GetAsync(
            $"/api/pacientes/{id}");

        // ASSERT - Validar consulta
        Assert.Equal(
            HttpStatusCode.OK,
            consulta.StatusCode);

        using var resultado = JsonDocument.Parse(
            await consulta.Content.ReadAsStringAsync());

        Assert.Equal(
            "Paciente Integracao",
            resultado.RootElement
                .GetProperty("nome").GetString());

        Assert.Equal(
            "integracao@exemplo.invalid",
            resultado.RootElement
                .GetProperty("email").GetString());
    }


    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("Ab")]
    public async Task CadastrarPaciente_ComNomeInvalido_DeveRetornar400(
        string nome)
    {
        // ARRANGE
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
            });

        using var client = factory.CreateClient();

        var paciente = new
        {
            nome,
            email = "teste@exemplo.invalid"
        };

        // ACT
        var response = await client.PostAsJsonAsync(
            "/api/pacientes", paciente);

        // ASSERT
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        using var documento = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(
            "Nome deve ter pelo menos 3 caracteres.",
            documento.RootElement
                .GetProperty("erro").GetString());
    }


    [Fact]
    public async Task ConsultarPacienteInexistente_DeveRetornar404()
    {
        // ARRANGE
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
            });

        using var client = factory.CreateClient();

        int idInexistente = 999999;

        // ACT
        var response = await client.GetAsync(
            $"/api/pacientes/{idInexistente}");

        // ASSERT
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);

        using var documento = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(
            "Paciente não encontrado.",
            documento.RootElement
                .GetProperty("erro").GetString());
    }


    [Fact]
    public async Task HealthCheck_DeveRetornarHealthy()
    {
        // ARRANGE
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
            });

        using var client = factory.CreateClient();

        // ACT
        var response = await client.GetAsync("/health");

        // ASSERT
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using var documento = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var resultado = documento.RootElement;

        Assert.Equal(
            "Healthy",
            resultado.GetProperty("status").GetString());

        var horario = resultado.GetProperty(
            "horarioUtc").GetString();

        Assert.True(
            DateTimeOffset.TryParse(horario, out _),
            "O horário UTC deve ser uma data válida.");
    }

}
