param(
    [Parameter(Mandatory=$true)]
    [string]$ToolName
)

$basePath = Join-Path -Path $PSScriptRoot -ChildPath "Pages/Tools/$ToolName"

if (Test-Path $basePath) {
    Write-Host "Folder '$basePath' already exists. Aborting." -ForegroundColor Red
    exit 1
}

# Create the folder
New-Item -ItemType Directory -Path $basePath | Out-Null

# Create Razor component
$razorFile = Join-Path $basePath "$ToolName.razor"
@"
@page "/tools/$ToolName"

<h3>$ToolName Tool</h3>

<!-- Add your tool UI here -->
"@ | Set-Content $razorFile


# Create CSS file
$cssFile = Join-Path $basePath "$ToolName.razor.css"
@"
/* Styles for $ToolName tool */
"@ | Set-Content $cssFile


# Optionally, create a C# class (uncomment if needed)
# $csFile = Join-Path $basePath "$ToolName.cs"
# $csContent = @"
# // C# logic for $ToolName tool
# namespace DeveloperTools.Pages.Tools.$ToolName
# {
#     public class $ToolName
#     {
#         // Add properties and methods here
#     }
# }
# "@
# Set-Content $csFile $csContent

Write-Host "Scaffolded new tool at: $basePath" -ForegroundColor Green
