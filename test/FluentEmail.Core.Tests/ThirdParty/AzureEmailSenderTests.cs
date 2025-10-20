using FluentEmail.Azure.Email;

namespace FluentEmail.Core.Tests.ThirdParty;

public class AzureEmailSenderTests
{
    private readonly string _toEmail = Credentials.Azure.ToEmail ?? Credentials.ToEmail;
    private readonly string _fromEmail = Credentials.Azure.FromEmail ?? Credentials.FromEmail;
    private readonly string _connectionString = Credentials.Azure.ApiHost;

    private const string ToName = "FluentEmail tester";
    private const string FromName = "AzureEmailSender Test";
    
    private ISender Sender { get; }

    public AzureEmailSenderTests()
    {
        if (!string.IsNullOrEmpty(_connectionString)) Sender = new AzureEmailSender(_connectionString);
    }

    [Fact]
    public async Task CanSendEmail()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_connectionString), "No Azure Credentials");
        
        const string subject = "SendMail Test";
        const string body = "This email is testing send mail functionality of Azure Email Sender.";

        var email = Email
            .From(_fromEmail, FromName)
            .To(_toEmail, ToName)
            .Subject(subject)
            .Body(body);

        email.Sender = Sender;
        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
    }

    [Fact]
    public async Task CanSendEmailWithReplyTo()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_connectionString), "No Azure Credentials");
        
        const string subject = "SendMail Test";
        const string body = "This email is testing send mail with ReplyTo functionality of Azure Email Sender.";

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

    [Fact]
    public async Task CanSendEmailWithAttachments()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_connectionString), "No Azure Credentials");
        
        const string subject = "SendMail With Attachments Test";
        const string body = "This email is testing the attachment functionality of Azure Email Sender.";

        await using var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}/test-binary.xlsx");
        var attachment = new Attachment
        {
            Data = stream,
            ContentType = "xlsx",
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

    [Fact]
    public async Task CanSendHighPriorityEmail()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_connectionString), "No Azure Credentials");
        
        const string subject = "SendMail Test";
        const string body = "This email is testing send mail functionality of Azure Email Sender.";

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

    [Fact]
    public async Task CanSendLowPriorityEmail()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_connectionString), "No Azure Credentials");
        
        const string subject = "SendMail Test";
        const string body = "This email is testing send mail functionality of Azure Email Sender.";

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
}