using System;
using System.Collections.Generic;
using System.Text;
using FluentEmail.Core.Interfaces;
using FluentEmail.Postmark;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides fluent extension methods for adding Postmark as a sending backend to FluentEmail.
    /// </summary>
    public static class FluentEmailPostmarkBuilderExtensions
    {
        /// <summary>
        /// Adds a PostmarkSender to be used by FluentEmail.
        /// </summary>
        /// <param name="builder">The builder for this FluentEmail service.</param>
        /// <param name="serverToken">The serverToken to use to authenticate at the Postmark API.</param>
        public static FluentEmailServicesBuilder AddPostmarkSender(this FluentEmailServicesBuilder builder, string serverToken)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(x => new PostmarkSender(serverToken)));
            return builder;
        }

        /// <summary>
        /// Adds a PostmarkSender to be used by FluentEmail.
        /// </summary>
        /// <param name="builder">The builder for this FluentEmail service.</param>
        /// <param name="serverToken">The serverToken to use to authenticate at the Postmark API.</param>
        /// <param name="configureOptions">A method that changes the desired options of a PostmarkSenderOptions instance.</param>
        public static FluentEmailServicesBuilder AddPostmarkSender(this FluentEmailServicesBuilder builder, string serverToken, Action<PostmarkSenderOptions> configureOptions)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            _ = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
            var opts = new PostmarkSenderOptions(serverToken);
            configureOptions(opts);
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(x => new PostmarkSender(opts)));
            return builder;
        }

        /// <summary>
        /// Adds a PostmarkSender to be used by FluentEmail.
        /// </summary>
        /// <param name="builder">The builder for this FluentEmail service.</param>
        /// <param name="options">A preconfigured PostmarkSenderOptions instance.</param>
        public static FluentEmailServicesBuilder AddPostmarkSender(this FluentEmailServicesBuilder builder, PostmarkSenderOptions options)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            _ = options ?? throw new ArgumentNullException(nameof(options));
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(x => new PostmarkSender(options)));
            return builder;
        }
    }
}
