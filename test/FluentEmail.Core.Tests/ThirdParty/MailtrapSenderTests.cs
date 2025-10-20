using FluentEmail.Mailtrap;

namespace FluentEmail.Core.Tests.ThirdParty;

public class MailtrapSenderTests
{
    private const string Subject = "Mailtrap Email Test";
    private const string Body = "This email is testing the functionality of mailtrap.";

    private readonly string _toEmail = Credentials.MailTrap.ToEmail ?? Credentials.ToEmail;
    private readonly string _fromEmail = Credentials.MailTrap.FromEmail ?? Credentials.FromEmail;
    private readonly string _host = Credentials.MailTrap.Host;
    private readonly string _username = Credentials.MailTrap.User;
    private readonly int _port = Credentials.MailTrap.Port ?? 587;
    private readonly string _password = Credentials.MailTrap.Password;
    private readonly string _apiHost = Credentials.MailTrap.ApiHost;
    private readonly string _apiKey = Credentials.MailTrap.ApiKey;
    private readonly string _templateId = Credentials.MailTrap.Template;

    private ISender Sender { get; }

    public MailtrapSenderTests()
    {
        if (!string.IsNullOrEmpty(_username)) Sender = new MailtrapSender(_username, _password, _host, _port);
    }

    [Fact]
    public void CanSendEmail()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_password), "No Mailtrap Credentials");
        
        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body);

        email.Sender = Sender;
        var response = email.Send();

        (response.Successful).Should().BeTrue();
    }


    [Fact]
    public async Task CanSendEmailAsync()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_password), "No Mailtrap Credentials");
        
        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body);

        email.Sender = Sender;
        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
    }

    [Fact]
    public async Task CanSendEmailWithAttachments()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_password), "No Mailtrap Credentials");
        
        var stream = new MemoryStream();
        var sw = new StreamWriter(stream);
        await sw.WriteLineAsync("Hey this is some text in an attachment");
        await sw.FlushAsync(TestContext.Current.CancellationToken);
        stream.Seek(0, SeekOrigin.Begin);

        var attachment = new Attachment
        {
            Data = stream,
            ContentType = "text/plain",
            Filename = "mailtrapTest.txt"
        };

        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body)
            .Attach(attachment);

        email.Sender = Sender;
        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
    }

    [Fact]
    public async Task CanSendEmailWithInlineImages()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_password), "No Mailtrap Credentials");

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

        (response.Successful).Should().BeTrue();
    }

    [Fact]
    public async Task CanSendEmailWithTemplate()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_apiKey), "No Mailtrap Credentials");
        
        var email = Email.From(_fromEmail).To(_toEmail);
        email.Sender = new MailtrapSender(_username, _apiKey, _host, 587, _apiHost);

        // ReSharper disable once StringLiteralTypo
        var response = await email.SendWithTemplateAsync(_templateId, new { var1 = "Test", var2 = "VVVVVVVVVVVVV" });
        
        (response.Successful).Should().BeTrue();
    }
}