[CmdletBinding()]
param (
    [switch] $watch = $false,
    [switch] $publish = $false,
    [switch] $trace = $false
)

$i = "./assets/app.scss"
$o = "./wwwroot/css/app.min.css"
$cmd = "npx tailwindcss", "-i $i", "-o $o"

if ($watch) { $cmd += "--watch" }
elseif ($publish) { $cmd += "--minify" }

if ($trace) {$cmd += "--trace-warnings"}

$cmd = $cmd | Join-String -Separator ' '

Write-Host "$ $cmd"
Invoke-Expression -Command $cmd
