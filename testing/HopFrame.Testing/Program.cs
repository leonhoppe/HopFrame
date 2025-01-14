using HopFrame.Core.Services;
using HopFrame.Testing;
using Microsoft.FluentUI.AspNetCore.Components;
using HopFrame.Testing.Components;
using HopFrame.Testing.Services;
using HopFrame.Web;
using HopFrame.Web.Components.Pages;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();

builder.Services.AddDbContext<DatabaseContext>(options => {
    options.UseInMemoryDatabase("testing");
});

builder.Services.AddHopFrame(options => {
    options.SetAuthHandler<AuthService>();
    options.AddDbContext<DatabaseContext>();
});

builder.Services.AddTransient<IHopFrameAuthHandler, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(HopFrameHome).Assembly);

app.Run();