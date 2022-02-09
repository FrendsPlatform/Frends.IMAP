# Frends.IMAP.ReadEmail

[![Frends.IMAP.ReadEmail Main](https://github.com/FrendsPlatform/Frends.IMAP/actions/workflows/ReadEmail_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.IMAP/actions/workflows/ReadEmail_main.yml)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.IMAP.ReadEmail?label=NuGet)
![GitHub](https://img.shields.io/github/license/FrendsPlatform/Frends.IMAP?label=License)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.IMAP/Frends.IMAP.ReadEmail|main)

Task for reading emails using IMAP protocol.

## Installing

You can install the Task via frends UI Task View or you can find the NuGet package from the following NuGet feed
https://www.myget.org/F/frends-tasks/api/v2.

## Settings for IMAP servers

|Property                   |Type                       |Description                |Example|
|---------------------------|---------------------------|---------------------------|---------------|
|Host                       |string                     |Host address               |imap.frends.com|
|Port                       |int                        |Host port                  |993|
|UseSSL                     |bool                       |Use SSL when connecting host|true|
|AcceptAllCerts             |bool                       |Accept all certificates when connecting the host, if true, will accept event invalid certificates. If false, will accept self-signed certificates if the root is untrusted|false|
|UserName                   |string                     |Account name to login with|emailUser|
|Password                   |string                     |Account password          |***|

### Options

|Property                   |Type                       |Description                |Example|
|---------------------------|---------------------------|---------------------------|---------------|
|MaxEmails                  |int                        |Maximum number of emails to retrieve|10|
|GetOnlyUnreadEmails        |bool                       |If true, will retrieve only unread emails|false|
|MarkEmailsAsRead           |bool                       |If true, will mark retrieved emails as read|false|
|DeleteReadEmails           |bool                       |If true, will delete retrieved emails from server|false|

## Result
ReadEmail task returns a list of EmailMessageResult objects. Each object contains following properties:

|Property                   |Type                       |Description                |Example|
|---------------------------|---------------------------|---------------------------|---------------|
|Id                         |string                     |Email message id           | ... |
|To                         |string                     |To field from email        |agent@frends.com|
|Cc                         |string                     |Cc field from email        |doubleagent@frends.com|
|From                       |string                     |From field from email      |sender@frends.com|
|Date                       |DateTime                   |Received date              | ... |
|Subject                    |string                     |Email subject              |Important email!|
|BodyText                   |string                     |Plain text email body      | ... |
|BodyHtml                   |string                     |Html email body            | ... |

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
