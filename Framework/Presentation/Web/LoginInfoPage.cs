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

namespace Framework.Presentation.Web {

    using Framework.Properties;
    using System.Collections.Generic;

    /// <summary>
    /// Lightweight system info web page without references.
    /// </summary>
    internal sealed class LoginInfoPage : Page {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of page</param>
        /// <param name="text">text to be displayed</param>
        /// <param name="imprintUrl">URL of imprint page</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        public LoginInfoPage(string title, string text, string imprintUrl, string privacyNoticeUrl)
            : this(title, text, imprintUrl, privacyNoticeUrl, new Control[0]) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of page</param>
        /// <param name="text">text to be displayed</param>
        /// <param name="imprintUrl">URL of imprint page</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        /// <param name="buttonControls">controls of buttons</param>
        public LoginInfoPage(string title, string text, string imprintUrl, string privacyNoticeUrl, IEnumerable<Control> buttonControls)
            : base() {
            this.Title = title;
            var articleControl = new WebControl("article");
            if (!string.IsNullOrEmpty(title)) {
                articleControl.AddChildControl(new Label("h1", title));
            }
            if (!string.IsNullOrEmpty(text)) {
                articleControl.AddChildControl(new MultilineText("p", text));
            }
            foreach (var buttonControl in buttonControls) {
                articleControl.AddChildControl(buttonControl);
            }
            this.ContentSection.AddChildControl(articleControl);
            var footer = new WebControl("footer");
            if (!string.IsNullOrEmpty(imprintUrl)) {
                footer.AddChildControl(new Link(Resources.Imprint, imprintUrl));
            }
            if (!string.IsNullOrEmpty(privacyNoticeUrl)) {
                footer.AddChildControl(new Link(Resources.PrivacyNotice, privacyNoticeUrl));
            }
            if (!footer.IsEmpty) {
                this.ContentSection.AddChildControl(footer);
            }
        }

    }

}