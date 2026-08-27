using System.ComponentModel;
using HopFrame.Core.Configuration;
using HopFrame.Core.EFCore;
using HopFrame.Web;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using TestApplication.Web;
using TestApplication.Web.Components;
using TestApplication.Web.Models;

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
            .Listable(false)
            .SetType(PropertyType.Password);

        table.AddProperty<string>("Username")
            .SetFormatter(u => $"{u.FirstName}.{u.LastName}".ToLower())
            .VisibleInEditor(false)
            .Listable(false);

        table.Property(u => u.Email)
            .SetValidator(input => {
                if (!input.Contains('.'))
                    return ["Email needs to contain a '.'"];

                return [];
            })
            .Presort(ListSortDirection.Ascending);

        table.SetPreferredProperty(u => u.Email);

        table.SetCategory("General");
    });

    config.Table<Post>(table => {
        table.SetDescription("The posts dataset. It contains all posts sent via the application.");
        table.SetEditClaim("deny");

        table.Property(p => p.Type)
            .AsDropdown("Normal", "VIP", "VIP++")
            .SetTypeRaw(PropertyType.Text | PropertyType.Nullable);

        table.Property(p => p.Sender)
            .AsDropdown()
            .SetTypeRaw(PropertyType.Text | PropertyType.Relation | PropertyType.Nullable);

        table.SetCategory("General");
    });

    config.Table<Typer>(table => {
        table.Property(t => t.LongText)
            .SetType(PropertyType.TextArea)
            .SetSizeRange(3..10);

        table.Property(t => t.Password)
            .SetType(PropertyType.Password);

        table.Property(t => t.PhoneNumber)
            .SetType(PropertyType.PhoneNumber);

        table.SetViewClaim("deny");

        table.SetCategory("Advanced");
    });

    config.AddCustomPage(new() {
        Name = "Custom Page",
        Description = "This is a custom page",
        Icon = Icons.Material.Filled.House,
        Route = "/",
        OrderIndex = 2,
        AsIFrame = true,
        Category = "Advanced"
    });

    config.SetCompanyName("Testing");

    config.AddRepository<VirtualRepo, Dictionary<string, object?>>(table => {
        table.SetDisplayName("Addresses");
        table.SetRoute("addresses");
        
        table.AddProperty<int>("Id");
        table.AddProperty<string>("Country");
        table.AddProperty<string>("City");
        table.AddProperty<string>("Street");
        
        table.AddProperty<User>("Owner")
            .IsRelation(config.Table<User>());
    });

    config.AddCategoryIcon("Advanced", Icons.Material.Filled.AccessAlarm);
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

    foreach (var i in Enumerable.Range(1, 100)) {
        var firstName = Faker.Name.First();
        var lastName = Faker.Name.Last();

        context.Users.Add(new() {
            Email = $"{firstName}.{lastName}@gmail.com".ToLower(),
            FirstName = firstName,
            LastName = lastName,
            Description = Faker.Lorem.Paragraph(),
            Birth = DateOnly.FromDateTime(Faker.Identification.DateOfBirth()),
            Password = Faker.RandomNumber.Next(100000L, 99999999999999999L).ToString(),
            Index = i
        });
    }
    
    await context.SaveChangesAsync();

    context.Posts.Add(new() {
        Message = Faker.Lorem.Paragraph(),
        Sender = context.Users.First()
    });
    
    context.Posts.Add(new() {
        Message = Faker.Lorem.Paragraph(),
        Sender = context.Users.Skip(1).First()
    });

    context.Typers.Add(new() {
        Id = Guid.NewGuid(),
        Number = 200,
        Toggle = true,
        DateTime = DateTime.Now,
        DateOnly = DateOnly.Parse("15.03.2022"),
        TimeOnly = TimeOnly.Parse("15:30"),
        SortDirection = ListSortDirection.Descending,
        Text = "A Text",
        Mail = "a@mail.com",
        LongText = "A long Text",
        Password = "1234567890",
        PhoneNumber = "+49 1234 567890",
        SortDirections = [ListSortDirection.Ascending, ListSortDirection.Descending]
    });

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