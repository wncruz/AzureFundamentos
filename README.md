# Azure Fundamentals — Dia 1 | .NET 10

Laboratório de API REST local para iniciar o treinamento de serviços do Microsoft Azure.

## Pré-requisitos
- SDK .NET 10 (`dotnet --list-sdks`)
- Windows PowerShell ou outro terminal com .NET CLI

## Abrir a solução `.sln`
O arquivo `AzureFundamentos.sln` já está incluído no pacote. Abra no Visual Studio compatível com .NET 10 ou compile a partir da pasta raiz:

```powershell
dotnet build .\AzureFundamentos.sln
```

Caso deseje gerar uma nova solução manualmente, .NET 10 produz `.slnx` por padrão; use `dotnet new sln --format sln` para gerar `.sln`.

## Executar
Abra um primeiro terminal na pasta raiz:

```powershell
$env:ASPNETCORE_URLS = 'http://localhost:5088'
dotnet run --project .\src\AzureFundamentos.Api --no-launch-profile
```

Abra outro terminal na raiz do pacote:

```powershell
.\scripts\testar-api.ps1
```

Se a execução do script estiver bloqueada por política da sua máquina, copie os comandos do script para o terminal sem alterar a política global.

## Endpoints
| Método | Rota | Resultado |
|--------|------|-----------|
| GET | `/` | Identificação do serviço |
| GET | `/health` | Status da API |
| GET | `/api/pacientes` | Lista de registros fictícios |
| GET | `/api/pacientes/{id}` | Busca por ID (200 ou 404) |
| POST | `/api/pacientes` | Cria registro (201 ou 400) |

## Limitações intencionais
- Dados temporários em memória (desaparecem quando a aplicação para).
- Nenhuma autenticação ou autorização: não publique em produção sem implementá-las.
- Não utilizar dados clínicos reais ou pessoais.
- Não há Azure SQL nem deploy na aula inicial.

## Exercícios
1. Criar GET `/api/pacientes/quantidade`.
2. Validar formato do e-mail no POST.
3. Acrescentar endpoint `/info` mostrando somente informações não sensíveis.
4. Desenhar uma arquitetura App Service + Azure SQL para migrar esta API ao Azure.
5. Explicar por que estado em memória não funciona corretamente com múltiplas instâncias.

## Observação
Projeto fornecido como material educacional; requer restauração/compilação no computador do aluno (não executado neste ambiente).
