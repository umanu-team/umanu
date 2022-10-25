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

    using Framework.Presentation.Forms;

    /// <summary>
    /// Control for master view of presentable objects for
    /// master/detail views.
    /// </summary>
    public abstract class MasterControl : CascadedControl {

        /// <summary>
        /// CSS class to use for form description pane.
        /// </summary>
        public string CssClassForDescriptionPane { get; set; }

        /// <summary>
        /// Delegate for resolval of on click URL for a presentable
        /// object.
        /// </summary>
        public OnClickUrlDelegate OnClickUrlDelegate { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tagName">HTML tag name of control</param>
        public MasterControl(string tagName)
            : base(tagName) {
            // nothing to do
        }

        /// <summary>
        /// Renders a description message if applicable.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="description">description text to be rendered</param>
        protected void RenderDescriptionMessage(HtmlWriter html, string description) {
            if (!string.IsNullOrEmpty(description)) {
                var infoPane = new InfoPane(description);
                infoPane.CssClasses.Add(this.CssClassForDescriptionPane);
                infoPane.Render(html);
            }
            return;
        }

    }

}
