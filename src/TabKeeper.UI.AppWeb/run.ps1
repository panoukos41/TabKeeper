[CmdletBinding()]
param (
    [switch] $css = $false,
    [switch] $css_watch = $false,
    [switch] $dotnet_watch = $false,
    [switch] $watch = $false
)

if ($css) {
    tailwindcss -i .\assets\app.scss -o .\wwwroot\css\app.css

    exit
}

if ($css_watch) {
    tailwindcss -i .\assets\app.scss -o .\wwwroot\css\app.css --watch

    exit
}

if ($dotnet_watch) {
    dotnet watch run

    exit
}

if ($watch) {
    Start-Job { tailwindcss -i .\assets\app.scss -o .\wwwroot\css\app.css --watch }
    dotnet watch run

    exit
}
