using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using Xunit;
using AwesomeAssertions;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace FluentEmail.Smtp.Tests
{
    // Note: XUnit runs tests in parallel by default. Use Collection attribute if sequential execution is needed.
    public class SmtpSenderTests
    {
        // Warning: To pass, an smtp listener must be running on localhost:25.

        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";
        const string body = "what be the hipitity hap?";

        private static IFluentEmail TestEmail => Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body);

        private readonly string tempDirectory;

        public SmtpSenderTests()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "EmailTest");
            
            var sender = new SmtpSender(() => new SmtpClient("localhost")
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = tempDirectory
            });

            Email.DefaultSender = sender;
            Directory.CreateDirectory(tempDirectory);
        }

        // Note: XUnit uses IDisposable for cleanup instead of TearDown.
        public void TearDown()
        {
            Directory.Delete(tempDirectory, true);
        }

        [Fact]
        public void CanSendEmail()
        {
            var email = TestEmail
                .Body("<h2>Test</h2>", true);

            var response = email.Send();

            var files = Directory.EnumerateFiles(tempDirectory, "*.eml");
            (response.Successful).Should().BeTrue();
            (files).Should().NotBeEmpty();
        }

        [Fact]
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

            var email = TestEmail
                .Attach(attachment);

            var response = await email.SendAsync();

            (response.Successful).Should().BeTrue();
            var files = Directory.EnumerateFiles(tempDirectory, "*.eml");
            (files).Should().NotBeEmpty();
        }

        [Fact]
        public async Task CanSendAsyncHtmlAndPlaintextTogether()
        {
            var email = TestEmail
                .Body("<h2>Test</h2><p>some body text</p>", true)
                .PlaintextAlternativeBody("Test - Some body text");

            var response = await email.SendAsync();

            (response.Successful).Should().BeTrue();
        }

        [Fact]
        public void CanSendHtmlAndPlaintextTogether()
        {
            var email = TestEmail
                .Body("<h2>Test</h2><p>some body text</p>", true)
                .PlaintextAlternativeBody("Test - Some body text");

            var response = email.Send();

            (response.Successful).Should().BeTrue();
        }

        [Fact]
        public void CancelSendIfCancelationRequested()
        {
            var email = TestEmail;

            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();

            var response = email.Send(tokenSource.Token);

            (response.Successful).Should().BeFalse();
        }
    }
}
