if ((gitversion /showvariable prereleaselabel) -eq "feat") {
    Write-Host "Version is $(gitversion /showvariable FullSemVer); not pushing to NuGet."
    return;
}
mkdir dist

$artifacts.keys `
    | ForEach-Object { $artifacts[$_]['sourcePath'] } `
    | Where-Object { $_ -notlike '*.symbols.nupkg' } `
    | Copy-Item -Destination dist
$packages = Get-ChildItem dist `
    | Select-Object -ExpandProperty FullName `
    | Where-Object { $_ -notlike '*.symbols.nupkg' }

$pushed = $((nuget list RdbmsEventStore).Replace(" ", ",") -replace "$",".nupkg")

$packages `
    | Where-Object { ($_ | Split-Path -Leaf) -notin $pushed } `
    | ForEach-Object { dotnet nuget push $_ -k $env:NUGET_API_KEY -sk $env:NUGET_API_KEY -s https://nuget.org/api/v2/package -ss https://nuget.smbsrc.net/ }

if ($LastExitCode -ne 0) { throw; }
