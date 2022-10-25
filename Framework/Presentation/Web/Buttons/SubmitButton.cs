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

namespace Framework.Presentation.Web.Buttons {

    using Framework.Presentation.Web;
    using System.Collections.Generic;

    /// <summary>
    /// Control for rendering submit buttons.
    /// </summary>
    internal sealed class SubmitButton : Control {

        /// <summary>
        /// Display caption of button.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        public SubmitButton(string title)
            : base("button") {
            this.Title = title;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            yield return new KeyValuePair<string, string>("onclick", "javascript:this.disabled=true;document.forms[document.forms.length-1].submit();");
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.AppendHtmlEncoded(this.Title);
            return;
        }

    }

}