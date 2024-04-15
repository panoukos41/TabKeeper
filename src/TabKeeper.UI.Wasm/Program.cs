using Blazored.LocalStorage;
using Ignis.Components.HeadlessUI;
using Ignis.Components.WebAssembly;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.Add<DialogOutlet>("#dialog-outlet");

services.AddIgnisWebAssembly();
services.AddBlazoredLocalStorage(c => c.JsonSerializerOptions = Options.Json);

await builder.Build().RunAsync();
