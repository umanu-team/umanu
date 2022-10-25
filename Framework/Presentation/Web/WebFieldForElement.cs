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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Field control for single elements.
    /// </summary>
    public abstract class WebFieldForElement : WebFieldForEditableValue {

        /// <summary>
        /// Editable value of this form field, which is either the
        /// field value or the postback value. Be aware: This
        /// property is supposed to be set during
        /// "CreateChildControls" and will be set to the old field
        /// value in any earlier state.
        /// </summary>
        protected virtual string EditableValue {
            get {
                string value;
                if (PostBackState.ValidPostBack == this.PostBackState) {
                    value = this.PostBackValue;
                } else {
                    value = this.PresentableField.ValueAsString;
                }
                return value;
            }
        }

        /// <summary>
        /// Current postback value of this form field. Be aware: This
        /// property is supposed to be set during
        /// "CreateChildControls" and will be set to null in any
        /// earlier state.
        /// </summary>
        public string PostBackValue { get; protected set; }

        /// <summary>
        /// Value as on form load as string.
        /// </summary>
        protected override string PreviousValue {
            get {
                return this.previousValue;
            }
        }
        private readonly string previousValue;

        /// <summary>
        /// Presentable field to build control for.
        /// </summary>
        public IPresentableFieldForElement PresentableField { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        private readonly ViewFieldForElement viewFieldForElement;

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
        public WebFieldForElement(IPresentableFieldForElement presentableField, ViewFieldForElement viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.PresentableField = presentableField;
            this.previousValue = presentableField.ValueAsString;
            this.viewFieldForElement = viewField;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            this.ErrorMessage = null;
            this.IsIncludedInPostBack = false;
            if (FieldRenderMode.Form == this.RenderMode && PostBackState.ValidPostBack == this.PostBackState) {
                if (this.IsReadOnly) {
                    this.IsIncludedInPostBack = true;
                } else {
                    this.PostBackValue = httpRequest.Form[this.ClientFieldId];
                    if (null != this.PostBackValue) {
                        this.IsIncludedInPostBack = true;
                        this.PostBackValue = this.CleanPostBackValue(this.PostBackValue, this.TopmostParentPresentableObject, this.OptionDataProvider);
                        if (this.PresentableField.ValueAsString != this.PostBackValue) {
                            var hashedPreviousValue = httpRequest.Form[this.ClientFieldId + "::"];
                            if (WebFieldForElement.GetHashedValueFor(this.PostBackValue) == hashedPreviousValue) {
                                this.PostBackValue = this.PresentableField.ValueAsString;
                            } else {
                                if (!this.PresentableField.TrySetValueAsString(this.PostBackValue)) {
                                    this.ErrorMessage = this.viewFieldForElement.GetDefaultErrorMessage();
                                }
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            if (FieldRenderMode.ListTable == this.RenderMode) {
                string sortableValue = this.PresentableField.SortableValue;
                if (!string.IsNullOrEmpty(sortableValue)) {
                    sortableValue = System.Web.HttpUtility.HtmlEncode(sortableValue);
                    if (sortableValue != this.GetReadOnlyValue()) {
                        yield return new KeyValuePair<string, string>("data-value", sortableValue);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the comparative value of this field if a comparison
        /// date is set.
        /// </summary>
        /// <returns>comparative value of this field if comparison
        /// date is set, null otherwise</returns>
        protected string GetComparativeValue() {
            string comparativeValue;
            var comparativeField = this.PresentableField.GetVersionedField(this.ComparisonDate);
            if (null == comparativeField) {
                comparativeValue = null;
            } else {
                comparativeValue = this.viewFieldForElement.GetReadOnlyValueFor(comparativeField, this.TopmostParentPresentableObject, this.OptionDataProvider);
            }
            return comparativeValue;
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="currentValue">selected value</param>
        /// <returns>true if comparative value was rendered, false
        /// otherwise</returns>
        protected bool RenderComparativeValue(HtmlWriter html, string currentValue) {
            bool isDiffNew;
            if (this.ComparisonDate.HasValue) {
                string comparativeValue = this.GetComparativeValue();
                isDiffNew = string.IsNullOrEmpty(comparativeValue) || comparativeValue != currentValue;
                if (isDiffNew && !string.IsNullOrEmpty(comparativeValue)) {
                    html.AppendOpeningTag("span", "diffrm");
                    html.AppendHtmlEncoded(comparativeValue);
                    html.AppendClosingTag("span");
                    if (!string.IsNullOrEmpty(currentValue)) {
                        html.Append(' ');
                    }
                }
            } else {
                isDiffNew = false;
            }
            return isDiffNew;
        }

        /// <summary>
        /// Gets the read-only value of this form field.
        /// </summary>
        /// <returns>read-only value of this form field</returns>
        protected string GetReadOnlyValue() {
            return this.viewFieldForElement.GetReadOnlyValueFor(this.PresentableField, this.TopmostParentPresentableObject, this.OptionDataProvider);
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            string value = this.GetReadOnlyValue();
            bool isDiffNew = this.RenderComparativeValue(html, value);
            if (!string.IsNullOrEmpty(value)) {
                if (isDiffNew) {
                    html.AppendOpeningTag("span", "diffnew");
                }
                html.AppendHtmlEncoded(value);
                if (isDiffNew) {
                    html.AppendClosingTag("span");
                }
            }
            return;
        }

        /// <summary>
        /// Set validity state of post back values.
        /// </summary>
        protected internal sealed override void SetHasValidValue() {
            if (this.IsIncludedInPostBack && !this.IsReadOnly && string.IsNullOrEmpty(this.ErrorMessage)) {
                if (null == this.PresentableField.ValueAsObject && !string.IsNullOrEmpty(this.PostBackValue)) {
                    this.ErrorMessage = this.viewFieldForElement.GetDefaultErrorMessage(); // to makes sure that optional lookup fields behave correctly on unresolvable input
                } else {
                    this.ErrorMessage = this.viewFieldForElement.Validate(this.PresentableField, ValidityCheck.Transitional, this.TopmostParentPresentableObject, this.OptionDataProvider);
                }
            }
            return;
        }

    }

}