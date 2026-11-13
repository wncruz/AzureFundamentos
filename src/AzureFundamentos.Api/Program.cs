using AzureFundamentos.Api.Data;
using AzureFundamentos.Api.Models;
using AzureFundamentos.Api.Diagnostics;
using AzureFundamentos.Api.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Integra o ILogger aos logs do Azure App Service
builder.Logging.AddAzureWebAppDiagnostics();

builder.Services.AddSingleton<IPacienteRepository, PacienteRepository>();


builder.Services
    .AddOptions<AppInfoOptions>()
    .Bind(builder.Configuration.GetSection(
        AppInfoOptions.SectionName))
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Nome),
        "AppInfo:Nome é obrigatório.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Mensagem),
        "AppInfo:Mensagem é obrigatória.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Versao),
        "AppInfo:Versao é obrigatória.")
    .ValidateOnStart();


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new
{
    aplicacao = "Azure Fundamentos API",
    versao = "1.0",
    ambiente = app.Environment.EnvironmentName
}));


app.MapGet("/health",
    (ILogger<Program> logger,
     IWebHostEnvironment environment) =>
    {
        var horario = DateTimeOffset.UtcNow;

        logger.LogInformation(
            "Health Check executado. Ambiente: {Ambiente}. Horario: {HorarioUtc}",
            environment.EnvironmentName,
            horario);

        return Results.Ok(new
        {
            status = "Healthy",
            horarioUtc = horario
        });
    });

if (app.Environment.IsDevelopment())
{

    var pacientes = app.MapGroup("/api/pacientes");


    pacientes.MapGet("",
        (IPacienteRepository repo,
         ILogger<Program> logger) =>
        {
            logger.LogInformation(
                "Iniciando consulta de pacientes.");

            var lista = repo.Listar();

            logger.LogInformation(
                "Consulta concluída. Total de registros: {Total}",
                lista.Count);

            return Results.Ok(lista);
        });


    pacientes.MapGet("/{id:int}", (int id, IPacienteRepository repositorio) =>
        repositorio.Obter(id) is { } paciente
            ? Results.Ok(paciente)
            : Results.NotFound(new { erro = "Paciente não encontrado." }));

    pacientes.MapPost("", (CriarPacienteRequest request, IPacienteRepository repositorio) =>
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || request.Nome.Trim().Length < 3)
            return Results.BadRequest(new { erro = "Nome deve ter pelo menos 3 caracteres." });

        var novo = repositorio.Adicionar(request.Nome.Trim(), request.Email?.Trim());
        return Results.Created($"/api/pacientes/{novo.Id}", novo);
    });


    app.MapGet("/api/diagnostico/nao-encontrado", () =>
    {
        throw new KeyNotFoundException(
            "Paciente não encontrado.");
    });

    app.MapGet("/api/diagnostico/requisicao-invalida", () =>
    {
        throw new ArgumentException(
            "Os dados informados são inválidos.");
    });

    app.MapGet("/api/diagnostico/conflito", () =>
    {
        throw new ConflitoNegocioException(
            "Já existe um paciente com este identificador.");
    });

    app.MapGet("/api/diagnostico/excecao", () =>
    {
        throw new InvalidOperationException(
            "Falha simulada para treinamento Azure.");
    });


    app.MapGet("/api/configuracao",
        (IOptions<AppInfoOptions> options,
         IWebHostEnvironment environment) =>
        {
            var config = options.Value;

            return Results.Ok(new
            {
                Aplicacao = config.Nome,
                Mensagem = config.Mensagem,
                Versao = config.Versao,
                Ambiente = environment.EnvironmentName
            });
        }
    );


    app.MapGet("/api/options/comparacao",
        (
            IOptions<AppInfoOptions> options,
            IOptionsSnapshot<AppInfoOptions> snapshot,
            IOptionsMonitor<AppInfoOptions> monitor
        ) =>
        {
            return Results.Ok(new
            {
                IOptions = options.Value.Mensagem,

                IOptionsSnapshot = snapshot.Value.Mensagem,

                IOptionsMonitor = monitor.CurrentValue.Mensagem
            });
        }
    );
}

app.Run();

public partial class Program { }
