
namespace FluentEmail.Core.Tests;

public class AttachmentTests
{
    private const string ToEmail = "bob@test.com";
    private const string FromEmail = "johno@test.com";
    private const string Subject = "sup dawg";

    [Fact]
    public void Attachment_from_stream_Is_set()
    {
        using var stream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}");
        var attachment = new Attachment
        {
            Data = stream,
            Filename = "Test.txt",
            ContentType = "text/plain"
        };

        var email = Email.From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .Attach(attachment);

        email.Data.Attachments.First().Data.Length.Should().Be(20);
    }

    [Fact]
    public void Attachment_from_filename_Is_set()
    {
        var email = Email.From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .AttachFromFilename($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", "text/plain");

        email.Data.Attachments.First().Data.Length.Should().Be(20);
    }

    [Fact]
    public void Attachment_from_filename_AttachmentName_Is_set()
    {
        var attachmentName = "attachment.txt";
        var email = Email.From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .AttachFromFilename($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", "text/plain", attachmentName);

        email.Data.Attachments.First().Data.Length.Should().Be(20);
        email.Data.Attachments.First().Filename.Should().Be(attachmentName);
    }
}