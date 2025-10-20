using System.Globalization;
using System.Reflection;

namespace FluentEmail.Core.Tests;

public class TemplateEmailTests
{
    private Assembly ThisAssembly() => this.GetType().GetTypeInfo().Assembly;
    private const string ToEmail = "bob@test.com";
    private const string FromEmail = "johno@test.com";
    private const string Subject = "sup dawg";

    [Fact]
    public void Anonymous_Model_Template_From_File_Matches()
    {
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENT EMAIL" });

        email.Data.Body.Should().Be("yo email FLUENT EMAIL");
    }

    [Fact]
    public void Using_Template_From_Not_Existing_Culture_File_Using_Default_Template()
    {
        var culture = new CultureInfo("fr-FR");
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENT EMAIL", culture }, culture);

        email.Data.Body.Should().Be("yo email FLUENT EMAIL");
    }

    [Fact]
    public void Using_Template_From_Culture_File()
    {
        var culture = new CultureInfo("he-IL");
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENT EMAIL" }, culture);

        email.Data.Body.Should().Be("hebrew email FLUENT EMAIL");
    }

    [Fact]
    public void Using_Template_From_Current_Culture_File()
    {
        var culture = new CultureInfo("he-IL");
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new {Test = "FLUENT EMAIL"}, culture);

        email.Data.Body.Should().Be("hebrew email FLUENT EMAIL");
    }

    [Fact]
    public void Anonymous_Model_Template_Matches()
    {
        var template = "sup ##Name##";

        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingTemplate(template, new { Name = "LUKE" });

        email.Data.Body.Should().Be("sup LUKE");
    }



    [Fact]
    public void Set_Custom_Template()
    {
        var template = "sup ##Name## here is a list @foreach(var i in Model.Numbers) { @i }";

        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingTemplateEngine(new TestTemplate())
            .UsingTemplate(template, new { Name = "LUKE", Numbers = (string[])["1", "2", "3"] });

        email.Data.Body.Should().Be("custom template");
    }

    [Fact]
    public void Using_Template_From_Embedded_Resource()
    {
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingTemplateFromEmbedded("FluentEmail.Core.Tests.test-embedded.txt", new { Test = "EMBEDDED TEST" }, ThisAssembly());

        email.Data.Body.Should().Be("yo email EMBEDDED TEST");
    }

    [Fact]
    public void Using_Template_From_Root_Configured_Embedded_Resource()
    {
        EmbeddedTemplates.Configure(Assembly.GetExecutingAssembly(), "FluentEmail.Core.Tests");
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingTemplateFromEmbedded("test-embedded.txt", new { Test = "EMBEDDED TEST" });

        email.Data.Body.Should().Be("yo email EMBEDDED TEST");
    }
		
    [Fact]
    public void Using_Template_From_Configured_Embedded_Resource()
    {
        EmbeddedTemplates.Configure(Assembly.GetExecutingAssembly(), "FluentEmail.Core.Tests.EmailTemplates");
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingTemplateFromEmbedded("test-embedded.txt", new { Test = "EMBEDDED TEST" });

        email.Data.Body.Should().Be("yo email EMBEDDED TEST");
    }
		
    [Fact]
    public void New_Anonymous_Model_Template_From_File_Matches()
    {
        var email = new Email(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENT EMAIL" });

        email.Data.Body.Should().Be("yo email FLUENT EMAIL");
    }

    [Fact]
    public void New_Using_Template_From_Not_Existing_Culture_File_Using_Default_Template()
    {
        var culture = new CultureInfo("fr-FR");
        var email = new Email(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENT EMAIL", culture }, culture);

        email.Data.Body.Should().Be("yo email FLUENT EMAIL");
    }

    [Fact]
    public void New_Using_Template_From_Culture_File()
    {
        var culture = new CultureInfo("he-IL");
        var email = new Email(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENT EMAIL" }, culture);

        email.Data.Body.Should().Be("hebrew email FLUENT EMAIL");
    }

    [Fact]
    public void New_Using_Template_From_Current_Culture_File()
    {
        var culture = new CultureInfo("he-IL");
        var email = new Email(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new {Test = "FLUENT EMAIL"}, culture);

        email.Data.Body.Should().Be("hebrew email FLUENT EMAIL");
    }



    [Fact]
    public void New_Set_Custom_Template()
    {
        var template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

        var email = new Email(new TestTemplate(), new SaveToDiskSender("/"), FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingTemplate(template, new { Name = "LUKE", Numbers = (string[])["1", "2", "3"] });

        email.Data.Body.Should().Be("custom template");
    }

    [Fact]
    public void New_Using_Template_From_Embedded_Resource()
    {
        var email = new Email(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .UsingTemplateFromEmbedded("FluentEmail.Core.Tests.test-embedded.txt", new { Test = "EMBEDDED TEST" }, ThisAssembly());

        email.Data.Body.Should().Be("yo email EMBEDDED TEST");
    }		
}

public class TestTemplate : ITemplateRenderer
{
    public string Parse<T>(string template, T model, bool isHtml = true)
    {
        return "custom template";
    }

    public Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
    {
        return Task.FromResult(Parse(template, model, isHtml));
    }
}