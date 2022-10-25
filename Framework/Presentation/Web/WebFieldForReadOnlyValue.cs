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
    /// Control for read-only field.
    /// </summary>
    public class WebFieldForReadOnlyValue : WebField {

        /// <summary>
        /// Data provider to use for option providers.
        /// </summary>
        public IOptionDataProvider OptionDataProvider { get; private set; }

        /// <summary>
        /// Topmost presentable parent object to build form for.
        /// </summary>
        public IPresentableObject TopmostParentPresentableObject { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        private readonly ViewField viewField;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        public WebFieldForReadOnlyValue(ViewField viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider)
            : base(viewField, renderMode) {
            this.viewField = viewField;
            this.TopmostParentPresentableObject = topmostParentPresentableObject;
            this.OptionDataProvider = optionDataProvider;
        }

        /// <summary>
        /// Gets the read-only value of this form field.
        /// </summary>
        /// <returns>read-only value of this form field</returns>
        protected string GetReadOnlyValue() {
            return this.viewField.GetReadOnlyValueFor(null, this.TopmostParentPresentableObject, this.OptionDataProvider);
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
            string value = this.GetReadOnlyValue();
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