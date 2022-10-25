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

    using System.Collections.Generic;

    /// <summary>
    /// Control for rendering link buttons.
    /// </summary>
    internal sealed class LinkButton : Control {

        /// <summary>
        /// Target URL of link.
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// Display caption of button.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        ///<param name="targetUrl">target URL of link</param>
        public LinkButton(string title, string targetUrl)
            : base("button") {
            this.TargetUrl = targetUrl;
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
            yield return new KeyValuePair<string, string>("onclick", "javascript:document.location='" + this.TargetUrl + "';");
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