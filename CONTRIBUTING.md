# Contributing to Developer Tools

Last Updated: August 23, 2025

Thanks for your interest in improving Developer Tools! This document describes how to propose changes, file issues, and submit pull requests.

## Ways to contribute

- Report bugs and request features by opening issues
- Improve docs and examples
- Add tests and fix failing tests
- Implement new tools or enhance existing ones

## Project setup

Prerequisites: .NET SDK 9.0 or later.

```powershell
dotnet restore src/DeveloperTools/DeveloperTools.csproj
dotnet build src/DeveloperTools/DeveloperTools.csproj
dotnet run --project src/DeveloperTools/DeveloperTools.csproj
```

Run tests:

```powershell
dotnet test DeveloperTools.sln
```

## Branching and pull requests

- Create a feature branch from `main` using a descriptive name, e.g. `feat/data-diff-sort-keys` or `fix/formatter-null-ref`.
- Keep PRs focused and small. Separate unrelated changes.
- Ensure the build, formatter, and tests pass before opening a PR:

```powershell
dotnet format DeveloperTools.sln --verify-no-changes
dotnet build src/DeveloperTools/DeveloperTools.csproj
dotnet test DeveloperTools.sln
```

- Update or add tests for behavior changes.
- Include a concise description: what changed, why, and how it was validated.

## Commit message style

Follow Conventional Commits when possible:

- feat: add new functionality
- fix: bug fix
- docs: documentation only changes
- test: add or update tests
- refactor: code change that neither fixes a bug nor adds a feature
- chore: tooling, CI, or maintenance tasks

Use imperative mood and keep the subject under ~72 characters. Add a short body when helpful.

## Coding standards

- C#/.NET naming and style conventions
- Prefer small, composable methods and pure functions for logic
- Add XML doc comments for public methods where clarity helps
- `.razor` files: UTF‑8 (no BOM), end with a single newline
- Run `dotnet format` before committing

## Adding a new tool

1) Scaffold files via the VS Code task "Scaffold New Tool" or the script:

```powershell
cd src/DeveloperTools
./Scaffold-Tool.ps1 -ToolName "MyNewTool"
```

2) Update the generated Razor page:

- Set `@page "/MyNewTool"` to follow the repo route convention
- Build the UI with Fluent components (`<FluentStack>`, `NumberedTextArea`, etc.)

3) Wire up navigation and DI as needed:

- Add a `<FluentNavLink>` in `Shared/NavMenu.razor`
- If you create a logic/service class, register it in `Program.cs` (e.g., `AddSingleton<MyNewTool>()`) and inject with `@inject`

4) Add tests:

- Create `tests/DeveloperTools.Tests/<MyNewTool>Tests.cs`
- Test the logic class directly; assert exact string outputs (including whitespace)

## Filing bugs and feature requests

When opening an issue, include:

- Environment (OS, .NET SDK version)
- Steps to reproduce
- Expected vs. actual behavior
- Relevant inputs/snippets and screenshots if UI related

Templates are welcome; PRs updating issue templates are appreciated.

## Code of Conduct

Be kind, inclusive, and respectful. This project follows a standard Code of Conduct; if not yet present, assume the [Contributor Covenant](https://www.contributor-covenant.org/version/2/1/code_of_conduct/) applies. Harassment or discrimination is not tolerated.

## License

By contributing, you agree that your contributions will be licensed under the MIT License. See LICENSE for details.
