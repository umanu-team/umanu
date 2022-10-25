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

    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Model.Units;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Field control for dimensions.
    /// </summary>
    public sealed class WebFieldForDimensions : WebFieldForObject {

        /// <summary>
        /// Editable height value of this form field, which is either
        /// the field value or the postback value. Be aware: This
        /// property is supposed to be set during
        /// "CreateChildControls" and will be set to the old field
        /// value in any earlier state.
        /// </summary>
        private string EditableHeight {
            get {
                string editableHeight;
                if (PostBackState.ValidPostBack == this.PostBackState) {
                    editableHeight = this.postBackHeight;
                } else {
                    if (true == this.Value?.Height.HasValue) {
                        editableHeight = Math.Round(this.Value.Height.Value, this.ViewField.DecimalPlaces).ToString(CultureInfo.InvariantCulture);
                    } else {
                        editableHeight = string.Empty;
                    }
                }
                return editableHeight;
            }
        }

        /// <summary>
        /// Editable length value of this form field, which is either
        /// the field value or the postback value. Be aware: This
        /// property is supposed to be set during
        /// "CreateChildControls" and will be set to the old field
        /// value in any earlier state.
        /// </summary>
        private string EditableLength {
            get {
                string editableLength;
                if (PostBackState.ValidPostBack == this.PostBackState) {
                    editableLength = this.postBackLength;
                } else {
                    if (true == this.Value?.Length.HasValue) {
                        editableLength = Math.Round(this.Value.Length.Value, this.ViewField.DecimalPlaces).ToString(CultureInfo.InvariantCulture);
                    } else {
                        editableLength = string.Empty;
                    }
                }
                return editableLength;
            }
        }

        /// <summary>
        /// Editable width value of this form field, which is either
        /// the field value or the postback value. Be aware: This
        /// property is supposed to be set during
        /// "CreateChildControls" and will be set to the old field
        /// value in any earlier state.
        /// </summary>
        private string EditableWidth {
            get {
                string editableWidth;
                if (PostBackState.ValidPostBack == this.PostBackState) {
                    editableWidth = this.postBackWidth;
                } else {
                    if (true == this.Value?.Width.HasValue) {
                        editableWidth = Math.Round(this.Value.Width.Value, this.ViewField.DecimalPlaces).ToString(CultureInfo.InvariantCulture);
                    } else {
                        editableWidth = string.Empty;
                    }
                }
                return editableWidth;
            }
        }

        /// <summary>
        /// Current postback height value of this form field. Be
        /// aware: This property is supposed to be set during
        /// "CreateChildControls" and will be set to null in any
        /// earlier state.
        /// </summary>
        private string postBackHeight;

        /// <summary>
        /// Current postback length value of this form field. Be
        /// aware: This property is supposed to be set during
        /// "CreateChildControls" and will be set to null in any
        /// earlier state.
        /// </summary>
        private string postBackLength;

        /// <summary>
        /// Current postback width value of this form field. Be
        /// aware: This property is supposed to be set during
        /// "CreateChildControls" and will be set to null in any
        /// earlier state.
        /// </summary>
        private string postBackWidth;

        /// <summary>
        /// Value as on form load as string.
        /// </summary>
        protected override string PreviousValue {
            get {
                return this.previousValue;
            }
        }
        private string previousValue;

        /// <summary>
        /// Current value of this field.
        /// </summary>
        public Dimensions Value { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForDimensions ViewField { get; private set; }

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
        public WebFieldForDimensions(IPresentableFieldForElement presentableField, ViewFieldForDimensions viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.Value = presentableField.ValueAsObject as Dimensions;
            this.previousValue = this.Value.LengthAsString + ' ' + this.Value.WidthAsString + ' ' + this.Value.HeightAsString;
            this.ViewField = viewField;
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
                    this.postBackLength = httpRequest.Form[this.ClientFieldId];
                    this.postBackWidth = httpRequest.Form["W_" + this.ClientFieldId];
                    this.postBackHeight = httpRequest.Form["H_" + this.ClientFieldId];
                    if (null != this.postBackLength && null != this.postBackWidth && null != this.postBackHeight) {
                        this.IsIncludedInPostBack = true;
                        this.postBackLength = WebFieldForEditableValue.RemoveUnnecessaryWhiteSpace(this.postBackLength);
                        this.postBackWidth = WebFieldForEditableValue.RemoveUnnecessaryWhiteSpace(this.postBackWidth);
                        this.postBackHeight = WebFieldForEditableValue.RemoveUnnecessaryWhiteSpace(this.postBackHeight);
                        if (this.Value.LengthAsString != this.postBackLength || this.Value.WidthAsString != this.postBackWidth || this.Value.HeightAsString != this.postBackHeight) {
                            var hashedPreviousValue = httpRequest.Form[this.ClientFieldId + "::"];
                            if (WebFieldForElement.GetHashedValueFor(this.postBackLength + ' ' + this.postBackWidth + ' ' + this.postBackHeight) == hashedPreviousValue) {
                                this.postBackLength = this.Value.LengthAsString;
                                this.postBackWidth = this.Value.WidthAsString;
                                this.postBackHeight = this.Value.HeightAsString;
                            } else {
                                if (!this.Value.TrySetValueAsString(this.postBackLength, this.postBackWidth, this.postBackHeight)) {
                                    this.ErrorMessage = this.ViewField.GetDefaultErrorMessage();
                                }
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Renders a control for editing the end customer units per
        /// shipping unit.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderEditableValue(HtmlWriter html) {
            this.RenderEditableValueForField(html, this.ClientFieldId, this.ViewField.IsAutofocused, this.EditableLength);
            html.AppendHtmlEncoded("x ");
            this.RenderEditableValueForField(html, "W_" + this.ClientFieldId, false, this.EditableWidth);
            html.AppendHtmlEncoded("x ");
            this.RenderEditableValueForField(html, "H_" + this.ClientFieldId, false, this.EditableHeight);
            if (!string.IsNullOrEmpty(this.ViewField.Unit)) {
                html.AppendHtmlEncoded(this.ViewField.Unit);
                html.AppendHtmlEncoded(" ");
            }
            html.AppendHtmlEncoded(Resources.LengthXWidthXHeightAbbr);
            return;
        }

        /// <summary>
        /// Renders a control for editing the value of a field.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="id">id of field</param>
        /// <param name="isAutofocused">true if field is supposed to
        /// be autofocused, false otherwise</param>
        /// <param name="editableValue">editable value</param>
        private void RenderEditableValueForField(HtmlWriter html, string id, bool isAutofocused, string editableValue) {
            var attributes = new Dictionary<string, string>(10);
            attributes.Add("id", id);
            attributes.Add("type", "number");
            attributes.Add("name", id);
            if (isAutofocused) {
                attributes.Add("autofocus", "autofocus");
            }
            if (Mandatoriness.Required == this.ViewField.Mandatoriness) {
                attributes.Add("required", "required");
            }
            attributes.Add("max", this.ViewField.MaxValue.ToString(CultureInfo.InvariantCulture.NumberFormat));
            attributes.Add("min", this.ViewField.MinValue.ToString(CultureInfo.InvariantCulture.NumberFormat));
            IDictionary<string, string> options;
            if (null == this.ViewField.OptionProvider) {
                options = new Dictionary<string, string>(0);
            } else {
                options = this.ViewField.OptionProvider.GetOptionDictionary(this.ParentPresentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
            }
            if (options.Count > 0) {
                attributes.Add("list", "o" + id);
            }
            if (this.ViewField.Step > 0) {
                attributes.Add("step", this.ViewField.Step.ToString(CultureInfo.InvariantCulture.NumberFormat));
            }
            if (!string.IsNullOrEmpty(editableValue)) {
                attributes.Add("value", System.Web.HttpUtility.HtmlEncode(editableValue));
            }
            html.AppendSelfClosingTag("input", attributes);
            this.RenderDataList(html, "o" + id, options);
            return;
        }

    }

}