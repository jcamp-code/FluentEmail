using System;
using System.Collections.Generic;
using System.Text;

namespace FluentEmail.Postmark
{
    /// <summary>
    /// This class provides options for a PostmarkSender instance. It allows to configure per message properties
    /// that aren't present in FluentEmail such as Tag. They are applied to all messages sent by the PostmarkSender
    /// instance that uses this options.
    /// </summary>
    public class PostmarkSenderOptions
    {
        /// <summary>
        /// Creates a new instance of the PostmarkSenderOptions class.
        /// </summary>
        public PostmarkSenderOptions()
        {
        }

        /// <summary>
        /// Creates a new instance of the PostmarkSenderOptions class.
        /// </summary>
        /// <param name="serverToken">The serverToken to use to authenticate at the Postmark API.</param>
        public PostmarkSenderOptions(string serverToken)
        {
            ServerToken = serverToken ?? throw new ArgumentNullException(nameof(serverToken));
        }

        /// <summary>
        /// Sending requires server level privileges. This token can be found on the API Tokens tab under your Postmark server.
        /// </summary>
        public string ServerToken { get; set; } = "";

        /// <summary>
        /// Activate open tracking for this email.
        /// </summary>
        public bool? TrackOpens { get; set; }

        /// <summary>
        /// Activate link tracking for links in the HTML or Text bodies of this email. Possible options: None, HtmlAndText, HtmlOnly, TextOnly
        /// </summary>
        public PostmarkDotNet.LinkTrackingOptions TrackLinks { get; set; }


        /// <summary>
        /// Custom metadata key/value pairs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Sammlungseigenschaften müssen schreibgeschützt sein",
            Justification = "This should be set-able as part of this options type.")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Email tag that allows you to categorize outgoing emails and get detailed statistics.
        /// </summary>
        public string Tag { get; set; }
    }
}
