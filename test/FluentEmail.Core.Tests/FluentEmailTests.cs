using System.Collections.Generic;
using FluentEmail.Core.Models;
using Xunit;
using AwesomeAssertions;
using System.Linq;

namespace FluentEmail.Core.Tests
{
    public class FluentEmailTests
	{
		const string toEmail = "bob@test.com";
		const string fromEmail = "johno@test.com";
		const string subject = "sup dawg";
		const string body = "what be the hipitity hap?";

		[Fact]
		public void To_Address_Is_Set()
		{
			var email = Email
				.From(fromEmail)
				.To(toEmail);

			email.Data.ToAddresses[0].EmailAddress.Should().Be(toEmail);
		}

		[Fact]
		public void From_Address_Is_Set()
		{
			var email = Email.From(fromEmail);

			email.Data.FromAddress.EmailAddress.Should().Be(fromEmail);
		}

		[Fact]
		public void Subject_Is_Set()
		{
			var email = Email
				.From(fromEmail)
				.Subject(subject);

			email.Data.Subject.Should().Be(subject);
		}

		[Fact]
		public void Body_Is_Set()
		{
			var email = Email.From(fromEmail)
				.Body(body);

			email.Data.Body.Should().Be(body);
		}

		[Fact]
		public void Can_Add_Multiple_Recipients()
		{
			string toEmail1 = "bob@test.com";
			string toEmail2 = "ratface@test.com";

			var email = Email
				.From(fromEmail)
				.To(toEmail1)
				.To(toEmail2);

			email.Data.ToAddresses.Count.Should().Be(2);
		}

		[Fact]
		public void Can_Add_Multiple_Recipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = Email
				.From(fromEmail)
				.To(emails);

			email.Data.ToAddresses.Count.Should().Be(2);
		}

		[Fact]
		public void Can_Add_Mutlitple_Recipients_From_String_List()
        {
			var emails = new List<string>();
			emails.Add("email1@email.com");
			emails.Add("email2@email.com");

			var email = Email
				.From(fromEmail)
				.To(emails);

			email.Data.ToAddresses.Count.Should().Be(2);
        }

		[Fact]
		public void Can_Add_Mutlitple_Recipients_From_String_Array()
		{
			var emails = new string[]
			{
				"email1@email.com",
				"email2@email.com"
			};

			var email = Email
				.From(fromEmail)
				.To(emails);

			email.Data.ToAddresses.Count.Should().Be(2);
		}

		[Fact]
		public void Can_Add_Multiple_CCRecipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = Email
				.From(fromEmail)
				.CC(emails);

			email.Data.CcAddresses.Count.Should().Be(2);
		}

		[Fact]
		public void Can_Add_Multiple_BCCRecipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = Email
				.From(fromEmail)
				.BCC(emails);

			email.Data.BccAddresses.Count.Should().Be(2);
		}

		[Fact]
		public void Is_Valid_With_Properties_Set()
		{
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.Body(body);

			email.Data.Body.Should().Be(body);
			email.Data.Subject.Should().Be(subject);
			email.Data.FromAddress.EmailAddress.Should().Be(fromEmail);
			email.Data.ToAddresses[0].EmailAddress.Should().Be(toEmail);
		}

		[Fact]
		public void ReplyTo_Address_Is_Set()
		{
			var replyEmail = "reply@email.com";

			var email = Email.From(fromEmail)
				.ReplyTo(replyEmail);

			email.Data.ReplyToAddresses.First().EmailAddress.Should().Be(replyEmail);
		}

        [Fact]
        public void Can_Add_Mutlitple_ReplyTo_From_String_List()
        {
            var emails = new List<string>();
            emails.Add("email1@email.com");
            emails.Add("email2@email.com");

            var email = Email
                .From(fromEmail)
                .ReplyTo(emails);

            email.Data.ReplyToAddresses.Count.Should().Be(2);
        }

        [Fact]
        public void Can_Add_Mutlitple_ReplyTo_From_String_Array()
        {
            var emails = new string[]
            {
                "email1@email.com",
                "email2@email.com"
            };

            var email = Email
                .From(fromEmail)
                .ReplyTo(emails);

            email.Data.ReplyToAddresses.Count.Should().Be(2);
        }

#region Refactored tests using setup through constructors.
        [Fact]
		public void New_To_Address_Is_Set()
		{
			var email = new Email(fromEmail)
				.To(toEmail);

			email.Data.ToAddresses[0].EmailAddress.Should().Be(toEmail);
		}

		[Fact]
		public void New_From_Address_Is_Set()
		{
			var email = new Email(fromEmail);

			email.Data.FromAddress.EmailAddress.Should().Be(fromEmail);
		}

		[Fact]
		public void New_Subject_Is_Set()
		{
			var email = new Email(fromEmail)
				.Subject(subject);

			email.Data.Subject.Should().Be(subject);
		}

		[Fact]
		public void New_Body_Is_Set()
		{
			var email = new Email(fromEmail)
				.Body(body);

			email.Data.Body.Should().Be(body);
		}

		[Fact]
		public void New_Can_Add_Multiple_Recipients()
		{
			string toEmail1 = "bob@test.com";
			string toEmail2 = "ratface@test.com";

			var email = new Email(fromEmail)
				.To(toEmail1)
				.To(toEmail2);

			email.Data.ToAddresses.Count.Should().Be(2);
		}

		[Fact]
		public void New_Can_Add_Multiple_Recipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = new Email(fromEmail)
				.To(emails);

			email.Data.ToAddresses.Count.Should().Be(2);
		}

		[Fact]
		public void New_Can_Add_Multiple_CCRecipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = new Email(fromEmail)
				.CC(emails);

			email.Data.CcAddresses.Count.Should().Be(2);
		}

		[Fact]
		public void New_Can_Add_Multiple_BCCRecipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = new Email(fromEmail)
				.BCC(emails);

			email.Data.BccAddresses.Count.Should().Be(2);
		}

		[Fact]
		public void New_Is_Valid_With_Properties_Set()
		{
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.Body(body);

			email.Data.Body.Should().Be(body);
			email.Data.Subject.Should().Be(subject);
			email.Data.FromAddress.EmailAddress.Should().Be(fromEmail);
			email.Data.ToAddresses[0].EmailAddress.Should().Be(toEmail);
		}

		[Fact]
		public void New_ReplyTo_Address_Is_Set()
		{
			var replyEmail = "reply@email.com";

			var email = new Email(fromEmail)
				.ReplyTo(replyEmail);

			email.Data.ReplyToAddresses.First().EmailAddress.Should().Be(replyEmail);
		}
		#endregion
	}
}
