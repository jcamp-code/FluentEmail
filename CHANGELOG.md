# Changelog

## v3.8.0

[compare changes](https://github.com/jcamp-code/FluentEmail/compare/v3.7.0...v3.8.0)

### 🚀 Enhancements

- Update Mailkit to 4.7 and upgrade vulnerable components
- Mailtrap support send with template method

### 🩹 Fixes
- Update Mailkit to 4.7 and upgrade vulnerable components
- Bind MailgunSender to ISender in singleton scope
- Plaintext parameter to always include plaintext ([4a38382](https://github.com/jcamp-code/FluentEmail/commit/4a38382))
- Remove prerelease from azure sender ([90cac43](https://github.com/jcamp-code/FluentEmail/commit/90cac43))
- Email.AttachFromFilename does not dispose stream ([87441ae](https://github.com/jcamp-code/FluentEmail/commit/87441ae))

### 🏡 Chore
- Updated FluentEmail.MailerSend package reference in Readme
- Tidy code ([8a24d6d](https://github.com/jcamp-code/FluentEmail/commit/8a24d6d))

### ❤️ Contributors

- [neo.zhu](https://github.com/neozhu) 
- [Aaron Sherber](https://github.com/asherber)
- [Mark Menchavez](https://github.com/MarkMenchavez)
- [marcoatribeiro](https://github.com/marcoatribeiro)
- [brnn8r](https://github.com/brnn8r)
- 

## v3.7.0

### 🚀 Enhancements

  - Allow configuring Liquid parser ([#18](https://github.com/jcamp-code/FluentEmail/pull/18))

### ❤️  Contributors

- [Ville Häkli](https://github.com/VilleHakli) 

## v3.6.1

### 🩹 Fixes

  - Use latest UnDotNet.BootstrapEmail ([f0fd690](https://github.com/jcamp-code/FluentEmail/commit/f0fd690))

## v3.6.0

### 🚀 Enhancements

  - Update to latest Azure Email Client ([aa3a419](https://github.com/jcamp-code/FluentEmail/commit/aa3a419)) - thanks [@TheObliterator](https://github.com/TheObliterator)
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
