using FluentEmail.MailKitSmtp;

namespace FluentEmail.Core.Tests;

public class MailKitSmtpSenderTests
{
    private const string ToEmail = "bob@test.com";
    private const string FromEmail = "johno@test.com";
    private const string Subject = "sup dawg";
    private const string Body = "what be the hipitity hap?";

    private ISender GetSender(out string tempDirectory)
    {
        var path = Path.Combine(Path.GetTempPath(), Random.Shared.NextInt64().ToString(), "EmailTest");

        var sender = new MailKitSender(new SmtpClientOptions
        {
            Server = "localhost",
            Port = 25,
            UseSsl = false,
            RequiresAuthentication = false,
            UsePickupDirectory = true,
            MailPickupDirectory = path
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
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
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
            Filename = "MailKitAttachment.txt"
        };

        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .Body(Body)
            .Attach(attachment);

        email.Sender = GetSender(out var s);

        var response = await email.SendAsync();

        var files = Directory.EnumerateFiles(s, "*.eml");
        (response.Successful).Should().BeTrue();
        (files).Should().NotBeEmpty();
        
        DeleteTemp(s);
    }

    [Theory]
    [InlineData("logotest.png")]
    public async Task CanSendEmailWithInlineImages(string contentId = null)
    {
        await using var stream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "logotest.png")}");
        var attachment = new Attachment
        {
            IsInline = true,
            Data = stream,
            ContentType = "image/png",
            Filename = "logotest.png",
            ContentId = contentId
        };

        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .Body("<html>Inline image here: <img src=\"cid:logotest.png\">" +
                  "<p>You should see an image without an attachment, or without a download prompt, depending on the email client.</p></html>", true)
            .Attach(attachment);

        email.Sender = GetSender(out var s);
        var response = await email.SendAsync();

        var files = Directory.EnumerateFiles(s, "*.eml");
        (response.Successful).Should().BeTrue();
        (files).Should().NotBeEmpty();
        DeleteTemp(s);
    }

    [Fact]
    public async Task CanSendEmailWithInlineImagesAndAttachmentTogether()
    {
        var attachmentStream = new MemoryStream();
        var sw = new StreamWriter(attachmentStream);
        await sw.WriteLineAsync("Hey this is some text in an attachment");
        await sw.FlushAsync(TestContext.Current.CancellationToken);
        attachmentStream.Seek(0, SeekOrigin.Begin);

        var attachment = new Attachment
        {
            Data = attachmentStream,
            ContentType = "text/plain",
            Filename = "MailKitAttachment.txt",
        };

        await using var inlineStream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "logotest.png")}");

        var attachmentInline = new Attachment
        {
            IsInline = true,
            Data = inlineStream,
            ContentType = "image/png",
            Filename = "logotest.png",
        };

        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .Body("<html>Inline image here: <img src=\"cid:logotest.png\">" +
                  "<p>You should see an image inline without a picture attachment.</p>" +
                  "<p>A single .txt file should also be attached.</p></html>", true)
            .Attach(attachment)
            .Attach(attachmentInline);

        email.Sender = GetSender(out var s);
        
        var response = await email.SendAsync();

        var files = Directory.EnumerateFiles(s, "*.eml");
        (response.Successful).Should().BeTrue();
        (files).Should().NotBeEmpty();
        
        DeleteTemp(s);
    }

    [Fact]
    public async Task CanSendAsyncHtmlAndPlaintextTogether()
    {
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
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
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Body("<h2>Test</h2><p>some body text</p>", true)
            .PlaintextAlternativeBody("Test - Some body text");

        email.Sender = GetSender(out var s);
        var response = email.Send();
        DeleteTemp(s);

        (response.Successful).Should().BeTrue();
    }
}