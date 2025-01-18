using System.Collections;
using HopFrame.Testing;
using Microsoft.FluentUI.AspNetCore.Components;
using HopFrame.Testing.Components;
using HopFrame.Testing.Models;
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
    options.DisplayUserInfo(false);
    options.AddDbContext<DatabaseContext>(context => {
        context.Table<User>(table => {
            table.Property(u => u.Password)
                .SetParser((pwd, _) => pwd + "-edited");

            table.Property(u => u.FirstName)
                .List(false);

            table.Property(u => u.LastName)
                .List(false);

            table.Property(u => u.Id)
                .IsSortable(false)
                .SetOrderIndex(3);

            table.AddListingProperty("Name", (user, _) => $"{user.FirstName} {user.LastName}")
                .SetOrderIndex(2);

            table.SetDisplayName("Benutzer");
            table.SetDescription("This table is used for user data store and user authentication");

            table.SetViewPolicy("policy");

            table.Property(u => u.Posts)
                .FormatEach<Post>((post, _) => post.Caption);
        });

        context.Table<Post>()
            .Property(p => p.Author)
            .Format((user, _) => $"{user.FirstName} {user.LastName}");

        context.Table<Post>()
            .Property(p => p.Id)
            .SetDisplayName("ID");

        context.Table<Post>()
            .Property(p => p.CreatedAt);

        context.Table<Post>()
            .Property(p => p.Content)
            .IsTextArea(true)
            /*.Validator(input => {
                var errors = new List<string>();
                
                if (input is null)
                    errors.Add("Value cannot be null");
                
                if (input?.Length > 10)
                    errors.Add("Value can only be 10 characters long");

                return errors;
            })*/;

        context.Table<Post>()
            .SetOrderIndex(-1);
    });
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
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(HopFrameHome).Assembly);

app.Run();