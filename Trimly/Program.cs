using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Blazorise;
using Blazorise.FluentUI2;
using Blazorise.Icons.FluentUI;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Trimly;
using Trimly.Core.Application.Interfaces.Services;
using Trimly.Core.Application.Services;
using Trimly.Infrastructure.Api;
using Trimly.Infrastructure.Api.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();

builder.Services.AddTransient<AuthTokenHandler>();
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthTokenHandler>();
    handler.InnerHandler = new HttpClientHandler();
    
    return new HttpClient(handler) 
    { 
        BaseAddress = new Uri("http://localhost:5172/api"),
        Timeout = TimeSpan.FromSeconds(5000)
    };
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRegisteredCompanyService, RegisteredCompanyHttpService>();
builder.Services.AddScoped<IScheduleService, ScheduleHttpService>();
builder.Services.AddScoped<IServices, ServicesHttp>();
builder.Services.AddScoped<IAppointmentHttpService, AppointmentHttpService>();
builder.Services
    .AddBlazorise()
    .AddFluentUI2Providers()
    .AddFluentUIIcons();
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.Converters.Add(new JsonStringEnumConverter());
});
builder.Logging.SetMinimumLevel(LogLevel.Debug);

await builder.Build().RunAsync();
