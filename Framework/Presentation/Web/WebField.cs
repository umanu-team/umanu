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
    /// Field control.
    /// </summary>
    public abstract class WebField : Control {

        /// <summary>
        /// Render mode of field, e.g. for form or for list table.
        /// </summary>
        public FieldRenderMode RenderMode { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        private ViewField viewField;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        public WebField(ViewField viewField, FieldRenderMode renderMode)
            : base(FieldRenderMode.Form == renderMode ? "div" : FieldRenderMode.ListTable == renderMode ? "td" : null) {
            this.RenderMode = renderMode;
            this.viewField = viewField;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            if (FieldRenderMode.Form == this.RenderMode) {
                this.RenderDisplayTitle(html);
            }
            return;
        }

        /// <summary>
        /// Renders the HTML markup of display title.
        /// </summary>
        /// <param name="html">HTML output response</param>
        protected virtual void RenderDisplayTitle(HtmlWriter html) {
            html.AppendOpeningTag("label");
            if (!string.IsNullOrEmpty(this.viewField.Title)) {
                html.AppendHtmlEncoded(this.viewField.Title);
            }
            html.AppendClosingTag("label");
            return;
        }

    }

}