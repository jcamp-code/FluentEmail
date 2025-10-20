using System.Collections.Generic;

namespace FluentEmail.Core.Tests;

public class FluentEmailTests
{
    private const string ToEmail = "bob@test.com";
    private const string FromEmail = "johno@test.com";
    private const string Subject = "sup dawg";
    private const string Body = "what be the hipitity hap?";

    [Fact]
    public void To_Address_Is_Set()
    {
        var email = Email
            .From(FromEmail)
            .To(ToEmail);

        email.Data.ToAddresses[0].EmailAddress.Should().Be(ToEmail);
    }

    [Fact]
    public void From_Address_Is_Set()
    {
        var email = Email.From(FromEmail);

        email.Data.FromAddress.EmailAddress.Should().Be(FromEmail);
    }

    [Fact]
    public void Subject_Is_Set()
    {
        var email = Email
            .From(FromEmail)
            .Subject(Subject);

        email.Data.Subject.Should().Be(Subject);
    }

    [Fact]
    public void Body_Is_Set()
    {
        var email = Email.From(FromEmail)
            .Body(Body);

        email.Data.Body.Should().Be(Body);
    }

    [Fact]
    public void Can_Add_Multiple_Recipients()
    {
        var toEmail1 = "bob@test.com";
        var toEmail2 = "ratface@test.com";

        var email = Email
            .From(FromEmail)
            .To(toEmail1)
            .To(toEmail2);

        email.Data.ToAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void Can_Add_Multiple_Recipients_From_List()
    {
        var emails = new List<Address>
        {
            new("email1@email.com"),
            new("email2@email.com")
        };

        var email = Email
            .From(FromEmail)
            .To(emails);

        email.Data.ToAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void Can_Add_Multiple_Recipients_From_String_List()
    {
        var emails = new List<string>
        {
            "email1@email.com",
            "email2@email.com"
        };

        var email = Email
            .From(FromEmail)
            .To(emails);

        email.Data.ToAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void Can_Add_Multiple_Recipients_From_String_Array()
    {
        var emails = new[]
        {
            "email1@email.com",
            "email2@email.com"
        };

        var email = Email
            .From(FromEmail)
            .To(emails);

        email.Data.ToAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void Can_Add_Multiple_CCRecipients_From_List()
    {
        var emails = new List<Address>
        {
            new("email1@email.com"),
            new("email2@email.com")
        };

        var email = Email
            .From(FromEmail)
            .CC(emails);

        email.Data.CcAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void Can_Add_Multiple_BCCRecipients_From_List()
    {
        var emails = new List<Address>
        {
            new("email1@email.com"),
            new("email2@email.com")
        };

        var email = Email
            .From(FromEmail)
            .BCC(emails);

        email.Data.BccAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void Is_Valid_With_Properties_Set()
    {
        var email = Email
            .From(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .Body(Body);

        email.Data.Body.Should().Be(Body);
        email.Data.Subject.Should().Be(Subject);
        email.Data.FromAddress.EmailAddress.Should().Be(FromEmail);
        email.Data.ToAddresses[0].EmailAddress.Should().Be(ToEmail);
    }

    [Fact]
    public void ReplyTo_Address_Is_Set()
    {
        var replyEmail = "reply@email.com";

        var email = Email.From(FromEmail)
            .ReplyTo(replyEmail);

        email.Data.ReplyToAddresses.First().EmailAddress.Should().Be(replyEmail);
    }

    [Fact]
    public void Can_Add_Multiple_ReplyTo_From_String_List()
    {
        var emails = new List<string>
        {
            "email1@email.com",
            "email2@email.com"
        };

        var email = Email
            .From(FromEmail)
            .ReplyTo(emails);

        email.Data.ReplyToAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void Can_Add_Multiple_ReplyTo_From_String_Array()
    {
        var emails = new[]
            {
                "email1@email.com",
                "email2@email.com"
            };

        var email = Email
            .From(FromEmail)
            .ReplyTo(emails);

        email.Data.ReplyToAddresses.Count.Should().Be(2);
    }

    #region Refactored tests using setup through constructors.
    [Fact]
    public void New_To_Address_Is_Set()
    {
        var email = new Email(FromEmail)
            .To(ToEmail);

        email.Data.ToAddresses[0].EmailAddress.Should().Be(ToEmail);
    }

    [Fact]
    public void New_From_Address_Is_Set()
    {
        var email = new Email(FromEmail);

        email.Data.FromAddress.EmailAddress.Should().Be(FromEmail);
    }

    [Fact]
    public void New_Subject_Is_Set()
    {
        var email = new Email(FromEmail)
            .Subject(Subject);

        email.Data.Subject.Should().Be(Subject);
    }

    [Fact]
    public void New_Body_Is_Set()
    {
        var email = new Email(FromEmail)
            .Body(Body);

        email.Data.Body.Should().Be(Body);
    }

    [Fact]
    public void New_Can_Add_Multiple_Recipients()
    {
        var toEmail1 = "bob@test.com";
        var toEmail2 = "ratface@test.com";

        var email = new Email(FromEmail)
            .To(toEmail1)
            .To(toEmail2);

        email.Data.ToAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void New_Can_Add_Multiple_Recipients_From_List()
    {
        var emails = new List<Address>
        {
            new("email1@email.com"),
            new("email2@email.com")
        };

        var email = new Email(FromEmail)
            .To(emails);

        email.Data.ToAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void New_Can_Add_Multiple_CCRecipients_From_List()
    {
        var emails = new List<Address>
        {
            new("email1@email.com"),
            new("email2@email.com")
        };

        var email = new Email(FromEmail)
            .CC(emails);

        email.Data.CcAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void New_Can_Add_Multiple_BCCRecipients_From_List()
    {
        var emails = new List<Address>
        {
            new("email1@email.com"),
            new("email2@email.com")
        };

        var email = new Email(FromEmail)
            .BCC(emails);

        email.Data.BccAddresses.Count.Should().Be(2);
    }

    [Fact]
    public void New_Is_Valid_With_Properties_Set()
    {
        var email = new Email(FromEmail)
            .To(ToEmail)
            .Subject(Subject)
            .Body(Body);

        email.Data.Body.Should().Be(Body);
        email.Data.Subject.Should().Be(Subject);
        email.Data.FromAddress.EmailAddress.Should().Be(FromEmail);
        email.Data.ToAddresses[0].EmailAddress.Should().Be(ToEmail);
    }

    [Fact]
    public void New_ReplyTo_Address_Is_Set()
    {
        var replyEmail = "reply@email.com";

        var email = new Email(FromEmail)
            .ReplyTo(replyEmail);

        email.Data.ReplyToAddresses.First().EmailAddress.Should().Be(replyEmail);
    }
    #endregion
}