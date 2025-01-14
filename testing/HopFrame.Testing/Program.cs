using HopFrame.Testing;
using Microsoft.FluentUI.AspNetCore.Components;
using HopFrame.Testing.Components;
using HopFrame.Testing.Models;
using HopFrame.Web;
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
    options.AddDbContext<DatabaseContext>()
        .Table<User>(table => table.Ignore());
});

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
    .AddInteractiveServerRenderMode();

app.Run();