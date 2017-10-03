gitversion | Out-Host
$buildNumber = $env:APPVEYOR_BUILD_NUMBER
$gitVersion = gitversion | ConvertFrom-JSON
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode)  }

$buildVersion = "$($gitVersion.FullSemVer).build.$buildNumber"
$packageVersion = if (($gitVersion.FullSemVer -like '*-*')) { "$($gitVersion.NuGetVersion).$buildNumber" } else { $gitVersion.FullSemVer }

Write-Host "Build number: $buildNumber"
Write-Host "Git version: $($gitVersion.InformationalVersion)"
Write-Host "Build version: $buildVersion"
Write-Host "Package version: $packageVersion"

return @{
	Build = $buildVersion;
	Package = $packageVersion;
	Git = $gitVersion
}
