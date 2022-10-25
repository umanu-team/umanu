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

    using Forms;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Field control for a single lookup value.
    /// </summary>
    public abstract class WebFieldForLookup : WebFieldForElement {

        /// <summary>
        /// View field to build control for.
        /// </summary>
        private ViewFieldForLookup viewField;

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
        public WebFieldForLookup(IPresentableFieldForElement presentableField, ViewFieldForLookup viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.viewField = viewField;
        }

        /// <summary>
        /// Cleans a post-back value.
        /// </summary>
        /// <param name="value">post-back value to clean</param>
        /// <param name="presentableObject">presentable object to
        /// clean value for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// options</param>
        /// <returns>cleaned post-back value</returns>
        protected override string CleanPostBackValue(string value, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            return value?.Trim(); // base may not be called because multiple whitespaces must remain
        }

        /// <summary>
        /// Renders a control for editing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderEditableValue(HtmlWriter html) {
            var attributes = new Dictionary<string, string>(10);
            attributes.Add("id", this.ClientFieldId);
            attributes.Add("type", "text");
            if (this.viewField.IsAutofocused) {
                attributes.Add("autofocus", "autofocus");
            }
            attributes.Add("data-ajaxlist", this.ClientFieldId + ".json");
            if (this.viewField.GetIsFillInAllowed()) {
                attributes.Add("data-allowfillin", "1");
            }
            attributes.Add("data-min-search-length", this.viewField.MinSearchLength.ToString());
            attributes.Add("name", this.ClientFieldId);
            attributes.Add("placeholder", System.Web.HttpUtility.HtmlEncode(this.viewField.Placeholder));
            if (Mandatoriness.Required == this.viewField.Mandatoriness) {
                attributes.Add("required", "required");
            }
            if (!string.IsNullOrEmpty(this.EditableValue)) {
                attributes.Add("value", System.Web.HttpUtility.HtmlEncode(this.EditableValue));
            }
            html.AppendSelfClosingTag("input", attributes);
            return;
        }

    }

}