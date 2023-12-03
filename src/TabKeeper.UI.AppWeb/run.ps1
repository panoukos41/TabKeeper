[CmdletBinding()]
param (
    [switch] $css = $false,
    [switch] $css_watch = $false,
    [switch] $dotnet_watch = $false,
    [switch] $watch = $false,
    [switch] $publish = $false
)

# CSS
if ($css) {
    tailwindcss -i .\assets\app.scss -o .\wwwroot\css\app.css

    exit
}
if ($css_watch) {
    tailwindcss -i .\assets\app.scss -o .\wwwroot\css\app.css --watch

    exit
}

# .NET
if ($dotnet_watch) {
    dotnet watch run

    exit
}

# Common
if ($watch) {
    Start-Job { tailwindcss -i .\assets\app.scss -o .\wwwroot\css\app.css --watch }
    dotnet watch run

    exit
}
if ($publish) {
    tailwindcss -i .\assets\app.scss -o .\wwwroot\css\app.css --minify
    dotnet publish

    exit
}
