using FrontendTest;
using FrontendTest.Components;
using HopFrame.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddHopFrameServices<DatabaseContext>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthorization();
app.UseAuthentication();
app.UseMiddleware<AuthMiddleware>();

app.MapRazorComponents<App>()
    .AddHopFramePages()
    .AddInteractiveServerRenderMode();

app.Run();