$testExitCode = 0

Get-ChildItem src\*.Tests `
| select -ExpandProperty Name `
| % {
	dotnet test "src\\$_" -c Release --no-build --logger trx --results-directory (Join-Path $(pwd) test_results)
	$testExitCode += $LASTEXITCODE
}
if ($testExitCode -ne 0) { $host.SetShouldExit($testExitCode); throw; }

$uploadExitCode
$wc = New-Object 'System.Net.WebClient'
if ($env:APPVEYOR -eq 'True') {
	Get-ChildItem test_results `
	| select -ExpandProperty FullName `
	| % {
		$wc.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", "$_")
		$uploadExitCode += $LASTEXITCODE
	}
}
if ($uploadExitCode -ne 0) { $host.SetShouldExit($uploadExitCode); throw; }
