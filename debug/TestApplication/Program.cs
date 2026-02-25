using HopFrame.Core.EFCore;
using HopFrame.Web;
using Microsoft.EntityFrameworkCore;
using TestApplication;
using TestApplication.Components;
using TestApplication.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<DatabaseContext>(options => {
    options.UseInMemoryDatabase("testing");
});

builder.Services.AddHopFrame(config => {
    config.AddDbContext<DatabaseContext>();

    config.Table<User>(table => {
        table.SetDescription("The user dataset. It contains all information for the users of the application.");

        table.Property(u => u.Password)
            .Listable(false);
    });

    config.Table<Post>(table => {
        table.SetDescription("The posts dataset. It contains all posts sent via the application.");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

await using (var scope = app.Services.CreateAsyncScope()) {
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

    foreach (var _ in Enumerable.Range(1, 100)) {
        var firstName = Faker.Name.First();
        var lastName = Faker.Name.Last();

        context.Users.Add(new() {
            Email = $"{firstName}.{lastName}@gmail.com".ToLower(),
            Username = $"{firstName}.{lastName}".ToLower(),
            FirstName = firstName,
            LastName = lastName,
            Description = Faker.Lorem.Paragraph(),
            Birth = DateOnly.FromDateTime(Faker.Identification.DateOfBirth()),
            Password = Faker.RandomNumber.Next(100000L, 99999999999999999L).ToString()
        });
    }

    await context.SaveChangesAsync();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddHopFrame();

app.Run();