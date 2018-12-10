<#
.SYNOPSIS
	1. If found, clear out any existing artifacts.
    2. Restore the required NuGet packages.
	3. Build the solution in Release mode.
	4. Package all the artifacts (NuGet packages) and put them in the artifacts directory.
#>

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

dotnet restore

dotnet build --configuration Release

dotnet pack .\libraries\Bot.Builder.Elasticsearch.Storage -c Release -o .\artifacts
dotnet pack .\libraries\Bot.Builder.Elasticsearch.TranscriptLogger -c Release -o .\artifacts
dotnet pack .\libraries\Bot.Builder.Elasticsearch.TranscriptStore -c Release -o .\artifacts