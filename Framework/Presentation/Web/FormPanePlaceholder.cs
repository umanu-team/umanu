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

namespace Framework.Presentation.Forms {

    using System.Web;
    using Framework.Presentation.Web;

    /// <summary>
    /// Placeholder for form pane section.
    /// </summary>
    public class FormPanePlaceholder : Control {

        /// <summary>
        /// Placeholder text to be shown.
        /// </summary>
        public string PlaceholderText { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="placeholderText">placeholder text to be
        /// shown</param>
        public FormPanePlaceholder(string placeholderText)
            : base("section") {
            this.PlaceholderText = placeholderText;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            if (!string.IsNullOrEmpty(this.PlaceholderText)) {
                html.Append(HttpUtility.HtmlEncode(this.PlaceholderText));
            }
            base.RenderChildControls(html);
            return;
        }

    }

}