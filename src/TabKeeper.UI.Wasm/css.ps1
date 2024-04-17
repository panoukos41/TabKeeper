[CmdletBinding()]
param (
    [switch] $watch = $false,
    [switch] $publish = $false
)

$i = "./assets/app.scss"
$o = "./wwwroot/css/app.min.css"
$cmd = "tailwindcss", "-i $i", "-o $o", "--postcss"

if ($watch) { $cmd += "--watch" }
elseif ($publish) { $cmd += "--minify" }

$cmd = $cmd | Join-String -Separator ' '

Write-Host $cmd
Invoke-Expression -Command $cmd
