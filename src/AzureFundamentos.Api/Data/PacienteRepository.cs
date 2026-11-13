using System.Collections.Concurrent;
using AzureFundamentos.Api.Models;

namespace AzureFundamentos.Api.Data;

public sealed class PacienteRepository : IPacienteRepository
{
    private readonly ConcurrentDictionary<int, Paciente> _pacientes = new();
    private int _proximoId;

    public IReadOnlyList<Paciente> Listar() =>
        _pacientes.Values.OrderBy(p => p.Id).ToArray();

    public Paciente? Obter(int id) =>
        _pacientes.TryGetValue(id, out var paciente) ? paciente : null;

    public Paciente Adicionar(string nome, string? email)
    {
        var id = Interlocked.Increment(ref _proximoId);
        var paciente = new Paciente(id, nome, email);
        _pacientes[id] = paciente;
        return paciente;
    }
}
