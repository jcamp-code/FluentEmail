using System.Globalization;
using System.Reflection;
using UnDotNet.BootstrapEmail;

// ReSharper disable once CheckNamespace
namespace FluentEmail.Core;

public static class FluentEmailBootstrap
{
    /// <summary>
    /// Compiles the template from the renderer through UnDotNet.BootstrapEmail
    /// </summary>
    /// <param name="email">The IFluentEmail object</param>
    /// <param name="plainText">True if you want to set the plain text alternative automatically using UnDotNet.HtmlToText (Optional)</param>
    /// <returns>Instance of the Email class</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static IFluentEmail CompileBootstrap(this IFluentEmail email, bool plainText = true)
    {
        var compiler = new BootstrapCompiler(email.Data.Body);
        var result = compiler.Multipart();
        email.Data.Body = result.Html;
        email.Data.IsHtml = true;
        if (plainText)
        {
            email.PlaintextAlternativeBody(result.Text);
        }

        return email;
    }

    /// <summary>
    /// Adds Bootstrap body to the email
    /// </summary>
    /// <param name="email">The IFluentEmail object</param>
    /// <param name="body">The body</param>
    /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
    /// <returns>Instance of the Email class</returns>
    public static IFluentEmail UsingBootstrapBody(this IFluentEmail email, string body, bool isHtml = true)
    {
        email.Body(body, isHtml);
        email.CompileBootstrap(isHtml);
        return email;
    }

    /// <summary>
    /// Adds Bootstrap template to the email
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="email">The IFluentEmail object</param> 
    /// <param name="template">The template</param>
    /// <param name="model">Model for the template</param>
    /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
    /// <returns>Instance of the Email class</returns>
    public static IFluentEmail UsingBootstrapTemplate<T>(this IFluentEmail email, string template, T model,
        bool isHtml = true)
    {
        email.UsingTemplate(template, model, isHtml);
        email.CompileBootstrap(isHtml);
        return email;
    }

    /// <summary>
    /// Adds Bootstrap template to email from embedded resource
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="email">The IFluentEmail object</param> 
    /// <param name="path">Path the the embedded resource eg [YourAssembly].[YourResourceFolder].[YourFilename.txt]</param>
    /// <param name="model">Model for the template</param>
    /// <param name="assembly">The assembly your resource is in. Defaults to calling assembly.</param>
    /// <param name="isHtml">True if Body is HTML (default), false for plain text</param>
    /// <returns></returns>
    public static IFluentEmail UsingBootstrapTemplateFromEmbedded<T>(this IFluentEmail email, string path, T model,
        Assembly assembly, bool isHtml = true)
    {
        email.UsingTemplateFromEmbedded(path, model, assembly, isHtml);
        email.CompileBootstrap(isHtml);
        return email;
    }

    /// <summary>
    /// Adds Bootstrap template to email from previously configured default embedded resource
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="email">The IFluentEmail object</param> 
    /// <param name="path">Path the the embedded resource eg [YourResourceFolder].[YourFilename.txt]. Will be appended to configured root path</param>
    /// <param name="model">Model for the template</param>
    /// <param name="isHtml">True if Body is HTML (default), false for plain text</param>
    /// <returns></returns>
    public static IFluentEmail UsingBootstrapTemplateFromEmbedded<T>(this IFluentEmail email, string path, T model,
        bool isHtml = true)
    {
        email.UsingTemplateFromEmbedded(path, model, isHtml);
        email.CompileBootstrap(isHtml);
        return email;
    }

    /// <summary>
    /// Adds Bootstrap template to email from embedded resource
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="email">The IFluentEmail object</param> 
    /// <param name="filename">The path to the file to load</param>
    /// <param name="model">Model for the template</param>
    /// <param name="isHtml">True if Body is HTML (default), false for plain text</param>
    /// <returns></returns>
    public static IFluentEmail UsingBootstrapTemplateFromFile<T>(this IFluentEmail email, string filename, T model,
        bool isHtml = true)
    {
        email.UsingTemplateFromFile(filename, model, isHtml);
        email.CompileBootstrap(isHtml);
        return email;
    }

    /// <summary>
    /// Adds Bootstrap template to email from previously configured default embedded resource
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="email">The IFluentEmail object</param> 
    /// <param name="filename">The path to the file to load</param>
    /// <param name="model">Model for the template</param>
    /// <param name="culture">The culture of the template (Default is the current culture)</param>
    /// <param name="isHtml">True if Body is HTML (default), false for plain text</param>
    /// <returns></returns>
    public static IFluentEmail UsingBootstrapCultureTemplateFromFile<T>(this IFluentEmail email, string filename,
        T model, CultureInfo culture, bool isHtml = true)
    {
        email.UsingCultureTemplateFromFile(filename, model, culture, isHtml);
        email.CompileBootstrap(isHtml);
        return email;
    }
}
