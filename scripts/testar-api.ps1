$base = 'http://localhost:5088'
Write-Host '1) Health:' -ForegroundColor Cyan
Invoke-RestMethod "$base/health" | Format-List
Write-Host '2) Cadastro de paciente fictício:' -ForegroundColor Cyan
$body = @{ nome = 'Paciente Exemplo'; email = 'paciente@exemplo.invalid' } | ConvertTo-Json
$novo = Invoke-RestMethod "$base/api/pacientes" -Method Post -ContentType 'application/json' -Body $body
$novo | Format-List
Write-Host '3) Consulta por ID:' -ForegroundColor Cyan
Invoke-RestMethod "$base/api/pacientes/$($novo.id)" | Format-List
Write-Host '4) Listagem:' -ForegroundColor Cyan
Invoke-RestMethod "$base/api/pacientes" | Format-Table
Write-Host 'Atenção: os dados ficam em memória, sem persistência.' -ForegroundColor Yellow
