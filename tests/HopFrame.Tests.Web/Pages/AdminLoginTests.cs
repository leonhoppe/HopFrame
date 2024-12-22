using BlazorStrap;
using Bunit;
using Bunit.TestDoubles;
using CurrieTechnologies.Razor.SweetAlert2;
using HopFrame.Security.Models;
using HopFrame.Web.Pages.Administration;
using HopFrame.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HopFrame.Tests.Web.Pages;

public class AdminLoginTests : TestContext {

    private (IRenderedComponent<AdminLogin>, NavigationManager) SetupEnvironment(bool correctCredentials = true) {
        var auth = new Mock<IAuthService>();
        auth
            .Setup(a => a.Login(It.IsAny<UserLogin>()))
            .ReturnsAsync(correctCredentials);
        
        Services.AddSweetAlert2();
        Services.AddBlazorStrap();
        Services.AddSingleton(auth.Object);
        var navigator = Services.GetRequiredService<FakeNavigationManager>();

        var component = RenderComponent<AdminLogin>();
        return (component, navigator);
    }
    
    [Fact]
    public void Login_Has_RequiredFields() {
        // Arrange
        var (component, _) = SetupEnvironment();

        // Act
        var inputs = component.FindAll("input");
        var buttons = component.FindAll("button");
        var form = component.FindAll("form");
        
        // Assert
        Assert.Equal(2, inputs.Count);
        Assert.Single(buttons);
        Assert.Single(form);
        Assert.Equal("submit", buttons[0].Attributes.GetNamedItem("type")?.Value);
        
        foreach (var input in inputs) {
            var attribute = input.Attributes.GetNamedItem("required");
            Assert.NotNull(attribute);
            Assert.NotEqual("false", attribute?.Value);
        }
    }

    [Fact]
    public void Login_With_CorrectCredentials_Should_Redirect() {
        // Arrange
        var (component, nav) = SetupEnvironment();
        var email = component.Find("""input[type="email"]""");
        var password = component.Find("""input[type="password"]""");
        var submit = component.Find("button");

        // Act
        email.Change("test@example.com");
        password.Change("1234567890");
        submit.Click();
        
        // Assert
        Assert.EndsWith("/administration", nav.Uri);
    }
    
    [Fact]
    public void Login_With_CorrectCredentials_And_CustomRedirect_Should_Redirect() {
        // Arrange
        var (component, nav) = SetupEnvironment();
        var email = component.Find("""input[type="email"]""");
        var password = component.Find("""input[type="password"]""");
        var submit = component.Find("button");

        component.Instance.RedirectAfter = "testRedirect";

        // Act
        email.Change("test@example.com");
        password.Change("1234567890");
        submit.Click();
        
        // Assert
        Assert.EndsWith("/administration/testRedirect", nav.Uri);
    }
    
    [Fact]
    public void Login_With_IncorrectCredentials_Should_Fail() {
        // Arrange
        var (component, nav) = SetupEnvironment(false);
        var email = component.Find("""input[type="email"]""");
        var password = component.Find("""input[type="password"]""");
        var submit = component.Find("button");

        // Act
        email.Change("test@example.com");
        password.Change("1234567890");
        submit.Click();
        
        // Assert
        Assert.False(nav.Uri.EndsWith("/administration"));
    }

    [Fact]
    public void Login_With_IncorrectCredentials_DisplaysError() {
        // Arrange
        var (component, _) = SetupEnvironment(false);
        var email = component.Find("""input[type="email"]""");
        var password = component.Find("""input[type="password"]""");
        var submit = component.Find("button");

        // Act
        email.Change("test@example.com");
        password.Change("1234567890");
        submit.Click();
        
        // Assert
        Assert.Contains("Email or password does not match any account!", component.Markup);
    }
}