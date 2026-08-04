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
                new Microsoft.Graph.Authentication.AzureIdentityAuthenticationProvider(tokenCredential, null, null, true, []),
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

        protected virtual Message CreateMessage(IFluentEmail email)
        {
            var message = new Message
            {
                Subject = email.Data.Subject,
                Body = new ItemBody
                {
                    Content = email.Data.Body,
                    ContentType = email.Data.IsHtml ? BodyType.Html : BodyType.Text
                },
                From = new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = email.Data.FromAddress.EmailAddress,
                        Name = email.Data.FromAddress.Name
                    }
                }
            };

            if (email.Data.ToAddresses != null && email.Data.ToAddresses.Count > 0)
            {
                var toRecipients = new List<Recipient>();

                email.Data.ToAddresses.ForEach(r => toRecipients.Add(new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = r.EmailAddress.ToString(),
                        Name = r.Name
                    }
                }));

                message.ToRecipients = toRecipients;
            }

            if (email.Data.BccAddresses != null && email.Data.BccAddresses.Count > 0)
            {
                var bccRecipients = new List<Recipient>();

                email.Data.BccAddresses.ForEach(r => bccRecipients.Add(new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = r.EmailAddress.ToString(),
                        Name = r.Name
                    }
                }));

                message.BccRecipients = bccRecipients;
            }

            if (email.Data.CcAddresses != null && email.Data.CcAddresses.Count > 0)
            {
                var ccRecipients = new List<Recipient>();

                email.Data.CcAddresses.ForEach(r => ccRecipients.Add(new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = r.EmailAddress.ToString(),
                        Name = r.Name
                    }
                }));

                message.CcRecipients = ccRecipients;
            }

            if (email.Data.Attachments != null && email.Data.Attachments.Count > 0)
            {
                message.Attachments = [];

                email.Data.Attachments.ForEach(a =>
                {
                    var attachment = new FileAttachment
                    {
                        Name = a.Filename,
                        ContentType = a.ContentType,
                        ContentBytes = GetAttachmentBytes(a.Data)
                    };

                    message.Attachments.Add(attachment);
                });
            }

            switch (email.Data.Priority)
            {
                case Priority.High:
                    message.Importance = Importance.High;
                    break;
                case Priority.Normal:
                    message.Importance = Importance.Normal;
                    break;
                case Priority.Low:
                    message.Importance = Importance.Low;
                    break;
                default:
                    message.Importance = Importance.Normal;
                    break;
            }
            return message;
        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
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
