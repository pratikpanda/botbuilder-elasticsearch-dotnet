<#
.SYNOPSIS
	1. If found, clear out any existing artifacts.
    2. Restore the required NuGet packages.
	3. Build the solution in Release mode.
	4. Package all the artifacts (NuGet packages) and put them in the artifaces directory.
#>

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

exec { & dotnet restore }

dotnet build --configuration Release

exec { & dotnet pack .\libraries\Bot.Builder.Elasticsearch.Storage -c Release -o .\artifacts }
exec { & dotnet pack .\libraries\Bot.Builder.Elasticsearch.TranscriptLogger -c Release -o .\artifacts }
exec { & dotnet pack .\libraries\Bot.Builder.Elasticsearch.TranscriptStore -c Release -o .\artifacts }