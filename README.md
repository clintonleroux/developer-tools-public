# Developer Tools

> Last Updated: August 23, 2025

## Overview

Developer Tools is a modular Blazor WebAssembly app that bundles small utilities for day‑to‑day development. It currently includes a Data Formatter (JSON/SQL/Text/Stack Trace) and a Data Difference tool for quick text/JSON diffs. The UI uses Fluent UI for Blazor for a modern, accessible experience.

## Features

- Modular tool architecture—each tool is a self‑contained Razor page
- Data Formatter: JSON, SQL, Text, Stack Trace
- Data Difference: text and JSON diff with options (ignore case/whitespace, pretty print, sort keys, show unchanged)
- Modern Fluent UI components
- Easy scaffolding for new tools
- Unit‑tested core logic

## Tech stack

- .NET 9 (Blazor WebAssembly)
- Microsoft.FluentUI.AspNetCore.Components (Fluent UI for Blazor)

## Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/) or later

## Install, Build, Run

From the repository root:

```powershell
# Restore dependencies
dotnet restore src/DeveloperTools/DeveloperTools.csproj

# Build
dotnet build src/DeveloperTools/DeveloperTools.csproj

# Run (opens a local dev server)
dotnet run --project src/DeveloperTools/DeveloperTools.csproj
```

The app will log the local URL (for example, http://localhost:5233). Open it in your browser.

### Run with Docker (optional)

```powershell
docker build -t developer-tools .
docker run --rm -p 8080:8080 developer-tools
```

Then browse to http://localhost:8080.

## Usage

Once running, open a tool by its route:

- Data Formatter: `/DataFormatter`
	- JSON: pretty‑print with optional key sorting
	- SQL/Text/Stack Trace: normalize spacing and layout
- Data Difference: `/DataDifference`
	- Text mode: ignore case/whitespace, trim lines, optionally show unchanged lines
	- JSON mode: pretty print, sort properties, optionally show unchanged
	- Left/Right inputs are persisted to `localStorage`

## Testing

```powershell
dotnet test DeveloperTools.sln
```

## Repository layout

```
.
├─ src/
│  └─ DeveloperTools/
│     ├─ Pages/Tools/        # Razor pages for each tool
│     ├─ Shared/             # Shared components and layout
│     └─ wwwroot/            # Static assets
├─ tests/
│  └─ DeveloperTools.Tests/  # Unit tests
├─ Dockerfile
├─ LICENSE
└─ README.md
```

## Scaffolding a new tool

Use the script or the VS Code task to bootstrap files:

```powershell
# VS Code task: "Scaffold New Tool" (prompts for a name)

# Or run the script directly
cd src/DeveloperTools
./Scaffold-Tool.ps1 -ToolName "MyNewTool"
```

What to do next:

- Set the page route to `/<ToolName>` in the generated Razor page
- Add a nav item in `Shared/NavMenu.razor`
- If you add a logic class, register DI in `Program.cs` and add unit tests under `tests/DeveloperTools.Tests/`

## Fluent UI usage

Fluent components are available via `Microsoft.FluentUI.AspNetCore.Components` and registered in `Program.cs` with `builder.Services.AddFluentUIComponents()`. Use components like `<FluentStack>`, `<FluentTextArea>`, `<FluentButton>`, and `<FluentNavMenu>` for a consistent look and feel.

## Formatting and checks

```powershell
# Ensure no formatting diffs
dotnet format DeveloperTools.sln --verify-no-changes

# Run tests
dotnet test DeveloperTools.sln
```

## License

MIT — see [LICENSE](LICENSE).
