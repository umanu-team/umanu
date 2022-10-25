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

    /// <summary>
    /// Text label to be used on web pages.
    /// </summary>
    public class Label : Control {

        /// <summary>
        /// Text to be displayed on label.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tagName">HTML tag name of this control</param>
        public Label(string tagName)
            : base(tagName) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tagName">HTML tag name of control</param>
        /// <param name="text">text to be displayed on label</param>
        public Label(string tagName, string text)
            : this(tagName) {
            this.Text = text;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.AppendHtmlEncoded(this.Text);
            return;
        }

    }

}