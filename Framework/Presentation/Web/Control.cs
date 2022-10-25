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

    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Control to be used on web pages.
    /// </summary>
    public abstract class Control {

        /// <summary>
        /// Cascading Style Sheet (CSS) classes to apply.
        /// </summary>
        public IList<string> CssClasses { get; private set; }

        /// <summary>
        /// HTML tag name of control.
        /// </summary>
        public string TagName { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tagName">HTML tag name of control</param>
        public Control(string tagName) {
            this.CssClasses = new List<string>();
            if (string.IsNullOrEmpty(tagName)) {
                throw new ArgumentException("Tag may not be null or empty.", nameof(tagName));
            }
            this.TagName = tagName;
        }

        /// <summary>
        /// Creates all child controls. This is called prior to
        /// HandleEvents().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public virtual void CreateChildControls(HttpRequest httpRequest) {
            return;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            yield return new KeyValuePair<string, string>("class", string.Join(" ", this.CssClasses));
        }

        /// <summary>
        /// Gets a value indicating whether control is supposed to be
        /// rendered.
        /// </summary>
        /// <returns>true if control is supposed to be rendered,
        /// false otherwise</returns>
        protected virtual bool GetIsVisible() {
            return true;
        }

        /// <summary>
        /// Handles all server-side postback events. This is called
        /// after CreateChildControls(), but prior to Render().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        /// <param name="httpResponse">HTTP response for client</param>
        public virtual void HandleEvents(HttpRequest httpRequest, HttpResponse httpResponse) {
            return;
        }

        /// <summary>
        /// Renders the HTML markup of control. This is called after
        /// HandleEvents().
        /// </summary>
        /// <param name="html">HTML response</param>
        public void Render(HtmlWriter html) {
            if (this.GetIsVisible()) {
                html.AppendOpeningTag(this.TagName, this.GetAttributes());
                this.RenderChildControls(html);
                html.AppendClosingTag(this.TagName);
            }
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected virtual void RenderChildControls(HtmlWriter html) {
            return;
        }

    }

}