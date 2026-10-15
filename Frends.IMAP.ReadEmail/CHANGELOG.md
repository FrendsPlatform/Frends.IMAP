# Changelog

## [3.0.0] - 2026-10-15
### Changed
- Breaking: The Task now takes `Input`, `Connection` and `Options` parameters (`Settings` was renamed to `Connection`), and `MaxEmails` and `GetOnlyUnreadEmails` moved from `Options` to `Input`. Existing processes need to be reconfigured.
- The result now contains `Success` and `Error` properties.
- The Task can now be cancelled.

### Added
- `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options to control how failures are reported.
- Examples added to the result documentation.

## [2.3.0] - 2026-10-02
### Changed
- Upgraded project to target .NET 8

## [2.2.0] - 2026-05-21
### Fixed
- Ensure to delete only emails flagged as Read

## [2.1.0] - 2026-02-23
### Fixed
- Ensure FrendsTaskMetadata.json is included in NuGet package

## [2.0.0] - 2023-06-12
### Fixed
- Fixed task not handling attachments at all (#3).
- Added better description of task.

### Changed
- Breaking change! Result is change from just a list of messages to a wrapper that contains an `Emails` subproperty with the list of messages.

## [1.0.0] - 2022-02-09
### Added
- Initial implementation
