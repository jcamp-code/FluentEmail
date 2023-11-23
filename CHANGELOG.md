# Changelog

## v3.6.0

### 🚀 Enhancements

  - Update to latest Azure Email Client ([aa3a419](https://github.com/jcamp-code/FluentEmail/commit/aa3a419))
  - Add UnDotNet.BootstrapEmail processing ([05cfca2](https://github.com/jcamp-code/FluentEmail/commit/05cfca2))

### 🏡 Chore

  - Added README to all packages ([8801ddd](https://github.com/jcamp-code/FluentEmail/commit/8801ddd))

## v3.5.1

- Use GetCallingAssembly() rather than GetExecutingAssemby() in LiquidRenderer builder extensions

## v3.5.0

- Added simplified configuration to setup and use embedded templates with and without the LiquidRenderer.

## v3.4.0

- Added MailPace sender - thanks [@maartenba](https://github.com/maartenba)

## v3.3.1

- Added MailKit builder to use injected config to allow it to come from .NET config system
- Updated to MailKit 4.3.0

## v3.3

- Added support for mailgun templates - [Original Source/Credit](https://github.com/gps-lasrol/FluentEmail/tree/support-mailgun-templates)
- Fix Azure Email CC and BCC sending to the wrong email addresses - thanks [@megasware128](https://github.com/Megasware128)

## v3.2

- Added FluentEmail.Postmark - [Original Source/Credit](https://github.com/georg-jung/FluentEmail.Postmark)

## v3.1

- Initial release of jcamp.\* variants of FluentEmail
