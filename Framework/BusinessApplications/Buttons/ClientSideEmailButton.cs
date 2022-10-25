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

namespace Framework.BusinessApplications.Buttons {

    using Framework.Model;
    using Framework.Presentation.Buttons;
    using System.Text;

    /// <summary>
    /// Action button for sending client-side e-mails.
    /// </summary>
    public class ClientSideEmailButton : ClientSideButton {

        /// <summary>
        /// E-Mail template to open on button click.
        /// </summary>
        public EmailTemplate EmailTemplate { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        public ClientSideEmailButton(string title)
            : base(title) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="emailTemplate">e-mail template to open on button
        /// click</param>
        public ClientSideEmailButton(string title, EmailTemplate emailTemplate)
            : this(title) {
            this.EmailTemplate = emailTemplate;
        }

        /// <summary>
        /// Adds a parameter to a mailto string builder.
        /// </summary>
        /// <param name="key">key of parameter to add</param>
        /// <param name="value">value of parameter to add</param>
        /// <param name="mailtoBuilder">mailto string builder to add
        /// parameter to</param>
        /// <param name="isFirstParameter">true if this is the first
        /// parameter to be added, false otherwise</param>
        /// <returns>true if the next parameter to be added is still
        /// the first one, false otherwise</returns>
        private static bool AddParameterToMailtoBuilder(string key, string value, StringBuilder mailtoBuilder, bool isFirstParameter) {
            if (!string.IsNullOrEmpty(value)) {
                if (isFirstParameter) {
                    isFirstParameter = false;
                    mailtoBuilder.Append('?');
                } else {
                    mailtoBuilder.Append("&amp;");
                }
                mailtoBuilder.Append(ClientSideEmailButton.EncodeUrl(key));
                mailtoBuilder.Append('=');
                mailtoBuilder.Append(ClientSideEmailButton.EncodeUrl(value));
            }
            return isFirstParameter;
        }

        /// <summary>
        /// Converts a text string into a URL encoded string.
        /// </summary>
        /// <param name="str">text string to encode</param>
        /// <returns>text string as URL encoded string</returns>
        private static string EncodeUrl(string str) {
            return str.Replace("%", "%25")
                .Replace("\n", "%0A")
                .Replace("\r", "%0D")
                .Replace(" ", "%20")
                .Replace("!", "%21")
                .Replace("#", "%23")
                .Replace("$", "%24")
                .Replace("&", "%26")
                .Replace("*", "%2A")
                .Replace("+", "%2B")
                .Replace(",", "%2C")
                .Replace("-", "%2D")
                .Replace("/", "%2F")
                .Replace(":", "%3A")
                .Replace(";", "%3B")
                .Replace("<", "%3C")
                .Replace("=", "%3D")
                .Replace(">", "%3E")
                .Replace("?", "%3F")
                .Replace("@", "%40")
                .Replace("[", "%5B")
                .Replace("]", "%5D")
                .Replace("Ä", "%C3%84")
                .Replace("ä", "%C3%A4")
                .Replace("Ö", "%C3%96")
                .Replace("ö", "%C3%B6")
                .Replace("Ü", "%C3%9C")
                .Replace("ü", "%C3%BC")
                .Replace("ß", "%C3%9F");
        }

        /// <summary>
        /// Gets the client action to execute on click - it may be
        /// null or empty.
        /// </summary>
        /// <param name="positionId">ID of position of parent widget
        /// or null</param>
        /// <returns>client action to execute on click - it may be
        /// null or empty</returns>
        public override string GetOnClientClick(ulong? positionId) {
            var mailtoBuilder = new StringBuilder();
            mailtoBuilder.Append("javascript:document.location='mailto:");
            string to = this.EmailTemplate.ToAsString;
            if (!string.IsNullOrEmpty(to)) {
                mailtoBuilder.Append(ClientSideEmailButton.EncodeUrl(to));
            }
            bool isFirstParameter = true;
            isFirstParameter = ClientSideEmailButton.AddParameterToMailtoBuilder("cc", this.EmailTemplate.CcAsString, mailtoBuilder, isFirstParameter);
            isFirstParameter = ClientSideEmailButton.AddParameterToMailtoBuilder("bcc", this.EmailTemplate.BccAsString, mailtoBuilder, isFirstParameter);
            isFirstParameter = ClientSideEmailButton.AddParameterToMailtoBuilder("subject", this.EmailTemplate.Subject, mailtoBuilder, isFirstParameter);
            isFirstParameter = ClientSideEmailButton.AddParameterToMailtoBuilder("body", this.EmailTemplate.BodyText, mailtoBuilder, isFirstParameter);
            mailtoBuilder.Append("'");
            return mailtoBuilder.ToString();
        }

    }

}