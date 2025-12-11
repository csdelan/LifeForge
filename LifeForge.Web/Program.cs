using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LifeForge.Web;
using LifeForge.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7001") });

// Register QuestService
builder.Services.AddScoped<QuestService>();

await builder.Build().RunAsync();
