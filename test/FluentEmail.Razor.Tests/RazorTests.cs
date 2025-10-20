using FluentEmail.Core;
using Xunit;
using AwesomeAssertions;
using System;
using System.Dynamic;
using System.IO;

namespace FluentEmail.Razor.Tests
{
	public class RazorTests
    {
        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";

        [Fact]
        public void Anonymous_Model_With_List_Template_Matches()
        {
            string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

            var email = new Email(fromEmail)
                {
                    Renderer = new RazorRenderer()
                }
                .To(toEmail)
                .Subject(subject)
                .UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

            email.Data.Body.Should().Be("sup LUKE here is a list 123");
        }

        [Fact]
        public void Reuse_Cached_Templates()
        {
            string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";
            string template2 = "sup @Model.Name this is the second template";

            for (var i = 0; i < 10; i++)
            {
                var email = new Email(fromEmail)
                    {
                        Renderer = new RazorRenderer()
                    }
                    .To(toEmail)
                    .Subject(subject)
                    .UsingTemplate(template, new { Name = i, Numbers = new string[] { "1", "2", "3" } });

                email.Data.Body.Should().Be("sup " + i + " here is a list 123");

                var email2 = new Email(fromEmail)
                    {
                        Renderer = new RazorRenderer()
                    }
                    .To(toEmail)
                    .Subject(subject)
                    .UsingTemplate(template2, new { Name = i });

                email2.Data.Body.Should().Be("sup " + i + " this is the second template");
            }
        }

        [Fact]
        public void New_Anonymous_Model_Template_Matches()
        {
            string template = "sup @Model.Name";

            var email = new Email(fromEmail)
                {
                    Renderer = new RazorRenderer()
                }
                .To(toEmail)
                .Subject(subject)
                .UsingTemplate(template, new { Name = "LUKE" });

            email.Data.Body.Should().Be("sup LUKE");
        }

        [Fact]
        public void New_Anonymous_Model_With_List_Template_Matches()
        {
            string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

            var email = new Email(fromEmail)
                {
                    Renderer = new RazorRenderer()
                }
                .To(toEmail)
                .Subject(subject)
                .UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

            email.Data.Body.Should().Be("sup LUKE here is a list 123");
        }

        [Fact]
        public void New_Reuse_Cached_Templates()
        {
            string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";
            string template2 = "sup @Model.Name this is the second template";

            for (var i = 0; i < 10; i++)
            {
                var email = new Email(fromEmail)
                    {
                        Renderer = new RazorRenderer()
                    }
                    .To(toEmail)
                    .Subject(subject)
                    .UsingTemplate(template, new { Name = i, Numbers = new string[] { "1", "2", "3" } });

                email.Data.Body.Should().Be("sup " + i + " here is a list 123");

                var email2 = new Email(fromEmail)
                    {
                        Renderer = new RazorRenderer()
                    }
                    .To(toEmail)
                    .Subject(subject)
                    .UsingTemplate(template2, new { Name = i });

                email2.Data.Body.Should().Be("sup " + i + " this is the second template");
            }
        }


	    [Fact]
	    public void Should_be_able_to_use_project_layout_with_viewbag()
	    {
		    var projectRoot = Directory.GetCurrentDirectory();
		    Email.DefaultRenderer = new RazorRenderer(projectRoot);

		    string template = @"
@{
	Layout = ""./Shared/_Layout.cshtml"";
}
sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

			dynamic viewBag = new ExpandoObject();
			viewBag.Title = "Hello!";
            var email = new Email(fromEmail)
                {
                    Renderer = new RazorRenderer()
                }
			    .To(toEmail)
			    .Subject(subject)
			    .UsingTemplate(template, new ViewModelWithViewBag{ Name = "LUKE", Numbers = new[] { "1", "2", "3" }, ViewBag = viewBag});

		    email.Data.Body.Should().Be($"<h1>Hello!</h1>{Environment.NewLine}<div>{Environment.NewLine}sup LUKE here is a list 123</div>");
	    }

	    [Fact]
	    public void Should_be_able_to_use_embedded_layout_with_viewbag()
	    {
		    string template = @"
@{
	Layout = ""_EmbeddedLayout.cshtml"";
}
sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

		    dynamic viewBag = new ExpandoObject();
		    viewBag.Title = "Hello!";
            var email = new Email(fromEmail)
                {
                    Renderer = new RazorRenderer(typeof(RazorTests))
                }
			    .To(toEmail)
			    .Subject(subject)
			    .UsingTemplate(template, new ViewModelWithViewBag{ Name = "LUKE", Numbers = new[] { "1", "2", "3" }, ViewBag = viewBag});

		    email.Data.Body.Should().Be($"<h2>Hello!</h2>{Environment.NewLine}<div>{Environment.NewLine}sup LUKE here is a list 123</div>");
	    }
    }

	public class ViewModelWithViewBag : IViewBagModel
	{
		public ExpandoObject ViewBag { get; set;}
		public string Name {get;set;}
		public string[] Numbers {get;set;}
	}
}
