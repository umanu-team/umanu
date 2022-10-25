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

    using Framework.Presentation.Exceptions;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Field control for editable values.
    /// </summary>
    public abstract class WebFieldForEditableValue : WebField {

        /// <summary>
        /// Client ID to use for field.
        /// </summary>
        public string ClientFieldId { get; private set; }

        /// <summary>
        /// Point in time to compare form data of read-only fields
        /// to or null to not compare any data.
        /// </summary>
        public DateTime? ComparisonDate { get; private set; }

        /// <summary>
        /// CSS class to use for field descriptions.
        /// </summary>
        public string CssClassForDescription { get; set; }

        /// <summary>
        /// CSS class to use for asterisk symbol of desired fields.
        /// </summary>
        public string CssClassForDesiredValueSymbol { get; set; }

        /// <summary>
        /// CSS class to use for field error messages.
        /// </summary>
        public string CssClassForErrorMessage { get; set; }

        /// <summary>
        /// CSS class to use for asterisk symbol of required fields.
        /// </summary>
        public string CssClassForRequiredValueSymbol { get; set; }

        /// <summary>
        /// Error message to dispay if the value of this field is
        /// invalid.
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// Indicates whether at least one value for this field was
        /// included in post-back.
        /// </summary>
        public bool IsIncludedInPostBack { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this field is read-only.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Data provider to use for option providers.
        /// </summary>
        public IOptionDataProvider OptionDataProvider { get; private set; }

        /// <summary>
        /// Post back state of the parent form.
        /// </summary>
        public PostBackState PostBackState { get; private set; }

        /// <summary>
        /// Direct presentable parent object to build form for.
        /// </summary>
        public IPresentableObject ParentPresentableObject { get; private set; }

        /// <summary>
        /// Value as on form load as string.
        /// </summary>
        protected abstract string PreviousValue { get; }

        /// <summary>
        /// Topmost presentable parent object to build form for.
        /// </summary>
        public IPresentableObject TopmostParentPresentableObject { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        private readonly ViewFieldForEditableValue viewField;

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
        public WebFieldForEditableValue(IPresentableField presentableField, ViewFieldForEditableValue viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(viewField, renderMode) {
            this.ClientFieldId = clientFieldIdPrefix;
            if (!string.IsNullOrEmpty(this.ClientFieldId)) {
                this.ClientFieldId += '.';
            }
            this.ClientFieldId += viewField.Key + clientFieldIdSuffix;
            this.ComparisonDate = comparisonDate;
            if (FieldRenderMode.Form == this.RenderMode) {
                this.CssClassForDesiredValueSymbol = "desired";
                this.CssClassForRequiredValueSymbol = "required";
                this.CssClassForErrorMessage = "fielderror";
                this.CssClassForDescription = "fielddescription";
            }
            this.IsIncludedInPostBack = false;
            this.IsReadOnly = FieldRenderMode.ListTable == renderMode || viewField.IsReadOnly || presentableField.IsReadOnly; // order is important for performance
            this.OptionDataProvider = optionDataProvider;
            this.PostBackState = postBackState;
            this.ParentPresentableObject = presentableField.ParentPresentableObject;
            this.TopmostParentPresentableObject = topmostParentPresentableObject;
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
        protected virtual string CleanPostBackValue(string value, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            return WebFieldForEditableValue.RemoveUnnecessaryWhiteSpace(value);
        }

        /// <summary>
        /// Gets the hashed value for a value.
        /// </summary>
        /// <param name="value">value to get hashed value for</param>
        /// <returns>hashed value for value</returns>
        protected static string GetHashedValueFor(string value) {
            string hashedValue;
            if (null == value) {
                hashedValue = null;
            } else {
                hashedValue = value.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }
            return hashedValue;
        }

        /// <summary>
        /// Removes unnecessary white space in a post-back value.
        /// </summary>
        /// <param name="value">post-back value to clean</param>
        /// <returns>cleaned post-back value</returns>
        public static string RemoveUnnecessaryWhiteSpace(string value) {
            var cleanedValue = value.Trim();
            cleanedValue = Regex.ForMultipleSpaces.Replace(cleanedValue, " ");
            return cleanedValue;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            base.RenderChildControls(html);
            if (FieldRenderMode.ListTable == this.RenderMode) {
                this.RenderReadOnlyValue(html);
            } else if (FieldRenderMode.Form == this.RenderMode) {
                if (this.IsReadOnly) {
                    html.AppendOpeningTag("p");
                    this.RenderReadOnlyValue(html);
                    var descriptionForViewMode = this.viewField.DescriptionForViewMode;
                    if (!string.IsNullOrEmpty(descriptionForViewMode)) {
                        html.AppendSelfClosingTag("br");
                        html.AppendHtmlEncoded(descriptionForViewMode);
                    }
                    html.AppendClosingTag("p");
                } else {
                    html.AppendOpeningTag("div");
                    this.RenderEditableValue(html);
                    this.RenderHashedValue(html);
                    if (!string.IsNullOrEmpty(this.ErrorMessage)) {
                        html.AppendOpeningTag("span", this.CssClassForErrorMessage);
                        html.AppendHtmlEncoded(this.ErrorMessage);
                        html.AppendClosingTag("span");
                    }
                    var descriptionForEditMode = this.viewField.DescriptionForEditMode;
                    if (!string.IsNullOrEmpty(descriptionForEditMode)) {
                        html.AppendOpeningTag("span", this.CssClassForDescription);
                        html.AppendHtmlEncoded(descriptionForEditMode);
                        html.AppendClosingTag("span");
                    }
                    html.AppendClosingTag("div");
                }
            }
            return;
        }

        /// <summary>
        /// Renders a data list providing a list of options if list
        /// of options is not empty.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="clientId">client-side ID of data list</param>
        /// <param name="options">list of options</param>
        protected void RenderDataList(HtmlWriter html, string clientId, IEnumerable<KeyValuePair<string, string>> options) {
            var isOpeningTagRendered = false;
            foreach (var option in options) {
                if (!isOpeningTagRendered) {
                    var datalistAttributes = new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("id", clientId)
                    };
                    html.AppendOpeningTag("datalist", datalistAttributes);
                    isOpeningTagRendered = true;
                }
                var attributes = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("value", System.Web.HttpUtility.HtmlEncode(option.Key))
                };
                if (option.Key == option.Value) {
                    html.AppendSelfClosingTag("option", attributes);
                } else {
                    html.AppendOpeningTag("option", attributes);
                    html.AppendHtmlEncoded(option.Value);
                    html.AppendClosingTag("option");
                }
            }
            if (isOpeningTagRendered) {
                html.AppendClosingTag("datalist");
            }
            return;
        }

        /// <summary>
        /// Renders the HTML markup of display title and description.
        /// </summary>
        /// <param name="html">HTML output response</param>
        protected override void RenderDisplayTitle(HtmlWriter html) {
            var attributes = new Dictionary<string, string>(1);
            if (!this.IsReadOnly) {
                attributes.Add("for", this.ClientFieldId);
            }
            html.AppendOpeningTag("label", attributes);
            if (!string.IsNullOrEmpty(this.viewField.Title)) {
                html.AppendHtmlEncoded(this.viewField.Title);
            }
            if (!this.IsReadOnly && Mandatoriness.Optional != this.viewField.Mandatoriness) {
                string cssClass;
                if (Mandatoriness.Desired == this.viewField.Mandatoriness) {
                    cssClass = this.CssClassForDesiredValueSymbol;
                } else if (Mandatoriness.Required == this.viewField.Mandatoriness) {
                    cssClass = this.CssClassForRequiredValueSymbol;
                } else {
                    throw new PresentationException("Mandatoriness \"" + this.viewField.Mandatoriness + "\" is invalid.");
                }
                html.AppendOpeningTag("span", cssClass);
                html.Append('*');
                html.AppendClosingTag("span");
            }
            html.AppendClosingTag("label");
            return;
        }

        /// <summary>
        /// Renders a control for editing the value(s).
        /// </summary>
        /// <param name="html">HTML response</param>
        protected abstract void RenderEditableValue(HtmlWriter html);

        /// <summary>
        /// Renders a hidden field with hashed previous value(s).
        /// </summary>
        /// <param name="html">HTML response</param>
        protected void RenderHashedValue(HtmlWriter html) {
            var hashedValue = WebFieldForEditableValue.GetHashedValueFor(this.PreviousValue);
            if (!string.IsNullOrEmpty(hashedValue)) {
                html.AppendHiddenInputTag(this.ClientFieldId + "::", hashedValue);
            }
            return;
        }

        /// <summary>
        /// Renders a read-only paragraph showing the value(s).
        /// </summary>
        /// <param name="html">HTML response</param>
        protected abstract void RenderReadOnlyValue(HtmlWriter html);

        /// <summary>
        /// Set validity state of post back values.
        /// </summary>
        protected internal abstract void SetHasValidValue();

    }

}