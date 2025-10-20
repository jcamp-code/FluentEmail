using System.Net.Mail;
using System.Threading;
using FluentEmail.Smtp;

namespace FluentEmail.Core.Tests;

public class SmtpSenderTests
{
    private const string ToEmail = "bob@test.com";
    private const string FromEmail = "johno@test.com";
    private const string Subject = "sup dawg";
    private const string Body = "what be the hipitity hap?";

    private static IFluentEmail TestEmail => Email
        .From(FromEmail)
        .To(ToEmail)
        .Subject(Subject)
        .Body(Body);

    private ISender GetSender(out string tempDirectory)
    {
        var path = Path.Combine(Path.GetTempPath(), Random.Shared.NextInt64().ToString(), "EmailTest");

        var sender = new SmtpSender(() => new SmtpClient("localhost")
        {
            EnableSsl = false,
            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
            PickupDirectoryLocation = path
        });

        Directory.CreateDirectory(path);
        tempDirectory = path;
        return sender;
    }

    private void DeleteTemp(string tempDirectory)
    {
        try
        {
            Directory.Delete(tempDirectory, true);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
        }
    }


    [Fact]
    public void CanSendEmail()
    {
        var email = TestEmail
            .Body("<h2>Test</h2>", true);

        email.Sender = GetSender(out var s);

        var response = email.Send();

        var files = Directory.EnumerateFiles(s, "*.eml");
        (response.Successful).Should().BeTrue();
        (files).Should().NotBeEmpty();
        DeleteTemp(s);

    }

    [Fact]
    public async Task CanSendEmailWithAttachments()
    {
        var stream = new MemoryStream();
        var sw = new StreamWriter(stream);
        await sw.WriteLineAsync("Hey this is some text in an attachment");
        await sw.FlushAsync(TestContext.Current.CancellationToken);
        stream.Seek(0, SeekOrigin.Begin);

        var attachment = new Attachment
        {
            Data = stream,
            ContentType = "text/plain",
            Filename = "mailgunTest.txt"
        };

        var email = TestEmail
            .Attach(attachment);

        email.Sender = GetSender(out var s);

        var response = await email.SendAsync();

        (response.Successful).Should().BeTrue();
        var files = Directory.EnumerateFiles(s, "*.eml");
        (files).Should().NotBeEmpty();

        DeleteTemp(s);

    }

    [Fact]
    public async Task CanSendAsyncHtmlAndPlaintextTogether()
    {
        var email = TestEmail
            .Body("<h2>Test</h2><p>some body text</p>", true)
            .PlaintextAlternativeBody("Test - Some body text");

        email.Sender = GetSender(out var s);

        var response = await email.SendAsync();

        DeleteTemp(s);

        (response.Successful).Should().BeTrue();
    }

    [Fact]
    public void CanSendHtmlAndPlaintextTogether()
    {
        var email = TestEmail
            .Body("<h2>Test</h2><p>some body text</p>", true)
            .PlaintextAlternativeBody("Test - Some body text");

        email.Sender = GetSender(out var s);

        var response = email.Send();

        DeleteTemp(s);

        (response.Successful).Should().BeTrue();
    }

    [Fact]
    public void CancelSendIfCancellationRequested()
    {
        var email = TestEmail;

        var tokenSource = new CancellationTokenSource();
        tokenSource.Cancel();

        email.Sender = GetSender(out var s);

        var response = email.Send(tokenSource.Token);

        DeleteTemp(s);

        (response.Successful).Should().BeFalse();
    }
}
