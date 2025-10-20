using AwesomeAssertions;
using FluentEmail.Core.Interfaces;
using FluentEmail.SendGrid;
using System.IO;
using System.Threading.Tasks;
using TUnit.Core;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace FluentEmail.Core.Tests.ThirdParty;

public class SendGridSenderTests
{
    private readonly string _apiKey = Credentials.SendGrid.ApiKey;
    private readonly string _toEmail = Credentials.SendGrid.ToEmail ?? Credentials.ToEmail;
    private readonly string _fromEmail = Credentials.SendGrid.FromEmail ?? Credentials.FromEmail;

    private const string ToName = "FluentEmail Test";
    private const string FromName = "SendGridSender Test";

    private ISender Sender { get; set; }

    public SendGridSenderTests()
    {
        if (!string.IsNullOrEmpty(_apiKey)) Sender = new SendGridSender(_apiKey, true);
    }

    [Test]
    public async Task CanSendEmail()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No SendGrid Credentials");
            
        const string subject = "SendMail Test";
        const string body = "This email is testing send mail functionality of SendGrid Sender.";

        var email = Email
            .From(_fromEmail, FromName)
            .To(_toEmail, ToName)
            .Subject(subject)
            .Body(body);

        email.Sender = Sender;

        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
    }

    [Test]
    public async Task CanSendTemplateEmail()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No SendGrid Credentials");
        
        const string subject = "SendMail Test";
        var templateId = Credentials.SendGrid.Template;
        object templateData = new
        {
            Name = ToName,
            ArbitraryValue = "The quick brown fox jumps over the lazy dog."
        };

        var email = Email
            .From(_fromEmail, FromName)
            .To(_toEmail, ToName)
            .Subject(subject);
        
        email.Sender = Sender;

        var response = await email.SendWithTemplateAsync(templateId, templateData);

        (response.Successful).Should().BeTrue();
    }

    [Test]
    public async Task CanSendEmailWithReplyTo()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No SendGrid Credentials");
        
        const string subject = "SendMail Test";
        const string body = "This email is testing send mail with ReplyTo functionality of SendGrid Sender.";

        var email = Email
            .From(_fromEmail, FromName)
            .To(_toEmail, ToName)
            .ReplyTo(_toEmail, ToName)
            .Subject(subject)
            .Body(body);

        email.Sender = Sender;

        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
    }

    [Test]
    public async Task CanSendEmailWithCategory()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No SendGrid Credentials");
        
        const string subject = "SendMail Test";
        const string body = "This email is testing send mail with Categories functionality of SendGrid Sender.";

        var email = Email
            .From(_fromEmail, FromName)
            .To(_toEmail, ToName)
            .ReplyTo(_toEmail, ToName)
            .Subject(subject)
            .Tag("TestCategory")
            .Body(body);
        
        email.Sender = Sender;
        
        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
    }

    [Test]
    public async Task CanSendEmailWithAttachments()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No SendGrid Credentials");
        
        const string subject = "SendMail With Attachments Test";
        const string body = "This email is testing the attachment functionality of SendGrid Sender.";

        using (var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}/test-binary.xlsx"))
        {
            var attachment = new Attachment
            {
                Data = stream,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Filename = "test-binary.xlsx"
            };

            var email = Email
                .From(_fromEmail, FromName)
                .To(_toEmail, ToName)
                .Subject(subject)
                .Body(body)
                .Attach(attachment);
            
            email.Sender = Sender;

            var response = await email.SendAsync();

            (response.Successful).Should().BeTrue();
        }
    }

    [Test]
    public async Task CanSendHighPriorityEmail()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No SendGrid Credentials");
        
        const string subject = "SendMail Test";
        const string body = "This email is testing send mail functionality of SendGrid Sender.";

        var email = Email
            .From(_fromEmail, FromName)
            .To(_toEmail, ToName)
            .Subject(subject)
            .Body(body)
            .HighPriority();

        email.Sender = Sender;

        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
    }

    [Test]
    public async Task CanSendLowPriorityEmail()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No SendGrid Credentials");
        
        const string subject = "SendMail Test";
        const string body = "This email is testing send mail functionality of SendGrid Sender.";

        var email = Email
            .From(_fromEmail, FromName)
            .To(_toEmail, ToName)
            .Subject(subject)
            .Body(body)
            .LowPriority();
        
        email.Sender = Sender;

        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
    }

    [Test]
    public async Task CanSendEmailWithInlineAttachments()
    {
        if (string.IsNullOrEmpty(_apiKey)) Skip.Test("No SendGrid Credentials");
        
        // Arrange
        const string subject = "SendMail With Inline Attachments Test";
        const string body = "This email is testing the inline attachment functionality of SendGrid Sender.";

        using (var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}/logotest.png"))
        {
            var attachment = new Attachment
            {
                Data = stream,
                ContentType = "image/png",
                Filename = "logotest.png",
                IsInline = true,
                ContentId = "logotest_id"
            };

            var email = Email
                .From(_fromEmail, FromName)
                .To(_toEmail, ToName)
                .Subject(subject)
                .Body(body)
                .Attach(attachment);

            email.Sender = Sender;
            var response = await email.SendAsync();
            
            (response.Successful).Should().BeTrue();
        }
    }
}