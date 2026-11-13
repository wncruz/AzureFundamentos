
namespace AzureFundamentos.Api.Configuration;

public sealed class AppInfoOptions
{
    public const string SectionName = "AppInfo";

    public string Nome { get; set; } = string.Empty;

    public string Mensagem { get; set; } = string.Empty;

    public string Versao { get; set; } = string.Empty;
}

