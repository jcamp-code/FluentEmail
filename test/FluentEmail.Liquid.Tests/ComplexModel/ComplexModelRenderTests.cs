using System;
using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using Fluid;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Xunit;

namespace FluentEmail.Liquid.Tests.ComplexModel
{
    public class ComplexModelRenderTests
    {
        [Fact]
        public void Can_Render_Complex_Model_Properties()
        {
            var model = new ParentModel
            {
                ParentName = new NameDetails { Firstname = "Luke", Surname = "Dinosaur" },
                ChildrenNames =
                [
                    new NameDetails { Firstname = "ChildFirstA", Surname = "ChildLastA" },
                    new NameDetails { Firstname = "ChildFirstB", Surname = "ChildLastB" }
                ]
            };

            var expected = @"
Parent: Luke
Children:

* ChildFirstA ChildLastA
* ChildFirstB ChildLastB
";

            var email = Email
                .From(TestData.FromEmail)
                .To(TestData.ToEmail)
                .Subject(TestData.Subject);
                
            email.Renderer = SetupRenderer();
                
            email.UsingTemplate(Template(), model);

            email.Data.Body.Should().Be(expected);
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private string Template()
        {
            return @"
Parent: {{ ParentName.Firstname }}
Children:
{% for Child in ChildrenNames %}
* {{ Child.Firstname }} {{ Child.Surname }}{% endfor %}
";
        }

        private static ITemplateRenderer SetupRenderer(
            IFileProvider fileProvider = null,
            Action<TemplateContext, object> configureTemplateContext = null)
        {
            var options = new LiquidRendererOptions
            {
                FileProvider = fileProvider,
                ConfigureTemplateContext = configureTemplateContext,
                TemplateOptions = new TemplateOptions { MemberAccessStrategy = new UnsafeMemberAccessStrategy() }
            };
            return new LiquidRenderer(Options.Create(options));
        }
    }
}