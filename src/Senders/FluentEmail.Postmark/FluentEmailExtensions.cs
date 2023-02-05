using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentEmail.Core.Models;

namespace FluentEmail.Postmark
{
    internal static class FluentEmailExtensions
    {
        public static string ToPmAddressString(this IList<Address> addresses)
        {
            var adrStrs = addresses?.Select(a => a.EmailAddress).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (adrStrs == null || adrStrs.Count == 0) return null;
            if (adrStrs.Count > 50) throw new ArgumentException("Postmark does not support sending to more than 50 recipients at once");
            return string.Join(",", adrStrs);
        }

        public static IEnumerable<PostmarkDotNet.Model.MailHeader> GetPmHeaders(this Priority value)
        {
            foreach ((var name, var val) in value.GetMailHeaders())
            {
                yield return new PostmarkDotNet.Model.MailHeader(name, val);
            }
        }

        public static IEnumerable<(string name, string value)> GetMailHeaders(this Priority value)
        {
            // based on https://github.com/lukencode/FluentEmail/blob/master/src/Senders/FluentEmail.SendGrid/SendGridSender.cs
            switch (value)
            {
                case Priority.High:
                    // https://stackoverflow.com/questions/23230250/set-email-priority-with-sendgrid-api
                    yield return ("Priority", "Urgent");
                    yield return ("Importance", "High");
                    // https://docs.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-oxcmail/2bb19f1b-b35e-4966-b1cb-1afd044e83ab
                    yield return ("X-Priority", "1");
                    yield return ("X-MSMail-Priority", "High");
                    break;

                case Priority.Low:
                    // https://stackoverflow.com/questions/23230250/set-email-priority-with-sendgrid-api
                    yield return ("Priority", "Non-Urgent");
                    yield return ("Importance", "Low");
                    // https://docs.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-oxcmail/2bb19f1b-b35e-4966-b1cb-1afd044e83ab
                    yield return ("X-Priority", "5");
                    yield return ("X-MSMail-Priority", "Low");
                    break;

                case Priority.Normal:
                default:
                    break;
            }
        }
    }
}
