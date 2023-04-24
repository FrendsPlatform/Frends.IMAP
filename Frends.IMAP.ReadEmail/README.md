# Frends.IMAP.ReadEmail

Frends Task for reading emails using IMAP protocol.

[![Frends.IMAP.ReadEmail Main](https://github.com/FrendsPlatform/Frends.IMAP/actions/workflows/ReadEmail_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.IMAP/actions/workflows/ReadEmail_main.yml)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.IMAP.ReadEmail?label=NuGet)
![GitHub](https://img.shields.io/github/license/FrendsPlatform/Frends.IMAP?label=License)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.IMAP/Frends.IMAP.ReadEmail|main)


## Installing

You can install the Task via frends UI Task View or you can find the NuGet package from the following NuGet feed
https://www.myget.org/F/frends-tasks/api/v2.

## Settings for IMAP servers

|Property                   |Type                       |
|---------------------------|---------------------------|
|Host                       |string                     |
|Port                       |int                        |
|UseSSL                     |bool                       |
|AcceptAllCerts             |bool                       |
|UserName                   |string                     |
|Password                   |string                     |
|ArchiveDirectory           |string                     |

## Options

|Property                   |Type                       |
|---------------------------|---------------------------|
|MaxEmails                  |int                        |
|GetOnlyUnreadEmails        |bool                       |
|MarkEmailsAsRead           |bool                       |
|DeleteReadEmails           |bool                       |
|CreateDirectoryIfNotFound  |bool                       |

## Result
ReadEmail task returns a list of EmailMessageResult objects. Each object contains following properties:

|Property                   |Type                       |
|---------------------------|---------------------------|
|Id                         |string                     |
|To                         |string                     |
|Cc                         |string                     |
|From                       |string                     |
|Date                       |DateTime                   |
|Subject                    |string                     |
|BodyText                   |string                     |
|AttachmentSaveDirs         |List<string>               |

## Usage
You can loop email message by giving task result as input to foreach-shape:
```sh
#result[ReadEmail]
```

You can reference email properties like so:
```sh
#result[ReadEmail][0].BodyText
```

## Building

Clone a copy of the repository

`git clone https://github.com/FrendsPlatform/Frends.Regex.git`

Rebuild the project

`dotnet build`

Run tests

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`

# Testing

## Unit Tests

Unit tests are run on each push and can be run manually by `dotnet test` command.

## Integration Tests

Integration tests in Frends.IMAP are run as part of unit test runs.

## Performance Tests

No performance tests are done in Frends.IMAP as the IMAP (email) server itself is the main component during execution.
