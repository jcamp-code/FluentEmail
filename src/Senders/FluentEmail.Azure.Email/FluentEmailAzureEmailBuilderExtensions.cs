using System;
using Azure;
using Azure.Communication.Email;
using Azure.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Azure.Email;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailAzureEmailBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddAzureEmailSender(
            this FluentEmailServicesBuilder builder,
            string connectionString)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton((IServiceProvider x) => (ISender)(object)new AzureEmailSender(connectionString)));
            return builder;
        }
        
        public static FluentEmailServicesBuilder AddAzureEmailSender(
            this FluentEmailServicesBuilder builder,
            string connectionString,
            EmailClientOptions options)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton((IServiceProvider x) => (ISender)(object)new AzureEmailSender(connectionString, options)));
            return builder;
        }
        
        public static FluentEmailServicesBuilder AddAzureEmailSender(
            this FluentEmailServicesBuilder builder,
            Uri endpoint,
            AzureKeyCredential keyCredential,
            EmailClientOptions options = default)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton((IServiceProvider x) => (ISender)(object)new AzureEmailSender(endpoint, keyCredential, options)));
            return builder;
        }
        
        public static FluentEmailServicesBuilder AddAzureEmailSender(
            this FluentEmailServicesBuilder builder,
            Uri endpoint,
            TokenCredential tokenCredential,
            EmailClientOptions options = default)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton((IServiceProvider x) => (ISender)(object)new AzureEmailSender(endpoint, tokenCredential, options)));
            return builder;
        }
    }
}
