using AwesomeAssertions;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using FluentEmail.Mailtrap;
using System.IO;
using System.Threading.Tasks;
using TUnit.Core;

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
    private readonly string _templateid = Credentials.MailTrap.Template;

    private ISender Sender { get; set; }

    public MailtrapSenderTests()
    {
        if (!string.IsNullOrEmpty(_username)) Sender = new MailtrapSender(_username, _password, _host, _port);
    }

    [Test]
    public void CanSendEmail()
    {
        if (string.IsNullOrEmpty(_password)) Skip.Test("No Mailtrap Credentials");
        
        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body);

        email.Sender = Sender;
        var response = email.Send();

        (response.Successful).Should().BeTrue();
    }


    [Test]
    public async Task CanSendEmailAsync()
    {
        if (string.IsNullOrEmpty(_password)) Skip.Test("No Mailtrap Credentials");
        
        var email = Email
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body);

        email.Sender = Sender;
        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
    }

    [Test]
    public async Task CanSendEmailWithAttachments()
    {
        if (string.IsNullOrEmpty(_password)) Skip.Test("No Mailtrap Credentials");
        
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
            .From(_fromEmail)
            .To(_toEmail)
            .Subject(Subject)
            .Body(Body)
            .Attach(attachment);

        email.Sender = Sender;
        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
    }

    [Test]
    public async Task CanSendEmailWithInlineImages()
    {
        if (string.IsNullOrEmpty(_password)) Skip.Test("No Mailtrap Credentials");
        
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
    }

    [Test]
    public async Task CanSendEmailWithTemplate()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No Mailtrap Credentials");
        
        var email = Email.From(_fromEmail).To(_toEmail);
        email.Sender = new MailtrapSender(_username, _apiKey, _host, 587, _apiHost);

        var response = await email.SendWithTemplateAsync(_templateid, new { var1 = "Test", var2 = "VVVVVVVVVVVVV" });
        
        (response.Successful).Should().BeTrue();
    }
}