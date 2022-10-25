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

    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mail;

    /// <summary>
    /// Represents a plain-text e-mail message.
    /// </summary>
    public class PlainTextEmail : Email {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PlainTextEmail()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Initializes the mail message to be sent.
        /// </summary>
        /// <param name="mailMessage">mail message to be initialized</param>
        /// <param name="streamCollection">collection of streams to
        /// be disposed later</param>
        protected sealed override void InitializeMailMessage(MailMessage mailMessage, DisposableCollection<MemoryStream> streamCollection) {
            base.InitializeMailMessage(mailMessage, streamCollection);
            mailMessage.IsBodyHtml = false;
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
        public static void SendAsync(IPerson from, IEnumerable<IPerson> to, IEnumerable<IPerson> cc, string subject, string bodyText) {
            PlainTextEmail.SendAsync(from, to, cc, subject, bodyText, null);
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
        public static void SendAsync(IPerson from, IEnumerable<Persistence.Group> to, IEnumerable<Persistence.Group> cc, string subject, string bodyText) {
            PlainTextEmail.SendAsync(from, Persistence.Group.MembersOf(to), Persistence.Group.MembersOf(cc), subject, bodyText);
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
        public static void SendAsync(IPerson from, IEnumerable<IPerson> to, IEnumerable<IPerson> cc, string subject, string bodyText, IEnumerable<KeyValuePair<string, string>> placeholders) {
            PlainTextEmail.SendAsync(from, to, cc, subject, bodyText, placeholders, MailPriority.Normal);
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
        public static void SendAsync(IPerson from, IEnumerable<Persistence.Group> to, IEnumerable<Persistence.Group> cc, string subject, string bodyText, IEnumerable<KeyValuePair<string, string>> placeholders) {
            PlainTextEmail.SendAsync(from, Persistence.Group.MembersOf(to), Persistence.Group.MembersOf(cc), subject, bodyText, placeholders);
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
        public static void SendAsync(IPerson from, IEnumerable<Persistence.Group> to, IEnumerable<Persistence.Group> cc, string subject, string bodyText, IEnumerable<KeyValuePair<string, string>> placeholders, MailPriority priority) {
            PlainTextEmail.SendAsync(from, Persistence.Group.MembersOf(to), Persistence.Group.MembersOf(cc), subject, bodyText, placeholders, priority);
            return;
        }

    }

}