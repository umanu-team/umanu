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

    using System.Collections.Generic;

    /// <summary>
    /// Represents a hyperlink.
    /// </summary>
    public sealed class Link : Control {

        /// <summary>
        /// Target URL to call on hyperlink click.
        /// </summary>
        public string TargetUrl { get; private set; }

        /// <summary>
        /// Display title of hyperlink.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of hyperlink</param>
        /// <param name="targetUrl">target URL to call on hyperlink
        /// click</param>
        public Link(string title, string targetUrl)
            : base("a") {
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
            yield return new KeyValuePair<string, string>("href", this.TargetUrl);
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.AppendHtmlEncoded(Title);
            return;
        }

    }

}