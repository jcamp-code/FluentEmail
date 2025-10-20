using FluentEmail.Graph;

namespace FluentEmail.Core.Tests.ThirdParty;

public class GraphSenderTests
{
    private readonly string _appId = Credentials.Graph.AppId;
    private readonly string _tenantId = Credentials.Graph.TenantId;
    private readonly string _graphSecret = Credentials.Graph.ClientSecret;
    private readonly string _senderEmail = Credentials.Graph.FromEmail ?? Credentials.FromEmail;
    private readonly string _toEmail = Credentials.Graph.ToEmail ?? Credentials.ToEmail;
    private const bool SaveSent = false;

    private ISender Sender { get; }

    public GraphSenderTests()
    {
        if (string.IsNullOrWhiteSpace(_appId)) return;
        if (string.IsNullOrWhiteSpace(_tenantId)) return;
        if (string.IsNullOrWhiteSpace(_graphSecret)) return;
        if (string.IsNullOrWhiteSpace(_senderEmail)) return;

        Sender = new GraphSender(_appId, _tenantId, _graphSecret, SaveSent);
    }

    [Fact]
    public void CanSendEmail()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_appId), "No Graph/AD Credentials");
        
        var email = Email
            .From(_senderEmail)
            .To(_toEmail)
            .Subject("Test Email")
            .Body("Test email from Graph sender unit test");

        email.Sender = Sender;
        var response = email.Send();
        (response.Successful).Should().BeTrue();
    }

    [Fact]
    public async Task CanSendEmailAsync()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_appId), "No Graph/AD Credentials");
        
        var email = Email
            .From(_senderEmail)
            .To(_toEmail)
            .Subject("Test Async Email")
            .Body("Test email from Graph sender unit test");

        email.Sender = Sender;
        var response = await email.SendAsync();
        (response.Successful).Should().BeTrue();
    }

    [Fact]
    public async Task CanSendEmailWithAttachments()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_appId), "No Graph/AD Credentials");
        
        var stream = new MemoryStream();
        var sw = new StreamWriter(stream);
        await sw.WriteLineAsync("Hey this is some text in an attachment");
        await sw.FlushAsync(TestContext.Current.CancellationToken);
        stream.Seek(0, SeekOrigin.Begin);

        var attachment = new Attachment
        {
            ContentType = "text/plain",
            Filename = "graphtest.txt",
            Data = stream
        };

        var email = Email
            .From(_senderEmail)
            .To(_toEmail)
            .Subject("Test Email with Attachments")
            .Body("Test email from Graph sender unit test")
            .Attach(attachment);

        email.Sender = Sender;
        var response = await email.SendAsync();
        (response.Successful).Should().BeTrue();
    }

    [Fact]
    public async Task CanSendHighPriorityEmail()
    {
        Assert.SkipWhen(string.IsNullOrEmpty(_appId), "No Graph/AD Credentials");
        
        var email = Email
            .From(_senderEmail)
            .To(_toEmail)
            .Subject("Test High Priority Email")
            .Body("Test email from Graph sender unit test")
            .HighPriority();

        email.Sender = Sender;
        var response = await email.SendAsync();
        (response.Successful).Should().BeTrue();
    }
}