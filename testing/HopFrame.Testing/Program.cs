using HopFrame.Core.Services;
using HopFrame.Testing;
using Microsoft.FluentUI.AspNetCore.Components;
using HopFrame.Testing.Components;
using HopFrame.Testing.Models;
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
    options.AddDbContext<DatabaseContext>(context => {
        context.Table<User>(table => {
            table.Property(u => u.Password)
                .ValueParser(pwd => pwd + "-edited");

            table.Property(u => u.FirstName)
                .SetDisplayName("First Name");

            table.Property(u => u.LastName)
                .SetDisplayName("Last Name");

            table.Property(u => u.Id)
                .Sortable(false)
                .ValueTemplate(Guid.CreateVersion7);

            table.SetDisplayName("Benutzer");
        });

        context.Table<Post>()
            .Property(p => p.Author)
            .Format(user => $"{user?.FirstName} {user?.LastName}");

        context.Table<Post>()
            .Property(p => p.Id)
            .SetDisplayName("ID");

        context.Table<Post>()
            .Property(p => p.CreatedAt)
            .ValueTemplate(() => DateTime.UtcNow);
    });
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