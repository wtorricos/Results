# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [2.0.3]
- Minor fixes and increase code coverage.
- Add GetValueOrThrow extension method.

## [2.0.2]
- Added Match method to base IMaybe interface.
- Fix invariance issue that was throwing ArgumentOutOfRangeException. The issue happened when for example using Map with
compile time Some<IEnumerable<int>> type but runtime type Some<List<int>>.
- Flatten extension method added as supporting method for FlatMap implementation.

## [2.0.1]
- Added two missing FlatMap Overloads.

## [2.0.0]
### Added
- Support for Linq syntax.

### Breaking Changes
- In order to avoid name collisions with `Microsoft.AspNetCore.Http.IResult` the namespace has been renamed to MaybeResults.
- `IResult` interface has been renamed to `IMaybe`.
- `IErrorResult` interface has been renamed to `INone`.
- `ErrorResultAttribute` has been renamed to `NoneAttribute`.

## Upgrade guide
- Update all your using statements to reference `MaybeResults` instead of the `Results` namespace.
- Update all your method signatures and variables that reference `IResult` to reference `IMaybe` instead.
- Update all your method signatures and variables that reference `IErrorResult` to reference `INone` instead.
- Update all your custom errors to use [None] attribute instead of [ErrorResult].
- Replace all calls to `Result.Success()` with `Maybe.Create()`.

### Fixed
- Minor fixes on documentation and missing extension methods.

## [1.3.4] - 2024-01-29
### Added
- Initial release.
- The following blog post contains docs that match this release version: [Result Pattern in C#](https://medium.com/@walticotc/result-pattern-in-c-537bedda17a6)
