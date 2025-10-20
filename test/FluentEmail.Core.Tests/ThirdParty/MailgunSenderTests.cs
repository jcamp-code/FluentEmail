using System.IO;
using FluentEmail.Mailgun;
using Newtonsoft.Json;

namespace FluentEmail.Core.Tests.ThirdParty;

public class MailgunSenderTests
{
    private readonly string _toEmail = Credentials.Mailgun.ToEmail ?? Credentials.ToEmail;
    private readonly string _fromEmail = Credentials.Mailgun.FromEmail ?? Credentials.FromEmail;
    private readonly string _apiKey = Credentials.Mailgun.ApiKey;
    private readonly string _domain = Credentials.Mailgun.Domain;
    private const string Subject = "Attachment Tests";
    private const string Body = "This email is testing the attachment functionality of MailGun.";

    private ISender Sender { get; }

    public MailgunSenderTests()
    {
        if (!string.IsNullOrEmpty(_apiKey)) Sender = new MailgunSender(_domain, _apiKey);
    }

    [Test]
    public async Task CanSendEmail()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Mailgun Credentials");
        
        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body);

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task GetMessageIdInResponse()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Mailgun Credentials");
        
        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body);

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
        response.MessageId.Should().NotBeEmpty();
    }

    [Test]
    public async Task CanSendEmailWithTag()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Mailgun Credentials");
        
        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body)
            .Tag("test");

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task CanSendEmailWithVariables()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Mailgun Credentials");
        
        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body)
            .Header("X-Mailgun-Variables", JsonConvert.SerializeObject(new Variable { Var1 = "Test"}));

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task CanSendEmailWithAttachments()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Mailgun Credentials");
        
        var stream = new MemoryStream();
        var sw = new StreamWriter(stream);
        await sw.WriteLineAsync("Hey this is some text in an attachment");
        await sw.FlushAsync();
        stream.Seek(0, SeekOrigin.Begin);

        var attachment = new Attachment
        {
            Data = stream,
            ContentType = "text/plain",
            Filename = "mailgunTest.txt"
        };

        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body)
            .Attach(attachment);
        
        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task CanSendEmailWithInlineImages()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Mailgun Credentials");

        await using var stream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "logotest.png")}");
        var attachment = new Attachment
        {
            IsInline = true,
            Data = stream,
            ContentType = "image/png",
            Filename = "logotest.png"
        };

        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body("<html>Inline image here: <img src=\"cid:logotest.png\">" +
                  "<p>You should see an image without an attachment, or without a download prompt, depending on the email client.</p></html>", true)
            .Attach(attachment);

        email.Sender = Sender;
        var response = await email.SendAsync();

        response.Successful.Should().BeTrue();
    }

    [Test]
    public async Task CanSendEmailWithTemplate()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Mailgun Credentials");
        if (string.IsNullOrEmpty(Credentials.Mailgun.Template)) Skip.Test("No Mailgun Template");

        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject);

        email.Sender = Sender;
        var response = await email.SendWithTemplateAsync("test-template", new { var1 = "Test" });

        response.Successful.Should().BeTrue();
    }

    private class Variable
    {
        public string Var1 { get; set; }
    }
}