using FluentEmail.Core.Interfaces;
using FluentEmail.MailKitSmtp;
using MailKit;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailMailKitBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddMailKitSender(this FluentEmailServicesBuilder builder, SmtpClientOptions smtpClientOptions)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailKitSender(smtpClientOptions)));
            return builder;
        }
        public static FluentEmailServicesBuilder AddMailKitSender(this FluentEmailServicesBuilder builder, SmtpClientOptions smtpClientOptions, IProtocolLogger protocolLogger)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailKitSender(smtpClientOptions, protocolLogger)));
            return builder;
        }
        public static FluentEmailServicesBuilder AddMailKitSender(this FluentEmailServicesBuilder builder, string configPath = "SmtpClientOptions")
        {
            builder.Services.AddOptions<SmtpClientOptions>().BindConfiguration(configPath);
            builder.Services.TryAddScoped<SmtpClientOptions>(resolver =>
            {
                var smtpOptions = resolver.GetRequiredService<IOptions<SmtpClientOptions>>();
                if (smtpOptions.Value is null)
                {
                    throw new System.Exception("SmtpClientOptions is null");
                }
                if (smtpOptions.Value.User is not null && smtpOptions.Value.Password is not null)
                {
                    smtpOptions.Value.RequiresAuthentication = true;
                    smtpOptions.Value.UseSsl = true;
                    if (smtpOptions.Value.SocketOptions is null)
                        smtpOptions.Value.SocketOptions = SecureSocketOptions.StartTls;
                }
                return smtpOptions.Value;
            });
            builder.Services.TryAddScoped<ISender, MailKitSender>();
            return builder;
        }

    }
}
