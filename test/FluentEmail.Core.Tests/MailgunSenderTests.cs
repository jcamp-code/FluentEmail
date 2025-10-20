using System.IO;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Xunit;
using AwesomeAssertions;
using Newtonsoft.Json;

namespace FluentEmail.Mailgun.Tests
{
    public class MailgunSenderTests
    {
        const string toEmail = "bentest1@mailinator.com";
        const string fromEmail = "ben@test.com";
        const string subject = "Attachment Tests";
        const string body = "This email is testing the attachment functionality of MailGun.";

        public MailgunSenderTests()
        {
            var sender = new MailgunSender("<name>", "<key>");
            Email.DefaultSender = sender;
        }

        [Fact(Skip="Missing credentials")]
        public async Task CanSendEmail()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body);

            var response = await email.SendAsync();

            (response.Successful).Should().BeTrue();
        }

        [Fact(Skip="Missing credentials")]
        public async Task GetMessageIdInResponse()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body);

            var response = await email.SendAsync();

            (response.Successful).Should().BeTrue();
            (response.MessageId).Should().NotBeEmpty();
        }

        [Fact(Skip="Missing credentials")]
        public async Task CanSendEmailWithTag()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body)
                .Tag("test");

            var response = await email.SendAsync();

            (response.Successful).Should().BeTrue();
        }

        [Fact(Skip="Missing credentials")]
        public async Task CanSendEmailWithVariables()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body)
                .Header("X-Mailgun-Variables", JsonConvert.SerializeObject(new Variable { Var1 = "Test"}));

            var response = await email.SendAsync();

            (response.Successful).Should().BeTrue();
        }

        [Fact(Skip="Missing credentials")]
        public async Task CanSendEmailWithAttachments()
        {
            var stream = new MemoryStream();
            var sw = new StreamWriter(stream);
            sw.WriteLine("Hey this is some text in an attachment");
            sw.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            var attachment = new Attachment
            {
                Data = stream,
                ContentType = "text/plain",
                Filename = "mailgunTest.txt"
            };

            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body)
                .Attach(attachment);

            var response = await email.SendAsync();

            (response.Successful).Should().BeTrue();
        }

        [Fact(Skip="Missing credentials")]
        public async Task CanSendEmailWithInlineImages()
        {
            using (var stream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "logotest.png")}"))
            {
                var attachment = new Attachment
                {
                    IsInline = true,
                    Data = stream,
                    ContentType = "image/png",
                    Filename = "logotest.png"
                };

                var email = Email
                    .From(fromEmail)
                    .To(toEmail)
                    .Subject(subject)
                    .Body("<html>Inline image here: <img src=\"cid:logotest.png\">" +
                          "<p>You should see an image without an attachment, or without a download prompt, depending on the email client.</p></html>", true)
                    .Attach(attachment);

                var response = await email.SendAsync();

                (response.Successful).Should().BeTrue();
            }
        }
        
        // [Fact]
        // public async Task CanSendEmailWithTemplate()
        // {
        //     var email = Email
        //         .From(fromEmail)
        //         .To(toEmail)
        //         .Subject(subject);
        //
        //     var response = await email.SendWithTemplateAsync("test-template", new { var1 = "Test" });
        //
        //     (response.Successful).Should().BeTrue();
        // }

        class Variable
        {
            public string Var1 { get; set; }
        }
    }
}