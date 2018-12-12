[![Build status](https://ci.appveyor.com/api/projects/status/9m5pit2w7v3c8nl2/branch/master?svg=true)](https://ci.appveyor.com/project/pratikpanda/botbuilder-elasticsearch-dotnet/branch/master)

# botbuilder-elasticsearch-dotnet
Repository for Elasticsearch based extensions for the Bot Builder .NET SDK.

To see the list of current extensions and utilities available for the Bot Builder .NET SDK, use the link below to jump to the extensions or utilities section.

* [Extensions](#extensions)
* [Utilities](#utilities)

## Installation

Each extension is available individually from NuGet. See each individual component's description for installation details and links.

## Contributing and Reporting Issues

Please contribute to this project, in the form of bug fixes, enhancements or new extensions (based on elasticsearch only). Please fork the repo and raise a PR if you have something you would like me to review for inclusion.  If you want to discuss an idea first then the best way to do this right now is to raise a GitHub issue.

## Extensions
| Name | Description | NuGet |
| ------ | ------ | ------ |
| [Elasticsearch Storage](libraries/Bot.Builder.Elasticsearch.Storage) | Elasticsearch based storage extension for bots created using Microsoft Bot Framework. | [![NuGet version](https://img.shields.io/badge/NuGet-0.1.3-blue.svg)](https://www.nuget.org/packages/Bot.Builder.Elasticsearch.Storage/) |
| [Elasticsearch Transcript Logger](libraries/Bot.Builder.Elasticsearch.TranscriptLogger) | Elasticsearch based transcript logger extension for bots created using Microsoft Bot Framework. | [![NuGet version](https://img.shields.io/badge/NuGet-0.1.3-blue.svg)](https://www.nuget.org/packages/Bot.Builder.Elasticsearch.TranscriptLogger/) |
| [Elasticsearch Transcript Store](libraries/Bot.Builder.Elasticsearch.TranscriptStore) | Elasticsearch based transcript store extension for bots created using Microsoft Bot Framework. | [![NuGet version](https://img.shields.io/badge/NuGet-0.1.3-blue.svg)](https://www.nuget.org/packages/Bot.Builder.Elasticsearch.TranscriptStore/) |

## Utilities
| Name | Description |
| ------ | ------ |
| [Elasticsearch Transcript Store Manager](utilities/Bot.Builder.Elasticsearch.TranscriptStore.Manager) | Transcript store manager for the Elasticsearch based transcript store that allows for retrieval, deletion of activity transcripts. The component also allows for generation of transcripts as a .transcript file.|
| [Elasticsearch Transcript Store Console Client](utilities/Elasticsearch.TranscriptStore.ConsoleClient) | Console client for the Elasticsearch based transcript store that utilizes the transcript store manager. |
