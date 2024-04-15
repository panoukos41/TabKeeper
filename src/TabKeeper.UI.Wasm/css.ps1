[CmdletBinding()]
param (
    [switch] $watch = $false,
    [switch] $publish = $false
)

$i = ".\assets\app.scss"
$o = ".\wwwroot\css\app.min.css"
$cmd = "tailwindcss", "-i $i", "-o $o", "--postcss"

if ($watch) { $cmd += "--watch" }
elseif ($publish) { $cmd += "--minify" }

Write-Host ($cmd | Join-String -Separator ' ')
Invoke-Expression -Command ($cmd | Join-String -Separator ' ')
