namespace AzureFundamentos.Api.Models;

public record Paciente(int Id, string Nome, string? Email);
public record CriarPacienteRequest(string Nome, string? Email);
