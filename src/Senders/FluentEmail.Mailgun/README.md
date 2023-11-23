![fluent email logo](https://raw.githubusercontent.com/lukencode/FluentEmail/master/assets/fluentemail_logo_64x64.png "FluentEmail")

# FluentEmail - All in one email sender for .NET and .NET Core

## Mailgun Email Sender for [FluentEmail](https://github.com/jcamp-code/FluentEmail)

Send email via the MailGun REST API.

## Usage

Create a new instance of your sender and add it as the default Fluent Email sender.

    var sender = new MailgunSender(
		"sandboxcf5f41bbf2f84f15a386c60e253b5fe9.mailgun.org", // Mailgun Domain
		"key-8d32c046d7f14ada8d5ba8253e3e30de" // Mailgun API Key
	);
    Email.DefaultSender = sender;

	/*
		You can optionally set the sender per instance like so:
		
		email.Sender = new MailgunSender(...);
	*/

Send the email in the usual Fluent Email way.

    var email = Email
        .From(fromEmail)
        .To(toEmail)
        .Subject(subject)
        .Body(body);

    var response = await email.SendAsync();


## Further Information

Don't forget you can use Razor templating using the [FluentEmail.Razor](../../Renderers/FluentEmail.Razor) package as well.
