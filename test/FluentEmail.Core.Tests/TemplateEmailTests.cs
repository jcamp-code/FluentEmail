using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentEmail.Core.Defaults;
using FluentEmail.Core.Interfaces;
using Xunit;
using AwesomeAssertions;

namespace FluentEmail.Core.Tests
{
    public class TemplateEmailTests
    {
        private Assembly ThisAssembly() => this.GetType().GetTypeInfo().Assembly;
        const string toEmail = "bob@test.com";
		const string fromEmail = "johno@test.com";
		const string subject = "sup dawg";

		[Fact]
		public void Anonymous_Model_Template_From_File_Matches()
		{
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" });

			email.Data.Body.Should().Be("yo email FLUENTEMAIL");
		}

		[Fact]
		public void Using_Template_From_Not_Existing_Culture_File_Using_Default_Template()
		{
			var culture = new CultureInfo("fr-FR");
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL", culture }, culture);

			email.Data.Body.Should().Be("yo email FLUENTEMAIL");
		}

		[Fact]
		public void Using_Template_From_Culture_File()
		{
			var culture = new CultureInfo("he-IL");
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" }, culture);

			email.Data.Body.Should().Be("hebrew email FLUENTEMAIL");
		}

	    [Fact]
	    public void Using_Template_From_Current_Culture_File()
	    {
	        var culture = new CultureInfo("he-IL");
	        var email = Email
	            .From(fromEmail)
	            .To(toEmail)
	            .Subject(subject)
	            .UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new {Test = "FLUENTEMAIL"}, culture);

	        email.Data.Body.Should().Be("hebrew email FLUENTEMAIL");
	    }

	    [Fact]
		public void Anonymous_Model_Template_Matches()
		{
			string template = "sup ##Name##";

			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplate(template, new { Name = "LUKE" });

			email.Data.Body.Should().Be("sup LUKE");
		}



		[Fact]
		public void Set_Custom_Template()
		{
			string template = "sup ##Name## here is a list @foreach(var i in Model.Numbers) { @i }";

			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateEngine(new TestTemplate())
				.UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

			email.Data.Body.Should().Be("custom template");
		}

		[Fact]
		public void Using_Template_From_Embedded_Resource()
		{
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromEmbedded("FluentEmail.Core.Tests.test-embedded.txt", new { Test = "EMBEDDEDTEST" }, ThisAssembly());

			email.Data.Body.Should().Be("yo email EMBEDDEDTEST");
		}

		[Fact]
		public void Using_Template_From_Root_Configured_Embedded_Resource()
		{
			EmbeddedTemplates.Configure(Assembly.GetExecutingAssembly(), "FluentEmail.Core.Tests");
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromEmbedded("test-embedded.txt", new { Test = "EMBEDDEDTEST" });

			email.Data.Body.Should().Be("yo email EMBEDDEDTEST");
		}
		
		[Fact]
		public void Using_Template_From_Configured_Embedded_Resource()
		{
			EmbeddedTemplates.Configure(Assembly.GetExecutingAssembly(), "FluentEmail.Core.Tests.EmailTemplates");
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromEmbedded("test-embedded.txt", new { Test = "EMBEDDEDTEST" });

			email.Data.Body.Should().Be("yo email EMBEDDEDTEST");
		}
		
		[Fact]
		public void New_Anonymous_Model_Template_From_File_Matches()
		{
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" });

			email.Data.Body.Should().Be("yo email FLUENTEMAIL");
		}

		[Fact]
		public void New_Using_Template_From_Not_Existing_Culture_File_Using_Default_Template()
		{
			var culture = new CultureInfo("fr-FR");
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL", culture }, culture);

			email.Data.Body.Should().Be("yo email FLUENTEMAIL");
		}

		[Fact]
		public void New_Using_Template_From_Culture_File()
		{
			var culture = new CultureInfo("he-IL");
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" }, culture);

			email.Data.Body.Should().Be("hebrew email FLUENTEMAIL");
		}

	    [Fact]
	    public void New_Using_Template_From_Current_Culture_File()
	    {
	        var culture = new CultureInfo("he-IL");
	        var email = new Email(fromEmail)
	            .To(toEmail)
	            .Subject(subject)
	            .UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new {Test = "FLUENTEMAIL"}, culture);

	        email.Data.Body.Should().Be("hebrew email FLUENTEMAIL");
	    }



		[Fact]
		public void New_Set_Custom_Template()
		{
			string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

			var email = new Email(new TestTemplate(), new SaveToDiskSender("/"), fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

			email.Data.Body.Should().Be("custom template");
		}

		[Fact]
		public void New_Using_Template_From_Embedded_Resource()
		{
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromEmbedded("FluentEmail.Core.Tests.test-embedded.txt", new { Test = "EMBEDDEDTEST" }, ThisAssembly());

			email.Data.Body.Should().Be("yo email EMBEDDEDTEST");
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
}
