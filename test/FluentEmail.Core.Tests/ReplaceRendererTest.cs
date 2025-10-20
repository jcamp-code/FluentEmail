using FluentEmail.Core.Defaults;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Xunit;
using AwesomeAssertions;

namespace FluentEmail.Core.Tests
{
    public class ReplaceRendererTest
    {
        [Fact]
        public void ModelPropertyValueIsNull_Test()
        {
            ITemplateRenderer templateRenderer = new ReplaceRenderer();

            var address = new Address("james@test.com", "james");
            address.Name.Should().Be("james");
            var template = "this is name: ##Name##";
            templateRenderer.Parse(template, address).Should().Be("this is name: james");

            address.Name = null;
            templateRenderer.Parse(template, address).Should().Be("this is name: ");
        }
    }
}