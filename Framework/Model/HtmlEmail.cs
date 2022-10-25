/*********************************************************************
 * Umanu Framework / (C) Umanu Team / http://www.umanu.de/           *
 *                                                                   *
 * This program is free software: you can redistribute it and/or     *
 * modify it under the terms of the GNU Lesser General Public        *
 * License as published by the Free Software Foundation, either      *
 * version 3 of the License, or (at your option) any later version.  *
 *                                                                   *
 * This program is distributed in the hope that it will be useful,   *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of    *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the     *
 * GNU Lesser General Public License for more details.               *
 *                                                                   *
 * You should have received a copy of the GNU Lesser General Public  *
 * License along with this program.                                  *
 * If not, see <http://www.gnu.org/licenses/>.                       *
 *********************************************************************/

namespace Framework.Model {

    using Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Text;

    /// <summary>
    /// Represents an HMTL e-mail message.
    /// </summary>
    public class HtmlEmail : Email {

        /// <summary>
        /// Alternate plain text body of email to be displayed if
        /// email client of receiver does not support HTML.
        /// </summary>
        public string BodyPlainText {
            get { return this.bodyPlainText.Value; }
            set { this.bodyPlainText.Value = value; }
        }
        private readonly PresentableFieldForString bodyPlainText;

        /// <summary>
        /// Indicates whether hyperlinks, mail addresses and times
        /// are supposed to be autodetected and embedded into HTML
        /// tags.
        /// </summary>
        public bool? IsEmbeddingLinksAndTimesIntoHtmlTags {
            get { return this.isEmbeddingLinksAndTimesIntoHtmlTags.Value; }
            set { this.isEmbeddingLinksAndTimesIntoHtmlTags.Value = value; }
        }
        private readonly PresentableFieldForNullableBool isEmbeddingLinksAndTimesIntoHtmlTags;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public HtmlEmail()
            : base() {
            this.isEmbeddingLinksAndTimesIntoHtmlTags = new PresentableFieldForNullableBool(this, nameof(this.IsEmbeddingLinksAndTimesIntoHtmlTags));
            this.bodyPlainText = new PresentableFieldForString(this, nameof(this.BodyPlainText));
            this.AddPresentableField(this.bodyPlainText);
        }

        /// <summary>
        /// Initializes the mail message to be sent.
        /// </summary>
        /// <param name="mailMessage">mail message to be initialized</param>
        /// <param name="streamCollection">collection of streams to
        /// be disposed later</param>
        protected sealed override void InitializeMailMessage(MailMessage mailMessage, DisposableCollection<System.IO.MemoryStream> streamCollection) {
            base.InitializeMailMessage(mailMessage, streamCollection);
            if (!string.IsNullOrEmpty(mailMessage.Body) && true == this.IsEmbeddingLinksAndTimesIntoHtmlTags) {
                mailMessage.Body = XmlUtility.ReplaceEmailAddressesInText(mailMessage.Body, "<a href=\"mailto:$1\">$1</a>");
                mailMessage.Body = Regex.ForHyperlinkInText.Replace(mailMessage.Body, "<a href=\"$1\" rel=\"noopener\" target=\"_blank\">$1</a>");
            }
            if (HtmlEmail.ReplaceBase64Images(mailMessage, streamCollection)) {
                if (string.IsNullOrEmpty(this.BodyPlainText)) {
                    mailMessage.Body = "Please enable HTML to view this message.";
                } else {
                    mailMessage.Body = this.BodyPlainText;
                }
            } else {
                if (string.IsNullOrEmpty(this.BodyPlainText)) {
                    mailMessage.IsBodyHtml = true;
                } else {
                    mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(mailMessage.Body, Encoding.UTF8, "text/html"));
                    mailMessage.Body = this.BodyPlainText;
                }
            }
            return;
        }

        /// <summary>
        /// Replaces all Base64 encoded images by references to
        /// linked resources.
        /// </summary>
        /// <param name="mailMessage">mail message to replace Base64
        /// encoded images in</param>
        /// <param name="streamCollection">collection of streams to
        /// be disposed later</param>
        /// <returns>true if images have been replaced, false
        /// otherwise</returns>
        private static bool ReplaceBase64Images(MailMessage mailMessage, DisposableCollection<System.IO.MemoryStream> streamCollection) {
            var linkedResources = new List<LinkedResource>();
            int preambleStartIndex;
            int identifierStartIndex;
            do {
                preambleStartIndex = mailMessage.Body.IndexOf("data:image/");
                identifierStartIndex = mailMessage.Body.IndexOf(";base64,");
                if (preambleStartIndex > -1 && identifierStartIndex > -1) {
                    string contentId = "image" + (linkedResources.Count + 1);
                    string mimeType = mailMessage.Body.Substring(preambleStartIndex + 5, identifierStartIndex - preambleStartIndex - 5);
                    int imageStartIndex = identifierStartIndex + 8;
                    int imageEndIndex = mailMessage.Body.IndexOf("\"", imageStartIndex);
                    string base64Image = mailMessage.Body.Substring(imageStartIndex, imageEndIndex - imageStartIndex);
                    mailMessage.Body = mailMessage.Body.Remove(preambleStartIndex, imageEndIndex - preambleStartIndex);
                    mailMessage.Body = mailMessage.Body.Insert(preambleStartIndex, "cid:" + contentId);
                    var fileStream = new System.IO.MemoryStream(Convert.FromBase64String(base64Image));
                    streamCollection.Add(fileStream);
                    var linkedResource = new LinkedResource(fileStream, mimeType) {
                        ContentId = contentId
                    };
                    linkedResources.Add(linkedResource);
                }
            } while (preambleStartIndex > -1 && identifierStartIndex > -1);
            bool hasImagesReplaced = linkedResources.Count > 0;
            if (hasImagesReplaced) {
                var htmlView = AlternateView.CreateAlternateViewFromString(mailMessage.Body, Encoding.UTF8, "text/html");
                foreach (var linkedResource in linkedResources) {
                    htmlView.LinkedResources.Add(linkedResource);
                }
                mailMessage.AlternateViews.Add(htmlView);
            }
            return hasImagesReplaced;
        }

        /// <summary>
        /// Sends an email message asynchronously.
        /// </summary>
        /// <param name="from">sender</param>
        /// <param name="to">primary receivers</param>
        /// <param name="cc">copy receivers</param>
        /// <param name="subject">subject of mail</param>
        /// <param name="bodyText">body text of e-mail</param>
        public static void SendAsync(IPerson from, IEnumerable<IPerson> to, IEnumerable<IPerson> cc, string subject, string bodyText) {
            HtmlEmail.SendAsync(from, to, cc, subject, bodyText, null);
            return;
        }

        /// <summary>
        /// Sends an email message asynchronously.
        /// </summary>
        /// <param name="from">sender</param>
        /// <param name="to">primary receivers</param>
        /// <param name="cc">copy receivers</param>
        /// <param name="subject">subject of mail</param>
        /// <param name="bodyText">body text of e-mail</param>
        /// <param name="placeholders">pairs of keys and values to
        /// replace in subject and body text</param>
        public static void SendAsync(IPerson from, IEnumerable<IPerson> to, IEnumerable<IPerson> cc, string subject, string bodyText, IEnumerable<KeyValuePair<string, string>> placeholders) {
            HtmlEmail.SendAsync(from, to, cc, subject, bodyText, placeholders, MailPriority.Normal);
            return;
        }

        /// <summary>
        /// Sends an email message asynchronously.
        /// </summary>
        /// <param name="from">sender</param>
        /// <param name="to">primary receivers</param>
        /// <param name="cc">copy receivers</param>
        /// <param name="subject">subject of mail</param>
        /// <param name="bodyText">body text of e-mail</param>
        /// <param name="placeholders">pairs of keys and
        /// values to replace in subject and body text</param>
        /// <param name="priority">priority of mail</param>
        public static void SendAsync(IPerson from, IEnumerable<IPerson> to, IEnumerable<IPerson> cc, string subject, string bodyText, IEnumerable<KeyValuePair<string, string>> placeholders, MailPriority priority) {
            Email.SendAsync<PlainTextEmail>(from, to, cc, subject, bodyText, placeholders, priority);
            return;
        }

    }

}