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

namespace Framework.BusinessApplications.Web {

    using Buttons;
    using Framework.Presentation.Web;
    using System.Collections.Generic;

    /// <summary>
    /// Control for button of action bar for cancelling forms.
    /// </summary>
    internal sealed class CancelWebButton : ClientSideWebButton {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="cancelButton">cancel button to display</param>
        public CancelWebButton(CancelButton cancelButton)
            : base(cancelButton, null) {
            // nothing to do
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            yield return new KeyValuePair<string, string>("class", "back");
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.AppendOpeningTag("span", "icon");
            html.Append("&#xE5C4;");
            html.AppendClosingTag("span");
            html.AppendOpeningTag("span", "label");
            html.AppendHtmlEncoded(this.ActionButton.Title);
            html.AppendClosingTag("span");
            return;
        }

    }

}