if ((gitversion /showvariable prereleaselabel) -eq "feat") {
    Write-Host "Version is $(gitversion /showvariable FullSemVer); not pushing to NuGet."
    return;
}
mkdir dist
$artifacts.keys | % { $artifacts[$_]['sourcePath'] } | ? { $_ -notlike '*.symbols.nupkg' } | Copy-Item -Destination dist
$packages = ls dist | select -ExpandProperty FullName | ? { $_ -notlike '*.symbols.nupkg' }

$pushed = nuget list RdbmsEventStore `
    | Select-Object @{ Name="FileName"; Expression = { "$($_.Replace(" ", ",")).nupkg" } } `
    | Select-Object -ExpandProperty FileName

$packages `
    | Where-Object { ($_ | Split-Path -Leaf) -notin $pushed } `
    | % { dotnet nuget push $_ -k $env:NUGET_API_KEY -sk $env:NUGET_API_KEY -s https://nuget.org/api/v2/package -ss https://nuget.smbsrc.net/ }
if ($LastExitCode -ne 0) { throw; }