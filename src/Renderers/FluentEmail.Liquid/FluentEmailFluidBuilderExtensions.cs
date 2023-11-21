using System;
using System.Reflection;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Liquid;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailFluidBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddLiquidRenderer(
            this FluentEmailServicesBuilder builder,
            Action<LiquidRendererOptions>? configure = null)
        {
            builder.Services.AddOptions<LiquidRendererOptions>();
            if (configure != null)
            {
                builder.Services.Configure(configure);
            }

            builder.Services.TryAddSingleton<ITemplateRenderer, LiquidRenderer>();
            return builder;
        }
        
        public static FluentEmailServicesBuilder AddLiquidRendererWithEmbedded(
            this FluentEmailServicesBuilder builder,
            Action<LiquidRendererOptions>? configure = null)
        {
            var assembly = Assembly.GetCallingAssembly();
            var name = assembly.GetName().Name;
            return AddLiquidRendererWithEmbedded(builder, assembly, $"{name}.EmailTemplates", configure);
        }

        public static FluentEmailServicesBuilder AddLiquidRendererWithEmbedded(
            this FluentEmailServicesBuilder builder,
            Assembly assembly,
            Action<LiquidRendererOptions>? configure = null)
        {
            var name = assembly.GetName().Name;
            return AddLiquidRendererWithEmbedded(builder, assembly, $"{name}.EmailTemplates", configure);
        }

        public static FluentEmailServicesBuilder AddLiquidRendererWithEmbedded(
            this FluentEmailServicesBuilder builder,
            string rootPath,
            Action<LiquidRendererOptions>? configure = null)
        {
            var assembly = Assembly.GetCallingAssembly();
            var name = assembly.GetName().Name;
            if (!string.IsNullOrEmpty(rootPath)) name += "."; 
            return AddLiquidRendererWithEmbedded(builder, assembly, $"{name}{rootPath}", configure);
        }
        
        public static FluentEmailServicesBuilder AddLiquidRendererWithEmbedded(
            this FluentEmailServicesBuilder builder,
            Assembly assembly,
            string rootNamespace,
            Action<LiquidRendererOptions>? configure = null)
        {
            builder.AddLiquidRenderer(options =>
            {
                options.FileProvider = new EmbeddedFileProvider(assembly, rootNamespace);
                configure?.Invoke(options);
            });
            EmbeddedTemplates.Configure(assembly, rootNamespace);
            return builder;
        }

    }
}