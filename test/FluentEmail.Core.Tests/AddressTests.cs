using Xunit;
using AwesomeAssertions;

namespace FluentEmail.Core.Tests
{
	public class AddressTests
	{
		[Fact]
		public void SplitAddress_Test()
		{
			var email = Email
				.From("test@test.com")
				.To("james@test.com;john@test.com", "James 1;John 2");

			email.Data.ToAddresses.Count.Should().Be(2);
			email.Data.ToAddresses[0].EmailAddress.Should().Be("james@test.com");
			email.Data.ToAddresses[1].EmailAddress.Should().Be("john@test.com");
			email.Data.ToAddresses[0].Name.Should().Be("James 1");
			email.Data.ToAddresses[1].Name.Should().Be("John 2");
		}

		[Fact]
		public void SplitAddress_Test2()
		{
			var email = Email
                .From("test@test.com")
                .To("james@test.com; john@test.com", "James 1");

			email.Data.ToAddresses.Count.Should().Be(2);
			email.Data.ToAddresses[0].EmailAddress.Should().Be("james@test.com");
			email.Data.ToAddresses[1].EmailAddress.Should().Be("john@test.com");
			email.Data.ToAddresses[0].Name.Should().Be("James 1");
			email.Data.ToAddresses[1].Name.Should().Be(string.Empty);
		}

		[Fact]
		public void SplitAddress_Test3()
		{
			var email = Email
                .From("test@test.com")
                .To("james@test.com; john@test.com;   Fred@test.com", "James 1;;Fred");

			email.Data.ToAddresses.Count.Should().Be(3);
			email.Data.ToAddresses[0].EmailAddress.Should().Be("james@test.com");
			email.Data.ToAddresses[1].EmailAddress.Should().Be("john@test.com");
			email.Data.ToAddresses[2].EmailAddress.Should().Be("Fred@test.com");
			email.Data.ToAddresses[0].Name.Should().Be("James 1");
			email.Data.ToAddresses[1].Name.Should().Be(string.Empty);
			email.Data.ToAddresses[2].Name.Should().Be("Fred");
		}

        [Fact]
        public void SetFromAddress()
        {
            var email = new Email();
            email.SetFrom("test@test.test", "test");

            email.Data.FromAddress.EmailAddress.Should().Be("test@test.test");
            email.Data.FromAddress.Name.Should().Be("test");
        }

        #region Refactored tests using setup through constructor.
        [Fact]
		public void New_SplitAddress_Test()
		{
			var email = new Email()
				.To("james@test.com;john@test.com", "James 1;John 2");

			email.Data.ToAddresses.Count.Should().Be(2);
			email.Data.ToAddresses[0].EmailAddress.Should().Be("james@test.com");
			email.Data.ToAddresses[1].EmailAddress.Should().Be("john@test.com");
			email.Data.ToAddresses[0].Name.Should().Be("James 1");
			email.Data.ToAddresses[1].Name.Should().Be("John 2");
		}


		[Fact]
		public void New_SplitAddress_Test2()
		{
			var email = new Email()
				.To("james@test.com; john@test.com", "James 1");

			email.Data.ToAddresses.Count.Should().Be(2);
			email.Data.ToAddresses[0].EmailAddress.Should().Be("james@test.com");
			email.Data.ToAddresses[1].EmailAddress.Should().Be("john@test.com");
			email.Data.ToAddresses[0].Name.Should().Be("James 1");
			email.Data.ToAddresses[1].Name.Should().Be(string.Empty);
		}


		[Fact]
		public void New_SplitAddress_Test3()
		{
			var email = new Email()
				.To("james@test.com; john@test.com;   Fred@test.com", "James 1;;Fred");

			email.Data.ToAddresses.Count.Should().Be(3);
			email.Data.ToAddresses[0].EmailAddress.Should().Be("james@test.com");
			email.Data.ToAddresses[1].EmailAddress.Should().Be("john@test.com");
			email.Data.ToAddresses[2].EmailAddress.Should().Be("Fred@test.com");
			email.Data.ToAddresses[0].Name.Should().Be("James 1");
			email.Data.ToAddresses[1].Name.Should().Be(string.Empty);
			email.Data.ToAddresses[2].Name.Should().Be("Fred");
		}
		#endregion
	}
}
