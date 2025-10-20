using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotenv.net;
using dotenv.net.Utilities;
using Microsoft.Graph;

namespace FluentEmail.Core.Tests;

internal static class Credentials
{
    public static TestCredentials MailTrap => new ("MAILTRAP");
    public static MailgunCredentials Mailgun => new("MAILGUN");
    public static TestCredentials Azure => new ("AZURE");
    public static TestCredentials Postmark => new("POSTMARK");
    public static TestCredentials SendGrid => new("SENDGRID");
    public static GraphCredentials Graph => new("GRAPH");

    public static string ToEmail = null;
    public static string FromEmail = null;

    static Credentials()
    {
        DotEnv.Load();
        ToEmail = GetDotEnv("FE_TEST_TO_EMAIL");
        FromEmail = GetDotEnv("FE_TEST_FROM_EMAIL");
    }

    public static string GetDotEnv(string key)
    {
        if (EnvReader.TryGetStringValue(key, out var value)) return value;
        return null;

    }
    public static int? GetDotEnvInt(string key)
    {
        if (EnvReader.TryGetIntValue(key, out var value)) return value;
        return null;

    }
}

internal class TestCredentials
{
    public string Host { get; private set; }
    public string User { get; private set; }
    public string Password { get; private set; }
    public int? Port { get; private set; }
    public string ApiKey { get; private set; }
    public string ApiHost { get; private set; }
    public string Template { get; private set; }
    public string FromEmail { get; private set; }
    public string ToEmail { get; private set; }

    public TestCredentials(string keyBase)
    {
        Host = Credentials.GetDotEnv($"FE_TEST_{keyBase}_HOST");
        User = Credentials.GetDotEnv($"FE_TEST_{keyBase}_USER");
        Password = Credentials.GetDotEnv($"FE_TEST_{keyBase}_PWD");
        Port = Credentials.GetDotEnvInt($"FE_TEST_{keyBase}_PORT") ?? 587;
        ApiKey = Credentials.GetDotEnv($"FE_TEST_{keyBase}_API_KEY");
        ApiHost = Credentials.GetDotEnv($"FE_TEST_{keyBase}_API_HOST");
        Template = Credentials.GetDotEnv($"FE_TEST_{keyBase}_TEMPLATE");
        FromEmail = Credentials.GetDotEnv($"FE_TEST_{keyBase}_FROM_EMAIL");
        ToEmail = Credentials.GetDotEnv($"FE_TEST_{keyBase}_TO_EMAIL");
    }

}
    
internal class MailgunCredentials : TestCredentials
{
    public string Domain { get; set; }
    public MailgunCredentials(string keyBase) : base(keyBase)
    {
        Domain = Credentials.GetDotEnv($"FE_TEST_{keyBase}_DOMAIN");
    }
}

internal class GraphCredentials : TestCredentials
{
    public string AppId { get; set; }
    public string TenantId { get; set; }
    public string ClientSecret { get; set; }
    public GraphCredentials(string keyBase) : base(keyBase)
    {
        AppId = Credentials.GetDotEnv($"FE_TEST_{keyBase}_APP_ID");
        TenantId = Credentials.GetDotEnv($"FE_TEST_{keyBase}_TENANT_ID");
        ClientSecret = Credentials.GetDotEnv($"FE_TEST_{keyBase}_CLIENT_SECRET");
    }
}