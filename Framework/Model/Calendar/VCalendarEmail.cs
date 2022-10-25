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

namespace Framework.Model.Calendar {

    using Framework.Presentation;
    using Framework.Presentation.Converters;
    using Framework.Presentation.Forms;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net.Mail;
    using System.Net.Mime;

    /// <summary>
    /// Represents an HMTL e-mail message for calendaring actions.
    /// </summary>
    public class VCalendarEmail : Email {

        /// <summary>
        /// RFC 5546 method to be used.
        /// </summary>
        protected Method Method { get; private set; }

        /// <summary>
        /// VCalendarWriter or VEventListWriter to be used for
        /// writing VCalendar data.
        /// </summary>
        protected object VDataWriter { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="method">RFC 5546 method to be used</param>
        /// <param name="item">event to be sent</param>
        public VCalendarEmail(Method method, Event item)
            : base() {
            this.BodyText = XmlUtility.RemoveTags(item.Description);
            this.Method = method;
            this.Subject = item.Title;
            this.VDataWriter = new VCalendarWriter(method, new Event[] { item });
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="method">RFC 5546 method to be used</param>
        /// <param name="item">event to be sent</param>
        /// <param name="view">event list view to be applied for
        /// field mapping</param>
        public VCalendarEmail(Method method, IPresentableObject item, VEventListView view)
            : base() {
            var bodyTextField = item.FindPresentableField(view.DescriptionKey) as IPresentableFieldForElement;
            if (null != bodyTextField) {
                this.BodyText = XmlUtility.RemoveTags(bodyTextField.ValueAsString);
            }
            this.Method = method;
            var subjectField = item.FindPresentableField(view.TitleKey) as IPresentableFieldForElement;
            if (null != subjectField) {
                this.Subject = subjectField.ValueAsString;
            }
            this.VDataWriter = new VEventListWriter(view, method, new IPresentableObject[] { item });
        }

        /// <summary>
        /// Initializes the mail message to be sent.
        /// </summary>
        /// <param name="mailMessage">mail message to be initialized</param>
        /// <param name="streamCollection">collection of streams to
        /// be disposed later</param>
        protected sealed override void InitializeMailMessage(MailMessage mailMessage, DisposableCollection<MemoryStream> streamCollection) {
            base.InitializeMailMessage(mailMessage, streamCollection);
            var contentType = new ContentType("text/calendar") {
                CharSet = "utf-8"
            };
            contentType.Parameters.Add("method", this.Method.ToString().ToUpperInvariant());
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(this.VDataWriter.ToString(), contentType));
            return;
        }

        /// <summary>
        /// Sends an email message asynchronously.
        /// </summary>
        /// <param name="to">primary receivers</param>
        /// <param name="method">RFC 5546 method to be used</param>
        /// <param name="item">event to be sent</param>
        public static void SendAsync(IEnumerable<Persistence.Group> to, Method method, Event item) {
            VCalendarEmail.SendAsync(Persistence.Group.MembersOf(to), method, item);
            return;
        }

        /// <summary>
        /// Sends an email message asynchronously.
        /// </summary>
        /// <param name="to">primary receivers</param>
        /// <param name="method">RFC 5546 method to be used</param>
        /// <param name="item">event to be sent</param>
        /// <param name="view">event list view to be applied for
        /// field mapping</param>
        public static void SendAsync(IEnumerable<Persistence.Group> to, Method method, IPresentableObject item, VEventListView view) {
            VCalendarEmail.SendAsync(Persistence.Group.MembersOf(to), method, item, view);
            return;
        }

        /// <summary>
        /// Sends an email message asynchronously.
        /// </summary>
        /// <param name="to">primary receivers</param>
        /// <param name="method">RFC 5546 method to be used</param>
        /// <param name="item">event to be sent</param>
        public static void SendAsync(IEnumerable<IPerson> to, Method method, Event item) {
            if (!item.IsSequenceNumberRelevantFieldChanged && Method.Cancel == method) {
                item.SequenceNumber++;
            }
            var vCalendarEmail = new VCalendarEmail(method, item);
            foreach (var receiver in to) {
                vCalendarEmail.To.Add(receiver);
            }
            vCalendarEmail.SendAsync();
            return;
        }

        /// <summary>
        /// Sends an email message asynchronously.
        /// </summary>
        /// <param name="to">primary receivers</param>
        /// <param name="method">RFC 5546 method to be used</param>
        /// <param name="item">event to be sent</param>
        /// <param name="view">event list view to be applied for
        /// field mapping</param>
        public static void SendAsync(IEnumerable<IPerson> to, Method method, IPresentableObject item, VEventListView view) {
            if (Method.Cancel == method) {
                var sequenceField = item.FindPresentableField(view.SequenceKey) as IPresentableFieldForElement;
                if (null != sequenceField) {
                    sequenceField.ValueAsString = (long.Parse(sequenceField.ValueAsString) + 1).ToString(CultureInfo.InvariantCulture);
                }
            }
            var vCalendarEmail = new VCalendarEmail(method, item, view);
            foreach (var receiver in to) {
                vCalendarEmail.To.Add(receiver);
            }
            vCalendarEmail.SendAsync();
            return;
        }

    }

}