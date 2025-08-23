param(
    [Parameter(Mandatory=$true)]
    [string]$ToolName
)

# Sanitize tool name: remove non-alphanumerics and PascalCase words
$words = [regex]::Split($ToolName, '[^A-Za-z0-9]+') | Where-Object { $_ -ne '' }
if ($words.Count -eq 0) {
    Write-Host "Invalid ToolName provided." -ForegroundColor Red
    exit 1
}
$pascal = ($words | ForEach-Object {
    if ($_.Length -gt 1) {
        $_.Substring(0,1).ToUpper() + $_.Substring(1)
    } else {
        $_.Substring(0,1).ToUpper()
    }
}) -join ''
$sanitizedName = $pascal

# Use sanitized name for paths and filenames
$basePath = Join-Path -Path $PSScriptRoot -ChildPath "Pages/Tools/$sanitizedName"

# Abort if either sanitized or raw path exists to avoid duplicates
if ((Test-Path $basePath) -or (Test-Path (Join-Path -Path $PSScriptRoot -ChildPath "Pages/Tools/$ToolName"))) {
    Write-Host "Folder already exists for '$ToolName' or '$sanitizedName'. Aborting." -ForegroundColor Red
    exit 1
}


# Create the folder
New-Item -ItemType Directory -Path $basePath | Out-Null

# Create Razor component
$razorFile = Join-Path $basePath "${sanitizedName}Page.razor"
@"
@page "/$sanitizedName"

<h3>$sanitizedName Tool</h3>
<!-- Add your tool UI here -->
"@ | Set-Content $razorFile

# Create CSS file
$cssFile = Join-Path $basePath "${sanitizedName}Page.razor.css"
@"
/* Styles for $sanitizedName tool */
"@ | Set-Content $cssFile


# Create a C# class (logic placeholder) following static helper pattern
$csFile = Join-Path $basePath "${sanitizedName}.cs"
$csContent = @"
// C# logic for $sanitizedName tool
namespace DeveloperTools.Pages.Tools.$sanitizedName
{
    public static class $sanitizedName
    {
        // Add public static methods here
    }
}
"@
Set-Content $csFile $csContent

Write-Host "Scaffolded new tool at: $basePath" -ForegroundColor Green
Write-Host " - Razor: $(Split-Path -Leaf $razorFile)" -ForegroundColor Green
Write-Host " - CSS:   $(Split-Path -Leaf $cssFile)" -ForegroundColor Green
Write-Host " - C#:    $(Split-Path -Leaf $csFile)" -ForegroundColor Green
