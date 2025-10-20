using FluentEmail.Core;
using FluentEmail.Core.Models;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using FluentEmail.Core.Tests;

namespace FluentEmail.Graph.Tests
{
    public class Tests
    {
        private readonly string appId = Credentials.Graph.AppId;
        private readonly string tenantId = Credentials.Graph.TenantId;
        private readonly string graphSecret = Credentials.Graph.ClientSecret;
        private readonly string senderEmail = Credentials.Graph.FromEmail ?? Credentials.FromEmail;
        private readonly string toEmail = Credentials.Graph.ToEmail ?? Credentials.ToEmail;
        private bool saveSent = false;

        [SetUp]
        public void Setup()
        {
            if (string.IsNullOrWhiteSpace(appId)) throw new ArgumentException("Graph App ID needs to be supplied");
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Graph tenant ID needs to be supplied");
            if (string.IsNullOrWhiteSpace(graphSecret)) throw new ArgumentException("Graph client secret needs to be supplied");
            if (string.IsNullOrWhiteSpace(senderEmail)) throw new ArgumentException("Sender email address needs to be supplied");

            var sender = new GraphSender(appId, tenantId, graphSecret, saveSent);

            Email.DefaultSender = sender;
        }

        [Test, Ignore("Missing Graph credentials")]
        public void CanSendEmail()
        {
            var email = Email
                .From(senderEmail)
                .To(toEmail)
                .Subject("Test Email")
                .Body("Test email from Graph sender unit test");

            var response = email.Send();
            Assert.IsTrue(response.Successful);
        }

        [Test, Ignore("Missing Graph credentials")]
        public async Task CanSendEmailAsync()
        {
            var email = Email
                .From(senderEmail)
                .To(toEmail)
                .Subject("Test Async Email")
                .Body("Test email from Graph sender unit test");

            var response = await email.SendAsync();
            Assert.IsTrue(response.Successful);
        }

        [Test, Ignore("Missing Graph credentials")]
        public async Task CanSendEmailWithAttachments()
        {
            var stream = new MemoryStream();
            var sw = new StreamWriter(stream);
            sw.WriteLine("Hey this is some text in an attachment");
            sw.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            var attachment = new Attachment
            {
                ContentType = "text/plain",
                Filename = "graphtest.txt",
                Data = stream
            };

            var email = Email
                .From(senderEmail)
                .To(toEmail)
                .Subject("Test Email with Attachments")
                .Body("Test email from Graph sender unit test")
                .Attach(attachment);

            var response = await email.SendAsync();
            Assert.IsTrue(response.Successful);
        }

        [Test, Ignore("Missing Graph credentials")]
        public async Task CanSendHighPriorityEmail()
        {
            var email = Email
                .From(senderEmail)
                .To(toEmail)
                .Subject("Test High Priority Email")
                .Body("Test email from Graph sender unit test")
                .HighPriority();

            var response = await email.SendAsync();
            Assert.IsTrue(response.Successful);
        }
    }
}