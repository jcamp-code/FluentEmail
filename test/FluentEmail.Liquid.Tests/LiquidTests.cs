using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using FluentEmail.Core;

using Fluid;
using Fluid.Ast;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

using Xunit;
using AwesomeAssertions;
using FluentEmail.Core.Interfaces;

namespace FluentEmail.Liquid.Tests
{
	public class LiquidTests
    {
	    private const string ToEmail = "bob@test.com";
        private const string FromEmail = "johno@test.com";
        private const string Subject = "sup dawg";

        private static ITemplateRenderer SetupRenderer(
            IFileProvider fileProvider = null,
            Action<TemplateContext, object> configureTemplateContext = null,
            Action<LiquidParser> configureParser = null)
        {
            var options = new LiquidRendererOptions
            {
                FileProvider = fileProvider,
                ConfigureTemplateContext = configureTemplateContext,
                ConfigureParser = configureParser
            };
            return new LiquidRenderer(Options.Create(options));
        }

        [Fact]
        public void Model_With_List_Template_Matches()
        {
            const string template = "sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";

            var email = Email
                .From(FromEmail)
                .To(ToEmail)
                .Subject(Subject);
                email.Renderer = SetupRenderer();
                email.UsingTemplate(template, new ViewModel { Name = "LUKE", Numbers = ["1", "2", "3"] });

            email.Data.Body.Should().Be("sup LUKE here is a list 123");
        }

        [Fact]
        public void Custom_Context_Values()
        {
            var renderer = SetupRenderer(new NullFileProvider(), (context, _) =>
            {
                context.SetValue("FirstName", "Samantha");
                context.SetValue("IntegerNumbers", (int[])[3, 2, 1]);
            });

            const string template = "sup {{ FirstName }} here is a list {% for i in IntegerNumbers %}{{ i }}{% endfor %}";

            var email = Email
                .From(FromEmail)
                .To(ToEmail)
                .Subject(Subject);
            email.Renderer = renderer;
            email.UsingTemplate(template, new ViewModel { Name = "LUKE", Numbers = ["1", "2", "3"] });

            email.Data.Body.Should().Be("sup Samantha here is a list 321");
        }

        // currently not cached as Fluid is so fast, but can be added later
        [Fact]
        public void Reuse_Cached_Templates()
        {
            const string template = "sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";
            const string template2 = "sup {{ Name }} this is the second template";

            for (var i = 0; i < 10; i++)
            {
                var email = Email
                    .From(FromEmail)
                    .To(ToEmail)
                    .Subject(Subject);
                email.Renderer = SetupRenderer();
                email.UsingTemplate(template, new ViewModel { Name = i.ToString(), Numbers = ["1", "2", "3"] });

                email.Data.Body.Should().Be("sup " + i + " here is a list 123");

                var email2 = Email
                    .From(FromEmail)
                    .To(ToEmail)
                    .Subject(Subject);
                email2.Renderer = SetupRenderer();
                email2.UsingTemplate(template2, new ViewModel { Name = i.ToString() });

                email2.Data.Body.Should().Be("sup " + i + " this is the second template");
            }
        }

        [Fact]
        public void New_Model_Template_Matches()
        {
            const string template = "sup {{ Name }}";

            var email = new Email(FromEmail)
                .To(ToEmail)
                .Subject(Subject);
            email.Renderer = SetupRenderer();
            email.UsingTemplate(template, new ViewModel { Name = "LUKE" });

            email.Data.Body.Should().Be("sup LUKE");
        }

        [Fact]
        public void New_Model_With_List_Template_Matches()
        {
            const string template = "sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";

            var email = new Email(FromEmail)
                .To(ToEmail)
                .Subject(Subject);
            email.Renderer = SetupRenderer();
            email.UsingTemplate(template, new ViewModel { Name = "LUKE", Numbers = ["1", "2", "3"] });

            email.Data.Body.Should().Be("sup LUKE here is a list 123");
        }

        // currently not cached as Fluid is so fast, but can be added later
        [Fact]
        public void New_Reuse_Cached_Templates()
        {
            const string template = "sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";
            const string template2 = "sup {{ Name }} this is the second template";

            for (var i = 0; i < 10; i++)
            {
                var email = new Email(FromEmail)
                    .To(ToEmail)
                    .Subject(Subject);
                email.Renderer = SetupRenderer();
                email.UsingTemplate(template, new ViewModel { Name = i.ToString(), Numbers = ["1", "2", "3"] });

                email.Data.Body.Should().Be("sup " + i + " here is a list 123");

                var email2 = new Email(FromEmail)
                    .To(ToEmail)
                    .Subject(Subject);
                email2.Renderer = SetupRenderer();
                email2.UsingTemplate(template2, new ViewModel { Name = i.ToString() });

                email2.Data.Body.Should().Be("sup " + i + " this is the second template");
            }
        }

	    [Fact]
	    public void Should_be_able_to_use_project_layout()
	    {
            var renderer = SetupRenderer(new PhysicalFileProvider(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName, "EmailTemplates")));

		    const string template = @"{% layout '_layout.liquid' %}
sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";

			var email = new Email(FromEmail)
			    .To(ToEmail)
			    .Subject(Subject);
            email.Renderer = renderer;
            email.UsingTemplate(template, new ViewModel{ Name = "LUKE", Numbers = ["1", "2", "3"] });

		    email.Data.Body.Should().Be($"<h1>Hello!</h1>{Environment.NewLine}<div>{Environment.NewLine}sup LUKE here is a list 123</div>");
	    }

        [Fact]
        public void Should_be_able_to_use_embedded_layout()
        {
            var renderer = SetupRenderer(new EmbeddedFileProvider(typeof(LiquidTests).Assembly, "FluentEmail.Liquid.Tests.EmailTemplates"));

            const string template = @"{% layout '_embedded.liquid' %}
sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";

            var email = new Email(FromEmail)
                .To(ToEmail)
                .Subject(Subject);
            email.Renderer = renderer;
            email
                .UsingTemplate(template, new ViewModel{ Name = "LUKE", Numbers = ["1", "2", "3"] });

            email.Data.Body.Should().Be($"<h2>Hello!</h2>{Environment.NewLine}<div>{Environment.NewLine}sup LUKE here is a list 123</div>");
        }

        [Fact]
        public void Should_be_able_to_configure_parser()
        {
            var renderer = SetupRenderer(
                new EmbeddedFileProvider(typeof(LiquidTests).Assembly, "FluentEmail.Liquid.Tests.EmailTemplates"),
                configureParser: parser => parser.RegisterExpressionTag("testTag", TestTag)
            );

            const string template = "sup {{ Name }} here is a custom tag: {% testTag 'test' %}";

            var email = Email
                .From(FromEmail)
                .To(ToEmail)
                .Subject(Subject);
            email.Renderer = renderer;
            email.UsingTemplate(template, new ViewModel { Name = "LUKE" });

            email.Data.Body.Should().Be("sup LUKE here is a custom tag: Hello from custom tag test");

            static async ValueTask<Completion> TestTag(Expression pathExpression, TextWriter writer, TextEncoder encoder, TemplateContext context)
            {
                var tagParameterValue = await pathExpression.EvaluateAsync(context);
                await writer.WriteAsync($"Hello from custom tag {tagParameterValue.ToStringValue()}");
                return Completion.Normal;
            }
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class ViewModel
        {
            public string Name { get; set; }
            public string[] Numbers { get; set; }
        }
    }
}
