using System.IO;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Xunit;
using AwesomeAssertions;

namespace FluentEmail.Mailtrap.Tests
{
    public class MailtrapSenderTests
    {
        const string subject = "Mailtrap Email Test";
        const string body = "This email is testing the functionality of mailtrap.";

        private readonly string toEmail = Credentials.MailTrap.ToEmail ?? Credentials.ToEmail;
        private readonly string fromEmail = Credentials.MailTrap.FromEmail ?? Credentials.FromEmail;
        private readonly string host = Credentials.MailTrap.Host;
        private readonly string username = Credentials.MailTrap.User;
        private readonly int port = Credentials.MailTrap.Port ?? 587;
        private readonly string password = Credentials.MailTrap.Password;
        private readonly string apiHost = Credentials.MailTrap.ApiHost;
        private readonly string apiKey = Credentials.MailTrap.ApiKey;
        private readonly string templateid = Credentials.MailTrap.Template;
        
        public MailtrapSenderTests()
        {
            var sender = new MailtrapSender(username, password, host, port);
            Email.DefaultSender = sender;
        }

        [Fact(Skip="Missing credentials")]
        public void CanSendEmail()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body);

            var response = email.Send();

            (response.Successful).Should().BeTrue();
        }


        [Fact(Skip="Missing credentials")]
        public async Task CanSendEmailAsync()
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
                Filename = "mailtrapTest.txt"
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

        [Fact(Skip="Missing credentials")]
        public async Task CanSendEmailWithTemplate()
        {
            var sender = new MailtrapSender(username, apiKey, host, 587, apiHost);
            Email.DefaultSender = sender;
            var email = Email.From(fromEmail).To(toEmail);
            var response = await email.SendWithTemplateAsync(templateid, new { var1 = "Test", var2 = "VVVVVVVVVVVVV" });
            (response.Successful).Should().BeTrue();
        }
    }
}
