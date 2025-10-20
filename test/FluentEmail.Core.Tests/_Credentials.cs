using dotenv.net;
using dotenv.net.Utilities;

namespace FluentEmail.Core.Tests;

internal static class Credentials
{
    public static TestCredentials MailTrap => new ("MAILTRAP");
    public static MailgunCredentials Mailgun => new("MAILGUN");
    public static TestCredentials Azure => new ("AZURE");
    public static TestCredentials Postmark => new("POSTMARK");
    public static TestCredentials SendGrid => new("SENDGRID");
    public static GraphCredentials Graph => new("GRAPH");

    public static string ToEmail;
    public static string FromEmail;

    static Credentials()
    {
        DotEnv.Load();
        ToEmail = GetDotEnvString("FE_TEST_TO_EMAIL");
        FromEmail = GetDotEnvString("FE_TEST_FROM_EMAIL");
    }

    public static string GetDotEnvString(string key) => EnvReader.TryGetStringValue(key, out var value) ? value : null;

    public static int? GetDotEnvInt(string key) => EnvReader.TryGetIntValue(key, out var value) ? value : null;
}

internal class TestCredentials(string keyBase)
{
    public string Host { get; private set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_HOST");
    public string User { get; private set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_USER");
    public string Password { get; private set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_PWD");
    public int? Port { get; private set; } = Credentials.GetDotEnvInt($"FE_TEST_{keyBase}_PORT") ?? 587;
    public string ApiKey { get; private set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_API_KEY");
    public string ApiHost { get; private set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_API_HOST");
    public string Template { get; private set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_TEMPLATE");
    public string FromEmail { get; private set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_FROM_EMAIL");
    public string ToEmail { get; private set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_TO_EMAIL");
}
    
internal class MailgunCredentials(string keyBase) : TestCredentials(keyBase)
{
    public string Domain { get; set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_DOMAIN");
}

internal class GraphCredentials(string keyBase) : TestCredentials(keyBase)
{
    public string AppId { get; set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_APP_ID");
    public string TenantId { get; set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_TENANT_ID");
    public string ClientSecret { get; set; } = Credentials.GetDotEnvString($"FE_TEST_{keyBase}_CLIENT_SECRET");
}