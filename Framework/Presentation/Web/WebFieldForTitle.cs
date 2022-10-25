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
    /// Field control for read-only title.
    /// </summary>
    public class WebFieldForTitle : WebField {

        /// <summary>
        /// Topmost presentable parent object to build form for.
        /// </summary>
        public IPresentableObject TopmostParentPresentableObject { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForTitle ViewField { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build list table row for</param>
        public WebFieldForTitle(ViewFieldForTitle viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject)
            : base(viewField, renderMode) {
            this.TopmostParentPresentableObject = topmostParentPresentableObject;
            this.ViewField = viewField;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            base.RenderChildControls(html);
            if (FieldRenderMode.Form == this.RenderMode) {
                html.AppendOpeningTag("p");
            }
            string value = this.ViewField.GetReadOnlyValueFor(null, this.TopmostParentPresentableObject, null);
            if (!string.IsNullOrEmpty(value)) {
                html.AppendHtmlEncoded(value);
            }
            if (FieldRenderMode.Form == this.RenderMode) {
                html.AppendClosingTag("p");
            }
            return;
        }

    }

}