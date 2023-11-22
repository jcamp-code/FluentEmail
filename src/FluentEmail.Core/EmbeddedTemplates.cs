using System;
using System.Reflection;

namespace FluentEmail.Core;

public static class EmbeddedTemplates
{
    private static Assembly _assembly;
    private static string _rootPath;

    public static void Configure(Assembly assembly, string rootPath)
    {
        _assembly = assembly;
        _rootPath = rootPath;
    }
    
    /// <summary>
    /// Adds template to email from previously configured default embedded resource
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">Path the the embedded resource eg [YourResourceFolder].[YourFilename.txt]. Will be appended to configured root path</param>
    /// <param name="model">Model for the template</param>
    /// <param name="isHtml">True if Body is HTML (default), false for plain text</param>
    /// <returns></returns>
    public static IFluentEmail UsingTemplateFromEmbedded<T>(this IFluentEmail email, string path, T model, bool isHtml = true)
    {
        if (_assembly is null)
        {
            throw new Exception("FluentEmail.Core.EmbeddedTemplates.Configure must be called with default assembly and root path");
        }

        var root = _rootPath;
        if (!string.IsNullOrEmpty(root)) root += ".";
        var template = EmbeddedResourceHelper.GetResourceAsString(_assembly, $"{root}{path}");
        var result = email.Renderer.Parse(template, model, isHtml);
        email.Data.IsHtml = isHtml;
        email.Data.Body = result;

        return email;
    }
    
    /// <summary>
    /// Adds alternative plaintext template to email from previously configured embedded resource
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">Path the the embedded resource eg [YourResourceFolder].[YourFilename.txt]. Will be appended to configured root path</param>
    /// <param name="model">Model for the template</param>
    /// <returns></returns>
    public static IFluentEmail PlaintextAlternativeUsingTemplateFromEmbedded<T>(this IFluentEmail email, string path, T model)
    {
        var root = _rootPath;
        if (!string.IsNullOrEmpty(root)) root += ".";
        var template = EmbeddedResourceHelper.GetResourceAsString(_assembly, $"{root}{path}");
        var result = email.Renderer.Parse(template, model, false);
        email.Data.PlaintextAlternativeBody = result;

        return email;
    }

}
