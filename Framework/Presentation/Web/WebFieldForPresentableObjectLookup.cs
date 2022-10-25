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
    using System.Web;

    /// <summary>
    /// Field control for a single lookup value of type presentable
    /// object.
    /// </summary>
    public class WebFieldForPresentableObjectLookup : WebFieldForLookup {

        /// <summary>
        /// Editable value of this form field, which is either the
        /// field value or the postback value. Be aware: This
        /// property is supposed to be set during
        /// "CreateChildControls" and will be set to the old field
        /// value in any earlier state.
        /// </summary>
        protected override string EditableValue {
            get {
                string value;
                if (PostBackState.ValidPostBack == this.PostBackState) {
                    value = this.PostBackValue;
                } else {
                    if (null == this.ViewField.LookupProvider) {
                        throw new InvalidOperationException("Lookup provider of view field for key \"" + this.ViewField.Key + "\" must not be null.");
                    }
                    value = this.ViewField.LookupProvider.FindValueForKey(this.PresentableField.ValueAsObject as IPresentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
                }
                return value;
            }
        }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForPresentableObjectLookup ViewField { get; private set; }

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
        public WebFieldForPresentableObjectLookup(IPresentableFieldForElement presentableField, ViewFieldForPresentableObjectLookup viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.ViewField = viewField;
        }

        /// <summary>
        /// Autocompletes a post-back value.
        /// </summary>
        /// <param name="value">post-back value to autocomplete</param>
        /// <param name="presentableObject">presentable object to
        /// autocomplete value for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// options</param>
        /// <returns>autocompleteed post-back value</returns>
        private string AutocompleteValue(string value, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string uniqueValue = this.ViewField.LookupProvider.FindUniqueValueByVagueTerm(value, presentableObject, optionDataProvider);
            if (string.IsNullOrEmpty(uniqueValue)) {
                uniqueValue = value;
            }
            return uniqueValue;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(HttpRequest httpRequest) {
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
                        var fieldStringValue = this.ViewField.LookupProvider.FindValueForKey(this.PresentableField.ValueAsObject as IPresentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
                        if (this.PostBackValue != fieldStringValue) { // this condition is for performance optimization only - usually FindValueForKey(...) is faster than FindUniqueValueByVagueTerm(...) that is called in AutocompleteValue(...)
                            this.PostBackValue = this.AutocompleteValue(this.PostBackValue, this.TopmostParentPresentableObject, this.OptionDataProvider);
                        }
                        var postBackObject = this.ViewField.LookupProvider.FindKeyForValue(this.PostBackValue, this.TopmostParentPresentableObject, this.OptionDataProvider);
                        if (this.PresentableField.ValueAsObject != postBackObject) {
                            var hashedPreviousValue = httpRequest.Form[this.ClientFieldId + "::"];
                            if (WebFieldForElement.GetHashedValueFor(postBackObject?.Id.ToString("N")) == hashedPreviousValue) {
                                this.PostBackValue = fieldStringValue;
                            } else {
                                this.PresentableField.ValueAsObject = postBackObject;
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            // override of this method is necessary to get the presentable object for rendering link
            string value = this.GetReadOnlyValue();
            bool isDiffNew = this.RenderComparativeValue(html, value);
            if (!string.IsNullOrEmpty(value)) {
                if (isDiffNew) {
                    html.AppendOpeningTag("span", "diffnew");
                }
                var presentableObject = this.PresentableField.ValueAsObject as IPresentableObject;
                var href = this.ViewField.OnClickUrlDelegate?.Invoke(presentableObject);
                if (!string.IsNullOrEmpty(href)) {
                    html.AppendOpeningTag("a", new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("href", href)
                    });
                }
                html.AppendHtmlEncoded(value);
                if (!string.IsNullOrEmpty(href)) {
                    html.AppendClosingTag("a");
                }
                if (isDiffNew) {
                    html.AppendClosingTag("span");
                }
            }
            return;
        }

    }

}