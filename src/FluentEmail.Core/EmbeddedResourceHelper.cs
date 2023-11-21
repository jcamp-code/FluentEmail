using System;
using System.IO;
using System.Reflection;

namespace FluentEmail.Core
{
    internal static class EmbeddedResourceHelper
    {
        internal static string GetResourceAsString(Assembly assembly, string path)
        {
            using var stream = assembly.GetManifestResourceStream(path);
            if (stream is null)
            {
                throw new Exception($"{path} was not found in embedded resources.");
            }

            using var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();
            return result;
        }
    }
}
