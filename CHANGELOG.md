# Changelog

## [4.0.0](https://github.com/jcamp-code/FluentEmail/compare/v3.8.0...v4.0.0) (2025-10-26)


### ⚠ BREAKING CHANGES

* changed to .net 8.0, 9.0, 10.0

### ✨ Features

* Added .net9.0 compatibility ([dbb57b4](https://github.com/jcamp-code/FluentEmail/commit/dbb57b4d90365b1f79cb8b1489ab40b7f6240426))
* Adds CheckCertificateRevocation to mailkit smtp options ([fed435e](https://github.com/jcamp-code/FluentEmail/commit/fed435e0abbafcea6f0597b4a17ef834b20e3645)), closes [#27](https://github.com/jcamp-code/FluentEmail/issues/27)
* Changed to .net 8.0, 9.0, 10.0 ([e1034c4](https://github.com/jcamp-code/FluentEmail/commit/e1034c46cf215e15fa9fcc51948055da04e67e01))


### 🏡 Miscellaneous Chores

* Initial release-please setup ([#34](https://github.com/jcamp-code/FluentEmail/issues/34)) ([3657e64](https://github.com/jcamp-code/FluentEmail/commit/3657e6418637a1fc5988560d9dac2df03d8fc353))
* Update Bootstrap tests ([4a352fe](https://github.com/jcamp-code/FluentEmail/commit/4a352fea3fb36ce6e51c65d97b23f7aabb41ca1b))
* Update icons ([3292af1](https://github.com/jcamp-code/FluentEmail/commit/3292af18edd3e0ee93d6cd54c862b72efe05e2b3))
* Update RazorLight ([985766c](https://github.com/jcamp-code/FluentEmail/commit/985766cd491868e58f22b84be570a3c22c40f54e)), closes [#24](https://github.com/jcamp-code/FluentEmail/issues/24)
* Update unit tests to xunit and Awesome Assertions ([#32](https://github.com/jcamp-code/FluentEmail/issues/32)) ([fe6f017](https://github.com/jcamp-code/FluentEmail/commit/fe6f017a82aa6bbd2911b61fa26e8dc527057466))

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
