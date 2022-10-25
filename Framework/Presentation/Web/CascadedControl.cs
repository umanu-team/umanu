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
    using System.Web;

    /// <summary>
    /// Control with child controls to be used on web pages.
    /// </summary>
    public abstract class CascadedControl : Control {

        /// <summary>
        /// Child controls of control.
        /// </summary>
        protected IList<Control> Controls { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tagName">HTML tag name of control</param>
        public CascadedControl(string tagName)
            : base(tagName) {
            this.Controls = new List<Control>();
        }

        /// <summary>
        /// Creates all child controls. This is called prior to
        /// HandleEvents().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(HttpRequest httpRequest) {
            base.CreateChildControls(httpRequest);
            foreach (var control in this.Controls) {
                control.CreateChildControls(httpRequest);
            }
            return;
        }

        /// <summary>
        /// Gets all child controls.
        /// </summary>
        /// <returns>enumerable of all child controls</returns>
        public IEnumerable<Control> GetChildControls() {
            return this.Controls;
        }

        /// <summary>
        /// Handles all server-side postback events. This is called
        /// after CreateChildControls(), but prior to Render().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        /// <param name="httpResponse">HTTP response for client</param>
        public override void HandleEvents(HttpRequest httpRequest, HttpResponse httpResponse) {
            base.HandleEvents(httpRequest, httpResponse);
            foreach (var control in this.Controls) {
                control.HandleEvents(httpRequest, httpResponse);
            }
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            base.RenderChildControls(html);
            foreach (var control in this.Controls) {
                control.Render(html);
            }
            return;
        }

    }

}
