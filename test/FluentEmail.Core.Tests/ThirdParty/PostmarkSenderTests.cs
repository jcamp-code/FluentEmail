using System;
using System.Collections.Generic;
using System.Linq;
using FluentEmail.Postmark;

namespace FluentEmail.Core.Tests.ThirdParty;

public class PostmarkSenderTests
{
    private readonly string _apiKey = Credentials.Postmark.ApiKey;

    private const string ToEmail = "test@blackhole.postmarkapp.com";
    private const string ToEmailHash = "test+test@blackhole.postmarkapp.com";
    private const string ToEmailHash2 = "test+second@blackhole.postmarkapp.com";
    private readonly string _fromEmail = Credentials.Postmark.FromEmail ?? Credentials.FromEmail;
    private const string FromName = "from name";
    private readonly string _fromEmailHash = Credentials.Postmark.FromEmail ?? Credentials.FromEmail;

    private ISender Sender { get; }

    public PostmarkSenderTests()
    {
        if (!string.IsNullOrEmpty(_apiKey)) Sender = new PostmarkSender(_apiKey);
    }

    [Test]
    public void SimpleMailFromCodeSync()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");

        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("Whats up?");

        email.Sender = Sender;
            
        var response = email.Send();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task SimpleMailFromCode()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");

        Email.DefaultSender = new PostmarkSender(_apiKey);

        var email = Email
            .From(_fromEmail)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("Whats up?");

        email.Sender = Sender;

        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
        response.MessageId.Should().NotBeNullOrEmpty();
        response.ErrorMessages.Should().BeEmpty();
    }

    [Test]
    public async Task SimpleMailFromCodeWithAddressesWithPlus()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");

        var email = Email
            .From(_fromEmailHash)
            .To(ToEmailHash)
            .ReplyTo(ToEmailHash2)
            .Subject("hows it going bob")
            .Body("Whats up?");

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
        response.MessageId.Should().NotBeNullOrEmpty();
        response.ErrorMessages.Should().BeEmpty();
    }

    [Test]
    public async Task SimpleMailReplyTo()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");

        var email = Email
            .From(_fromEmail)
            .To(ToEmail)
            .ReplyTo(_fromEmail)
            .Subject("hows it going bob")
            .Body("Whats up?");

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
        response.MessageId.Should().NotBeNullOrEmpty();
        response.ErrorMessages.Should().BeEmpty();
    }

    [Test]
    public async Task SimpleMailWithNameFromCode()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        
        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("Whats up?");

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
        response.MessageId.Should().NotBeNullOrEmpty();
        response.ErrorMessages.Should().BeEmpty();
    }

    [Test]
    public async Task SimpleHtmlMailFromCode()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        
        Email.DefaultSender = new PostmarkSender(_apiKey);

        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("<html><body><h1>Test</h1></body></html>", true);

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task SimpleMailWithAttachmentFromCode()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        
        Email.DefaultSender = new PostmarkSender(_apiKey);

        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("Whats up?")
            .Attach(new Attachment()
            {
                Filename = "test.txt",
                Data = new System.IO.MemoryStream([0, 1, 2, 3, 4, 5, 6, 7]),
                ContentType = "application/octet-stream"
            });

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
        response.MessageId.Should().NotBeNullOrEmpty();
        response.ErrorMessages.Should().BeEmpty();
    }

    [Test]
    public async Task SimpleHtmlMailWithAlternateFromCode()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        
        Email.DefaultSender = new PostmarkSender(_apiKey);

        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("<html><body><h1>Test</h1></body></html>", true)
            .PlaintextAlternativeBody("Test");

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task SimpleMailFromCodeWithOpts()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        
        var opts = new PostmarkSenderOptions(_apiKey)
        {
            TrackOpens = true,
            TrackLinks = PostmarkDotNet.LinkTrackingOptions.HtmlAndText,
            Tag = "unittest",
            Metadata = new Dictionary<string, string>() { { "key", "example" } }
        };
        var sender = new PostmarkSender(opts);

        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("Whats up?");

        email.Sender = sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task SimpleMailFromCodeWithLowPriority()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");

        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("Whats up?")
            .LowPriority();

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task SimpleMailFromCodeWithHighPriority()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");

        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("Whats up?")
            .HighPriority();

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task SimpleMailFromCodeWithHeaders()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        
        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("Whats up?")
            .Header("X-Random-Useless-Header", "SomeValue")
            .Header("X-Another-Random-Useless-Header", "AnotherValue");
        
        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public void SenderNullServerToken()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        Func<PostmarkSender> fn = () => new PostmarkSender((string)null!);
        fn.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void OptionsNullServerToken()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        Func<PostmarkSenderOptions> fn = () => new PostmarkSenderOptions(null!);
        fn.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void NullOptions()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        Func<PostmarkSender> fn = () => new PostmarkSender((PostmarkSenderOptions)null!);
        fn.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void SendNull()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        var sender = new PostmarkSender(_apiKey);
        Func<Task> fn = async () => await sender.SendAsync(null!).ConfigureAwait(false);
        fn.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task TooManyRecipients()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Postmark Credentials");
        
        var email = Email
            .From(_fromEmail, FromName)
            .Subject("hows it going bob")
            .Body("Whats up?");

        email.Sender = Sender;

        var recipientAddresses = new List<string>();
        for (var i = 0; i < 60; i++)
            // ReSharper disable StringLiteralTypo
            recipientAddresses.Add($"test{i}@blackhole.postmarkapp.com");

        var recipients = recipientAddresses.Select(s => new Address(s)).ToList();
        email.To(recipients);

        Func<Task> act = async () => { await email.SendAsync(); };
        await act.Should().ThrowAsync<ArgumentException>();
    }
}