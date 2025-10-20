using FluentEmail.Postmark;
using System.Collections.Generic;

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

    [Fact]
    public void SimpleMailFromCodeSync()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");

        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("Whats up?");

        email.Sender = Sender;
            
        var response = email.Send();

        response.Successful.Should().BeTrue();
    }

    [Fact]
    public async Task SimpleMailFromCode()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");

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

    [Fact]
    public async Task SimpleMailFromCodeWithAddressesWithPlus()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");

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

    [Fact]
    public async Task SimpleMailReplyTo()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");

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

    [Fact]
    public async Task SimpleMailWithNameFromCode()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        
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

    [Fact]
    public async Task SimpleHtmlMailFromCode()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        
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

    [Fact]
    public async Task SimpleMailWithAttachmentFromCode()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        
        Email.DefaultSender = new PostmarkSender(_apiKey);

        var email = Email
            .From(_fromEmail, FromName)
            .To(ToEmail)
            .Subject("hows it going bob")
            .Body("Whats up?")
            .Attach(new Attachment()
            {
                Filename = "test.txt",
                Data = new MemoryStream([0, 1, 2, 3, 4, 5, 6, 7]),
                ContentType = "application/octet-stream"
            });

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
        response.MessageId.Should().NotBeNullOrEmpty();
        response.ErrorMessages.Should().BeEmpty();
    }

    [Fact]
    public async Task SimpleHtmlMailWithAlternateFromCode()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        
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

    [Fact]
    public async Task SimpleMailFromCodeWithOpts()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        
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

    [Fact]
    public async Task SimpleMailFromCodeWithLowPriority()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");

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

    [Fact]
    public async Task SimpleMailFromCodeWithHighPriority()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");

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

    [Fact]
    public async Task SimpleMailFromCodeWithHeaders()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        
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

    [Fact]
    public void SenderNullServerToken()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        var fn = () => new PostmarkSender((string)null!);
        fn.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void OptionsNullServerToken()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        var fn = () => new PostmarkSenderOptions(null!);
        fn.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void NullOptions()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        var fn = () => new PostmarkSender((PostmarkSenderOptions)null!);
        fn.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SendNull()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        var sender = new PostmarkSender(_apiKey);
        Func<Task> fn = async () => await sender.SendAsync(null!).ConfigureAwait(false);
        fn.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task TooManyRecipients()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Postmark Credentials");
        
        var email = Email
            .From(_fromEmail, FromName)
            .Subject("hows it going bob")
            .Body("Whats up?");

        email.Sender = Sender;

        var recipientAddresses = new List<string>();
        for (var i = 0; i < 60; i++)
            // ReSharper disable twice StringLiteralTypo
            recipientAddresses.Add($"test{i}@blackhole.postmarkapp.com");

        var recipients = recipientAddresses.Select(s => new Address(s)).ToList();
        email.To(recipients);

        var act = async () => { await email.SendAsync(); };
        await act.Should().ThrowAsync<ArgumentException>();
    }
}