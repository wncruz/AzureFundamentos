using AzureFundamentos.Api.Models;

namespace AzureFundamentos.Api.Data;

public interface IPacienteRepository
{
    IReadOnlyList<Paciente> Listar();
    Paciente? Obter(int id);
    Paciente Adicionar(string nome, string? email);
}
