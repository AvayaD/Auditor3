param(
    # The folder to scan (defaults to the current directory)
    [string]$SourceDirectory = ".",
    
    # Extensions to include
    [string[]]$IncludeExtensions = @(".cs", ".xaml", ".csproj", ".json", ".xml", ".config"),
    
    # Folders to exclude from the scan
    [string[]]$ExcludeFolders = @("bin", "obj", ".git", ".vs", "packages", "node_modules")
)

$TempCombinedFile = "FullProjectCode_Temp.txt"
$BaseOutputName = "ProjectCode_Part"

# 1. Clean up any previous runs
Remove-Item $TempCombinedFile -ErrorAction SilentlyContinue
Get-ChildItem -Filter "$BaseOutputName*.txt" | Remove-Item -ErrorAction SilentlyContinue

Write-Host "Scanning for files..." -ForegroundColor Cyan

# 2. Gather and filter files
$filesToExport = Get-ChildItem -Path $SourceDirectory -Recurse -File | Where-Object {
    $file = $_
    $keep = $false

    # Check if the extension matches our include list
    if ($IncludeExtensions -contains $file.Extension.ToLower()) {
        $keep = $true
        
        # Check against exclude folders
        foreach ($excl in $ExcludeFolders) {
            # Match the excluded folder name anywhere in the directory path
            if ($file.DirectoryName -match "\\$excl(\\|$)") {
                $keep = $false
                break
            }
        }
    }
    $keep
}

Write-Host "Found $($filesToExport.Count) files. Combining into a single file..." -ForegroundColor Cyan

# 3. Combine files with headers
foreach ($file in $filesToExport) {
    # Add a clear separator and file path so the AI knows which file this is
    $header = "`n====================================================================`n" +
              "FILE: $($file.FullName.Replace((Resolve-Path $SourceDirectory).Path + '\', ''))`n" +
              "====================================================================`n"
    
    Add-Content -Path $TempCombinedFile -Value $header
    Get-Content -Path $file.FullName -Raw | Add-Content -Path $TempCombinedFile
}

# 4. Read the combined file and split it into 4 parts
Write-Host "Splitting the combined file into 4 parts..." -ForegroundColor Cyan

# Read all lines into an array
$allLines = Get-Content -Path $TempCombinedFile
$totalLines = $allLines.Count

# Calculate lines per file (rounding up to ensure we don't miss any lines)
$linesPerPart = [math]::Ceiling($totalLines / 4)

for ($i = 0; $i -lt 4; $i++) {
    $startIndex = $i * $linesPerPart
    
    # If we've exceeded the total lines, stop (handles edge cases with very small projects)
    if ($startIndex -ge $totalLines) { break }
    
    $endIndex = [math]::Min((($i + 1) * $linesPerPart - 1), ($totalLines - 1))
    
    # Extract the chunk
    $chunk = $allLines[$startIndex..$endIndex]
    
    # Save the chunk
    $partFileName = "$BaseOutputName$($i + 1).txt"
    $chunk | Set-Content -Path $partFileName
    
    Write-Host "Created $partFileName ($($chunk.Count) lines)" -ForegroundColor Green
}

# 5. Clean up the temporary combined file
Remove-Item $TempCombinedFile -ErrorAction SilentlyContinue

Write-Host "Done! You can now upload the $BaseOutputName*.txt files to your context window." -ForegroundColor Yellow
