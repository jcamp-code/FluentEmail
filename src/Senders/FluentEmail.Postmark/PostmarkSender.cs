using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using PostmarkDotNet;

namespace FluentEmail.Postmark
{
    /// <summary>
    /// A FluentEmail ISender implementation that uses postmark to send emails.
    /// </summary>
    public class PostmarkSender : ISender
    {
        private readonly PostmarkSenderOptions options;

        /// <summary>
        /// Creates a new instance of the PostmarkSender class.
        /// </summary>
        /// <param name="serverToken">The serverToken to use to authenticate at the Postmark API.</param>
        public PostmarkSender(string serverToken) :
            this(new PostmarkSenderOptions(serverToken ?? throw new ArgumentNullException(nameof(serverToken))))
        {
        }

        /// <summary>
        /// Creates a new instance of the PostmarkSender class.
        /// </summary>
        /// <param name="options">The options to configure the behaviour of this sender.</param>
        public PostmarkSender(PostmarkSenderOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.ServerToken))
                throw new ArgumentException("The ServerToken property of the given options is null or whitespace but it is required to be set to a valid value.", nameof(options.ServerToken));
        }

        /// <summary>
        /// Sends the email using the Postmark API. The implementation is async internally, this is a blocking wrapper.
        /// </summary>
        /// /// <returns>Returns a SendResponse instance that contains the Postmark ErrorCode as only ErrorMessage if the send was not successfull.</returns>
        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            // see https://stackoverflow.com/a/17284612/1200847
            // instead of .Result to not get an AggregateException
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends the email asynchronously using the Postmark API.
        /// </summary>
        /// <returns>Returns a SendResponse instance that contains the Postmark ErrorCode as only ErrorMessage if the send was not successfull.</returns>
        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            _ = email ?? throw new ArgumentNullException(nameof(email));
            var client = new PostmarkClient(options.ServerToken);
            var msg = CreatePostmarkMessage(email);
            var resp = await client.SendMessageAsync(msg).ConfigureAwait(false);
            return CreateSendResponse(resp);            
        }

        private PostmarkMessage CreatePostmarkMessage(IFluentEmail email)
        {
            // see also https://postmarkapp.com/developer/api/email-api

            var msg = new PostmarkMessage
            {
                From = string.IsNullOrEmpty(email.Data.FromAddress.Name) ?
                    email.Data.FromAddress.EmailAddress :
                    $"{email.Data.FromAddress.Name} {email.Data.FromAddress.EmailAddress}",

                To = email.Data.ToAddresses.ToPmAddressString(),
                Cc = email.Data.CcAddresses.ToPmAddressString(),
                Bcc = email.Data.BccAddresses.ToPmAddressString(),
                ReplyTo = email.Data.ReplyToAddresses.ToPmAddressString(),

                Subject = email.Data.Subject
            };

            var pmHeaderEnum = (email.Data.Headers ?? Enumerable.Empty<KeyValuePair<string, string>>())
                .Select(kvp => new PostmarkDotNet.Model.MailHeader(kvp.Key, kvp.Value));
            pmHeaderEnum = pmHeaderEnum.Concat(email.Data.Priority.GetPmHeaders()); // add headers representing prio
            foreach (var pmh in pmHeaderEnum)
            {
                msg.Headers.Add(pmh);
            }

            if (email.Data.IsHtml)
            {
                msg.HtmlBody = email.Data.Body;
                msg.TextBody = email.Data.PlaintextAlternativeBody;
            }
            else
            {
                msg.TextBody = email.Data.Body;
            }

            foreach (var attachment in email.Data.Attachments ?? Enumerable.Empty<Core.Models.Attachment>())
            {
                msg.AddAttachment(attachment.Data, attachment.Filename, attachment.ContentType, attachment.ContentId);
            }

            msg.TrackOpens = options.TrackOpens;
            msg.TrackLinks = options.TrackLinks;
            if (options.Tag != null)
                msg.Tag = options.Tag;
            if (options.Metadata != null)
                msg.Metadata = options.Metadata;

            return msg;
        }

        private static SendResponse CreateSendResponse(PostmarkResponse value)
        {
            var ret = new SendResponse();
            ret.MessageId = value.MessageID.ToString();
            if (value.Status == PostmarkStatus.Success) return ret;
            ret.ErrorMessages.Add(value.ErrorCode.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return ret;
        }
    }
}
