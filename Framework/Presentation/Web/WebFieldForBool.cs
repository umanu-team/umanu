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
    using Framework.Properties;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Field control for a boolean value.
    /// </summary>
    public class WebFieldForBool : WebFieldForElement {

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForBool ViewField { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableField">presentable field to
        /// build control for</param>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to ID of
        /// field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to ID of
        /// field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        public WebFieldForBool(IPresentableFieldForElement presentableField, ViewFieldForBool viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.ViewField = viewField;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            base.CreateChildControls(httpRequest);
            if (FieldRenderMode.Form == this.RenderMode && PostBackState.ValidPostBack == this.PostBackState && !this.IsIncludedInPostBack) {
                this.IsIncludedInPostBack = true;
                if (Mandatoriness.Required == this.ViewField.Mandatoriness) {
                    this.ErrorMessage = this.ViewField.GetDefaultErrorMessage();
                }
            }
            return;
        }

        /// <summary>
        /// Renders a control for editing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderEditableValue(HtmlWriter html) {
            if (null != this.EditableValue && bool.TryParse(this.EditableValue, out bool isChecked)) {
                this.RenderRadioButton(html, Resources.Yes, bool.TrueString, isChecked);
                this.RenderRadioButton(html, Resources.No, bool.FalseString, !isChecked);
            } else {
                this.RenderRadioButton(html, Resources.Yes, bool.TrueString, false);
                this.RenderRadioButton(html, Resources.No, bool.FalseString, false);
            }
            return;
        }

        /// <summary>
        /// Renders a radio button.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="title">display title of radio button</param>
        /// <param name="value">value of radio button</param>
        /// <param name="isChecked">true if radio button is checked,
        /// false otherwise</param>
        private void RenderRadioButton(HtmlWriter html, string title, string value, bool isChecked) {
            html.AppendOpeningTag("label", "radio");
            string id = this.ClientFieldId + "-" + value;
            var attributes = new Dictionary<string, string>(6);
            attributes.Add("id", id);
            attributes.Add("type", "radio");
            attributes.Add("name", this.ClientFieldId);
            if (this.ViewField.IsAutofocused) {
                attributes.Add("autofocus", "autofocus");
            }
            if (isChecked) {
                attributes.Add("checked", "checked");
            }
            attributes.Add("value", value);
            html.AppendSelfClosingTag("input", attributes);
            html.AppendHtmlEncoded(title);
            html.AppendClosingTag("label");
            return;
        }

    }

}