// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedAutoPropertyAccessor.Local

using System;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Liquid;
using Fluid;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Xunit;
using VerifyTests;
using VerifyXunit;

namespace FluentEmail.Bootstrap.Tests;

public class BootstrapTests
{
    private const string ToEmail = "bob@test.com";
    private const string FromEmail = "johno@test.com";
    private const string Subject = "sup dawg";
    private readonly VerifySettings _settings = new();

    public BootstrapTests()
    {
        _settings.ScrubLinesContaining("Compiled with Bootstrap Email DotNet");
        _settings.DisableDiff();
    }

    private static ITemplateRenderer SetupRenderer(
        IFileProvider fileProvider = null,
        Action<TemplateContext, object> configureTemplateContext = null)
    {
        var options = new LiquidRendererOptions
        {
            FileProvider = fileProvider,
            ConfigureTemplateContext = configureTemplateContext,
        };
        return new LiquidRenderer(Options.Create(options));
    }

    [Fact]
    public Task CompileBootstrap_Compiles()
    {
        var template = """
                       <html>
                         <body class="bg-light">
                         Hi {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}
                         </body>
                       </html>
                       """;
        var email = new Email(FromEmail)
            {
                Renderer = SetupRenderer()
            }
            .To(ToEmail)
            .Subject(Subject)
            .UsingTemplate(template, new ViewModel { Name = "LUKE", Numbers = ["1", "2", "3"] })
            .CompileBootstrap();

        return Verifier.Verify(email.Data.Body, _settings);
    }

    [Fact]
    public Task UsingBootstrapBody_Compiles()
    {
        var body = """
                   <html>
                     <body class="bg-light">
                     This is simple text, no templating
                     </body>
                   </html>
                   """;
        var email = new Email(FromEmail)
            {
                Renderer = SetupRenderer()
            }
            .To(ToEmail)
            .Subject(Subject)
            .UsingBootstrapBody(body);

        return Verifier.Verify(email.Data.Body, _settings);
    }


    [Fact]
    public Task UsingBootstrapTemplate_Compiles()
    {
        var template = """
                       <html>
                         <body class="bg-light">
                         Hi {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}
                         </body>
                       </html>
                       """;
        var email = new Email(FromEmail)
            {
                Renderer = SetupRenderer()
            }
            .To(ToEmail)
            .Subject(Subject)
            .UsingBootstrapTemplate(template, new ViewModel { Name = "LUKE", Numbers = ["1", "2", "3"] });

        return Verifier.Verify(email.Data.Body, _settings);
    }

    private class ViewModel
    {
        public string Name { get; set; }
        public string[] Numbers { get; set; }
    }
}
