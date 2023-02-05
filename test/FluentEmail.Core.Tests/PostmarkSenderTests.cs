using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentEmail.Core;
using NUnit.Framework;

namespace FluentEmail.Postmark.Tests
{
    [Ignore("missing Postmark API key")]
    public class WithTestApiToken
    {
        
        const string apiKey = "postmark-api-key"; // TODO: Put your API key here
        const string toEmail = "test@blackhole.postmarkapp.com";
        const string toEmailHash = "test+test@blackhole.postmarkapp.com";
        const string toEmailHash2 = "test+second@blackhole.postmarkapp.com";
        const string toName = "Test Name";
        const string fromEmail = "insert-sender-signature-here";
        const string fromName = "from name";
        const string fromEmailHash = "insert-sender-signature-here";

        [Test]
        public void SimpleMailFromCodeSync()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = Email
                .From(fromEmail, fromName)
                .To(toEmail)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?")
                .Send();

            response.Successful.Should().BeTrue();
        }

        [Test]
        public async Task SimpleMailFromCode()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = await Email
                .From(fromEmail)
                .To(toEmail)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?")
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
            response.MessageId.Should().NotBeNullOrEmpty();
            response.ErrorMessages.Should().BeEmpty();
        }

        [Test]
        public async Task SimpleMailFromCodeWithAddressesWithPlus()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = await Email
                .From(fromEmailHash)
                .To(toEmailHash)
                .ReplyTo(toEmailHash2)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?")
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
            response.MessageId.Should().NotBeNullOrEmpty();
            response.ErrorMessages.Should().BeEmpty();
        }

        [Test]
        public async Task SimpleMailReplyTo()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = await Email
                .From(fromEmail)
                .To(toEmail)
                .ReplyTo(fromEmail)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?")
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
            response.MessageId.Should().NotBeNullOrEmpty();
            response.ErrorMessages.Should().BeEmpty();
        }

        [Test]
        public async Task SimpleMailWithNameFromCode()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = await Email
                .From(fromEmail, fromName)
                .To(toEmail)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?")
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
            response.MessageId.Should().NotBeNullOrEmpty();
            response.ErrorMessages.Should().BeEmpty();
        }

        [Test]
        public async Task SimpleHtmlMailFromCode()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = await Email
                .From(fromEmail, fromName)
                .To(toEmail)
                .Subject("hows it going bob")
                .Body("<html><body><h1>Test</h1></body></html>", true)
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
        }

        [Test]
        public async Task SimpleMailWithAttachmentFromCode()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = await Email
                .From(fromEmail, fromName)
                .To(toEmail)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?")
                .Attach(new Core.Models.Attachment()
                {
                    Filename = "test.txt",
                    Data = new System.IO.MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }),
                    ContentType = "application/octet-stream"
                })
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
            response.MessageId.Should().NotBeNullOrEmpty();
            response.ErrorMessages.Should().BeEmpty();
        }

        [Test]
        public async Task SimpleHtmlMailWithAlternFromCode()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = await Email
                .From(fromEmail, fromName)
                .To(toEmail)
                .Subject("hows it going bob")
                .Body("<html><body><h1>Test</h1></body></html>", true)
                .PlaintextAlternativeBody("Test")
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
        }

        [Test]
        public async Task SimpleMailFromCodeWithOpts()
        {
            var opts = new PostmarkSenderOptions(apiKey);
            opts.TrackOpens = true;
            opts.TrackLinks = PostmarkDotNet.LinkTrackingOptions.HtmlAndText;
            opts.Tag = "unittest";
            opts.Metadata = new Dictionary<string, string>() { { "key", "example" } };
            Email.DefaultSender = new PostmarkSender(opts);

            var response = await Email
                .From(fromEmail, fromName)
                .To(toEmail)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?")
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
        }

        [Test]
        public async Task SimpleMailFromCodeWithLowPrio()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = await Email
                .From(fromEmail, fromName)
                .To(toEmail)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?")
                .LowPriority()
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
        }

        [Test]
        public async Task SimpleMailFromCodeWithHighPrio()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = await Email
                .From(fromEmail, fromName)
                .To(toEmail)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?")
                .HighPriority()
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
        }

        [Test]
        public async Task SimpleMailFromCodeWithHeaders()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var response = await Email
                .From(fromEmail, fromName)
                .To(toEmail)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?")
                .Header("X-Random-Useless-Header", "SomeValue")
                .Header("X-Another-Random-Useless-Header", "AnotherValue")
                .SendAsync()
                .ConfigureAwait(false);

            response.Successful.Should().BeTrue();
        }

        [Test]
        public void SenderNullServerToken()
        {
            Func<PostmarkSender> fn = () => new PostmarkSender((string)null!);
            fn.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void OptionsNullServerToken()
        {
            Func<PostmarkSenderOptions> fn = () => new PostmarkSenderOptions(null!);
            fn.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void NullOptions()
        {
            Func<PostmarkSender> fn = () => new PostmarkSender((PostmarkSenderOptions)null!);
            fn.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void SendNull()
        {
            var sender = new PostmarkSender(apiKey);
            Func<Task> fn = async () => await sender.SendAsync(null!).ConfigureAwait(false);
            fn.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task TooManyRecipients()
        {
            Email.DefaultSender = new PostmarkSender(apiKey);

            var mail = Email
                .From(fromEmail, fromName)
                .Subject("hows it going bob")
                .Body("yo dawg, sup?");

            var recipAdrs = new List<string>();
            for (var i = 0; i < 60; i++)
                recipAdrs.Add($"test{i}@blackhole.postmarkapp.com");

            var recips = recipAdrs.Select(s => new FluentEmail.Core.Models.Address(s)).ToList();
            mail.To(recips);

            Func<Task> act = async () => { await mail.SendAsync().ConfigureAwait(false); };
            await act.Should().ThrowAsync<ArgumentException>().ConfigureAwait(false);
        }
    }
}
