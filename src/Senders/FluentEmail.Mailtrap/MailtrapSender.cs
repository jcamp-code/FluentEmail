using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Mailtrap.HttpHelpers;
using FluentEmail.Smtp;

namespace FluentEmail.Mailtrap
{
    /// <summary>
    /// Send emails to a Mailtrap.io inbox
    /// </summary>
    public class MailtrapSender : IMailtrapSender, IDisposable
    {
        private const string URL = "https://send.api.mailtrap.io/api/send";
        private readonly SmtpClient _smtpClient;
        private static readonly int[] ValidPorts = {25,587, 2525};
        private readonly string _apiKey;

        /// <summary>
        /// Creates a sender that uses the given Mailtrap credentials, but does not dispose it.
        /// </summary>
        /// <param name="userName">Username of your mailtrap.io SMTP inbox</param>
        /// <param name="password">Password of your mailtrap.io SMTP inbox</param>
        /// <param name="host">Host address for the Mailtrap.io SMTP inbox</param>
        /// <param name="port">Port for the Mailtrap.io SMTP server. Accepted values are 25, 465 or 2525.</param>
        public MailtrapSender(string userName, string password, string host = "smtp.mailtrap.io", int? port = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Mailtrap UserName needs to be supplied", nameof(userName));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Mailtrap Password needs to be supplied", nameof(password));

            if (port.HasValue && !ValidPorts.Contains(port.Value))
                throw new ArgumentException("Mailtrap Port needs to be either 25, 465 or 2525", nameof(port));
            _apiKey = password;
            _smtpClient = new SmtpClient(host, port.GetValueOrDefault(587))
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = true
            };
        }
        
        public void Dispose() => _smtpClient?.Dispose();
        
        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            var smtpSender = new SmtpSender(_smtpClient);
            return smtpSender.Send(email, token);
        }

        public Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            var smtpSender = new SmtpSender(_smtpClient);
            return smtpSender.SendAsync(email, token);
        }

        public async Task<SendResponse> SendWithTemplateAsync(IFluentEmail email, string templateName, object templateData, CancellationToken? token = null)
        {
            token?.ThrowIfCancellationRequested();
            using (var httpClient = new HttpClient { BaseAddress = new Uri(URL) })
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var jsonContent = HttpClientHelpers.GetJsonBody(BuildMailtrapParameters(email, templateName, templateData));
                var response = await httpClient.Post<MailtrapResponse>(URL, jsonContent);
                var result = new SendResponse { MessageId = response.Data?.Id };
                if (!response.Success)
                {
                    result.ErrorMessages.AddRange(response.Errors.Select(x => x.ErrorMessage));
                    return result;
                }
                return result;
            }
          
        }

        private static Dictionary<string, object> BuildMailtrapParameters(IFluentEmail email, string templateName, object templateData)
        {
            var parameters = new Dictionary<string, object>();
            parameters["from"] = new Dictionary<string, string>{
                {"email", email.Data.FromAddress.EmailAddress},
                {"name", email.Data.FromAddress.Name}
            };
            var to= new List<Dictionary<string, string>>();
            email.Data.ToAddresses.ForEach(x =>
            {
                to.Add(new Dictionary<string, string> { { "email", x.EmailAddress } });
            });
            parameters["to"] = to;
            var cc = new List<Dictionary<string, string>>();
            email.Data.CcAddresses.ForEach(x =>
            {
                cc.Add(new Dictionary<string, string> { { "email", x.EmailAddress } });
            });
            if (cc.Any())
            {
                parameters["cc"] = to;
            }
            var bcc = new List<Dictionary<string, string>>();
            email.Data.BccAddresses.ForEach(x =>
            {
                bcc.Add(new Dictionary<string, string> { { "email", x.EmailAddress } });
            });
            if (cc.Any())
            {
                parameters["bcc"] = bcc;
            }
            parameters["template_uuid"] = templateName;
            var templateVariables = templateData.GetType().GetProperties()
            .ToDictionary(
                prop => prop.Name,
                prop => prop.GetValue(templateData).ToString()
            );
            parameters["template_variables"] = templateVariables;
            return parameters;
        }
    }
}
