$ErrorActionPreference = "Stop"

function FetchAndUnzip
{
	param ([string]$Url, [string]$Out)

	$tmp = [System.IO.Path]::GetTempFileName()
	[System.Reflection.Assembly]::LoadWithPartialName('System.Net.Http') | Out-Null
	$client = (New-Object System.Net.Http.HttpClient)
	try
	{
		if (-not([string]::IsNullOrEmpty($env:GITHUB_TOKEN)))
		{
			$credentials = [string]::Format([System.Globalization.CultureInfo]::InvariantCulture, "{0}:", $env:GITHUB_TOKEN);
			$credentials = [Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($credentials));
			$client.DefaultRequestHeaders.Authorization = (New-Object System.Net.Http.Headers.AuthenticationHeaderValue("Basic", $credentials));
		}
		$contents = $client.GetByteArrayAsync($url).Result;
		[System.IO.File]::WriteAllBytes($tmp, $contents);
	}
	finally
	{
		$client.Dispose()
	}

	if (-not(Test-Path $Out))
	{
		mkdir $Out | Out-Null
	}
	[System.Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem') | Out-Null
	[System.IO.Compression.ZipFile]::ExtractToDirectory($tmp, $Out)
}

function InstallAppveyorTools
{
	$travisUtilsVersion = "28"
	$localPath = "$env:USERPROFILE\.local"
	$travisUtilsPath = "$localPath\travis-utils-$travisUtilsVersion"
	if (Test-Path $travisUtilsPath)
	{
		echo "Reusing the Travis Utils version $travisUtilsVersion already downloaded under $travisUtilsPath"
	}
	else
	{
		$url = "https://github.com/SonarSource/travis-utils/archive/v$travisUtilsVersion.zip"
		echo "Downloading Travis Utils version $travisUtilsVersion from $url into $localPath"
		FetchAndUnzip $url $localPath
	}

	$mavenLocal = "$env:USERPROFILE\.m2"
	if (-not(Test-Path $mavenLocal))
	{
		mkdir $mavenLocal | Out-Null
	}
	echo "Installating Travis Utils public Maven settings.xml into $mavenLocal"
	Copy-Item "$travisUtilsPath\m2\settings-public.xml" "$mavenLocal\settings.xml"

	$env:ORCHESTRATOR_CONFIG_URL = ""
	$env:TRAVIS = "ORCH-332"
}

function CheckLastExitCode
{
    param ([int[]]$SuccessCodes = @(0))

    if ($SuccessCodes -notcontains $LastExitCode)
	{
        $msg = @"
EXE RETURNED EXIT CODE $LastExitCode
CALLSTACK:$(Get-PSCallStack | Out-String)
"@
        throw $msg
    }
}

InstallAppveyorTools

Add-Type -AssemblyName System.IO.Compression.FileSystem
if (!(Test-Path -Path "C:\maven" )) {
    (new-object System.Net.WebClient).DownloadFile(
        'http://www.us.apache.org/dist/maven/maven-3/3.2.5/binaries/apache-maven-3.2.5-bin.zip',
        'C:\maven-bin.zip'
    )
    [System.IO.Compression.ZipFile]::ExtractToDirectory("C:\maven-bin.zip", "C:\maven")
}