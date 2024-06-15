using Annular.Translate;
using Annular.Translate.Abstract;
using Annular.Translate.HttpLoader;
using Blazored.LocalStorage;
using Core.Blazor;
using Ignis.Components.HeadlessUI;
using Ignis.Components.WebAssembly;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Reactive.Threading.Tasks;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.Add<DialogOutlet>("#dialog-outlet");

services.AddIgnisWebAssembly();
services.AddBlazoredLocalStorage(c => c.JsonSerializerOptions = Options.Json);

services.AddScoped(sp => new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
services.AddScoped(sp => (IJSInProcessRuntime)sp.GetRequiredService<IJSRuntime>());

services.AddScoped<TranslateLoader, TranslateHttpLoader>();
services.AddScoped<TranslateService>();

var app = builder.Build();

await InitializeLang(app);
await app.RunAsync();

static Task InitializeLang(WebAssemblyHost app)
{
    var localStorage = app.Services.GetRequiredService<ISyncLocalStorageService>();
    var js = app.Services.GetRequiredService<IJSInProcessRuntime>();
    var translate = app.Services.GetRequiredService<TranslateService>();

    translate.Langs.AddRange(["en", "el"]);

    var lang = localStorage.GetItem<string>("lang");
    lang ??= js.GetBrowserLang();

    if (lang is null || !translate.Langs.Contains(lang))
    {
        lang = "en";
    }
    localStorage.SetItem("lang", lang);
    return translate.SetCurrentLang(lang).ToTask();
}
