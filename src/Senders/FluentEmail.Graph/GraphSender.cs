using Azure.Core;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace FluentEmail.Graph
{
    public class GraphSender(GraphServiceClient graphClient, bool saveSentItems) : ISender
    {
        public static readonly bool DefaultSaveSentItems = true;

        private readonly bool _saveSent = saveSentItems;
        private readonly GraphServiceClient _graphClient = graphClient;

        public GraphSender(GraphServiceClient graphClient)
            : this(graphClient, DefaultSaveSentItems)
        { }

        public GraphSender(
            IAuthenticationProvider authProvider,
            bool saveSentItems)
            : this(authProvider, saveSentItems, null)
        {
        }

        public GraphSender(IAuthenticationProvider authProvider, bool saveSentItems, string? baseUrl)
            : this(
                  new GraphServiceClient(authProvider, baseUrl),
                  saveSentItems
             )
        {
        }

        public GraphSender(
            TokenCredential tokenCredential,
            bool SaveSentItems,
            IEnumerable<string> scopes,
            string? baseUrl
        ) : this(
                new Microsoft.Graph.Authentication.AzureIdentityAuthenticationProvider(tokenCredential, null, null, true, scopes?.ToArray() ?? []),
                SaveSentItems,
                baseUrl
            )
        {
        }

        public GraphSender(
            TokenCredential tokenCredential,
            bool SaveSentItems,
            IEnumerable<string> scopes
        ) : this(
                tokenCredential,
                SaveSentItems,
                scopes,
                null
            )
        {
        }

        public GraphSender(
            TokenCredential tokenCredential,
            bool SaveSentItems
        ) : this(
                new Microsoft.Graph.Authentication.AzureIdentityAuthenticationProvider(tokenCredential, null, null, true, Array.Empty<string>()),
                SaveSentItems,
                null
            )
        {
        }

        public GraphSender(
            string GraphEmailAppId,
            string GraphEmailTenantId,
            string GraphEmailSecret,
            bool SaveSentItems,
            IEnumerable<string> scopes,
            string? baseUrl)
            : this(new ClientAuthHandler(GraphEmailAppId, GraphEmailTenantId, GraphEmailSecret), SaveSentItems, scopes, baseUrl)
        {
        }

        public GraphSender(
            string GraphEmailAppId,
            string GraphEmailTenantId,
            string GraphEmailSecret,
            bool SaveSentItems)
            : this(new ClientAuthHandler(GraphEmailAppId, GraphEmailTenantId, GraphEmailSecret), SaveSentItems)
        {
        }

        public GraphSender(
            string GraphEmailAppId,
            string GraphEmailTenantId,
            string GraphEmailSecret,
            bool SaveSentItems,
            IEnumerable<string> scopes)
            : this(new ClientAuthHandler(GraphEmailAppId, GraphEmailTenantId, GraphEmailSecret), SaveSentItems, scopes, null)
        {
        }

        private static Recipient? CreateRecipient(Address address)
        {
            if (address == null || string.IsNullOrWhiteSpace(address.EmailAddress))
            {
                return null;
            }
            return new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = address.EmailAddress,
                    Name = address.Name
                }
            };
        }

        private static List<Recipient>? CreateRecipients(IList<Address> recipients)
        {
            if (recipients.Count == 0)
            { 
                return null; 
            }
            var result = new List<Recipient>();
            foreach (var r in recipients)
            {
                var recipient = CreateRecipient(r);
                if (recipient != null)
                {
                    result.Add(recipient);
                }
            }
            if (result.Count == 0)
            {
                return null;
            }
            return result;
        }

        protected virtual Message CreateMessage(IFluentEmail email)
        {
            var message = new Message
            {
                Subject = email.Data.Subject,
                Body = new ItemBody
                {
                    Content = email.Data.Body,
                    ContentType = email.Data.IsHtml ? BodyType.Html : BodyType.Text,
                }
            };

            if (CreateRecipient(email.Data.FromAddress) is { } f)
            {
                message.From = f;
            }
            if (CreateRecipients(email.Data.ReplyToAddresses) is { } replyTos)
            {
                message.ReplyTo = replyTos;
            }
            if (CreateRecipients(email.Data.ToAddresses) is { } toRecipients)
            {
                message.ToRecipients = toRecipients;
            }
            if (CreateRecipients(email.Data.BccAddresses) is { } bccRecipients)
            {
                message.BccRecipients = bccRecipients;
            }
            if (CreateRecipients(email.Data.CcAddresses) is { } ccRecipients)
            {
                message.CcRecipients = ccRecipients;
            }

            if (email.Data.Attachments is { Count: > 0 })
            {
                message.Attachments = [];
                foreach(var a in email.Data.Attachments)
                {
                    var attachment = new FileAttachment
                    {
                        Name = a.Filename,
                        ContentType = a.ContentType,
                        IsInline = a.IsInline,
                        ContentBytes = GetAttachmentBytes(a.Data)
                    };
                    message.Attachments.Add(attachment);
                }
            }

            message.Importance = email.Data.Priority switch
            {
                Priority.High => (Importance?)Importance.High,
                Priority.Normal => (Importance?)Importance.Normal,
                Priority.Low => (Importance?)Importance.Low,
                _ => (Importance?)Importance.Normal,
            };
            return message;
        }

        Task<SendResponse> ISender.SendAsync(IFluentEmail email, CancellationToken? token)
        {
            return SendAsync(email, token);
        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token)
        {
            if (token.HasValue)
            {
                return SendAsync(email, token.Value);
            }
            else
            {
                return SendAsync(email, CancellationToken.None);
            }
        }

        public Task<SendResponse> SendAsync(IFluentEmail email)
            => SendAsync(email, CancellationToken.None);

        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken cancellationToken)
        {
            try
            {
                var message = CreateMessage(email);
                if (email is { Data.FromAddress.EmailAddress: { Length: > 0 } addr})
                {
                    var builder = _graphClient.Users[addr].SendMail;
                    await builder.PostAsync(
                        new()
                        {
                            Message = message,
                            SaveToSentItems = _saveSent
                        },
                        default,
                        cancellationToken
                    );
                    return new SendResponse
                    {
                        MessageId = message.Id
                    };
                }
                else
                {
                    var builder = _graphClient.Me.SendMail;
                    await builder.PostAsync(
                        new()
                        {
                            Message = message,
                            SaveToSentItems = _saveSent
                        },
                        default,
                        cancellationToken
                    );
                    return new SendResponse
                    {
                        MessageId = message.Id
                    };
                }
            }
            catch (Exception ex)
            {
                return new SendResponse
                {
                    ErrorMessages = [ ex.Message ]
                };
            }
        }

        private static byte[] GetAttachmentBytes(Stream stream)
        {
            using var m = new MemoryStream();
            stream.CopyTo(m);
            return m.ToArray();
        }
    }
}
