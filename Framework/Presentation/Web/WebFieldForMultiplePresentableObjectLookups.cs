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
    using System.Text;

    /// <summary>
    /// Field control for multiple lookup values of type presentable
    /// object.
    /// </summary>
    public class WebFieldForMultiplePresentableObjectLookups : WebFieldForMultipleLookups {

        /// <summary>
        /// View field to build control for.
        /// </summary>
        protected ViewFieldForMultiplePresentableObjectLookups ViewField { get; private set; }

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
        public WebFieldForMultiplePresentableObjectLookups(IPresentableFieldForCollection presentableField, ViewFieldForMultiplePresentableObjectLookups viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
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
        /// Cleans a post-back value.
        /// </summary>
        /// <param name="value">post-back value to clean</param>
        /// <param name="presentableObject">presentable object to
        /// clean value for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// options</param>
        /// <returns>cleaned post-back value</returns>
        protected override string CleanPostBackValue(string value, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string cleanedValue = base.CleanPostBackValue(value, presentableObject, optionDataProvider);
            return this.AutocompleteValue(cleanedValue, presentableObject, optionDataProvider);
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
                    if (this.SetPostBackValues(httpRequest.Form[this.ClientFieldId])) {
                        this.IsIncludedInPostBack = true;
                        var postBackObjects = new List<IPresentableObject>(this.PostBackValues.Count);
                        foreach (var postBackValue in this.PostBackValues) {
                            var postBackObject = this.ViewField.LookupProvider.FindKeyForValue(postBackValue, this.TopmostParentPresentableObject, this.OptionDataProvider);
                            postBackObjects.Add(postBackObject);
                        }
                        if (!this.FieldValuesAreEqualTo(postBackObjects)) {
                            var hashedPreviousValue = httpRequest.Form[this.ClientFieldId + "::"];
                            var postBackIdsBuilder = new StringBuilder();
                            foreach (var postbackObject in postBackObjects) {
                                postBackIdsBuilder.Append(postbackObject?.Id.ToString("N"));
                            }
                            if (WebFieldForElement.GetHashedValueFor(postBackIdsBuilder.ToString()) == hashedPreviousValue) {
                                this.PostBackValues.Clear();
                                this.PostBackValues.AddRange(WebFieldForMultiplePresentableObjectLookups.GetValuesForKeys(this.PresentableField, this.ViewField, this.TopmostParentPresentableObject, this.OptionDataProvider));
                            } else {
                                this.PresentableField.Clear();
                                foreach (var postBackObject in postBackObjects) {
                                    this.PresentableField.AddObject(postBackObject);
                                }
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Determines whether an enumerable of values as strings are
        /// equal to the current field values.
        /// </summary>
        /// <param name="values">values to compare with field values</param>
        /// <returns>true if values as string are equal to current
        /// field values, false otherwise</returns>
        private bool FieldValuesAreEqualTo(List<IPresentableObject> values) {
            bool valuesAreEqual = this.PresentableField.Count == values.Count;
            if (valuesAreEqual) {
                int i = 0;
                foreach (var presentableFieldValue in this.PresentableField.GetValuesAsObject()) {
                    if (values[i] != presentableFieldValue) {
                        valuesAreEqual = false;
                        break;
                    }
                    i++;
                }
            }
            return valuesAreEqual;
        }

        /// <summary>
        /// Gets the editable values. Be aware: This property is
        /// supposed to be set during "CreateChildControls" and will
        /// be set to the old field value in any earlier state.
        /// </summary>
        /// <returns>editable values</returns>
        protected override IEnumerable<string> GetEditableValues() {
            IEnumerable<string> editableValues;
            if (PostBackState.ValidPostBack == this.PostBackState) {
                editableValues = this.PostBackValues;
            } else {
                if (null == this.ViewField.LookupProvider) {
                    throw new ArgumentNullException(nameof(this.ViewField.LookupProvider), "Lookup provider of view field for key \"" + this.ViewField.Key + "\" must not be null.");
                }
                editableValues = WebFieldForMultiplePresentableObjectLookups.GetValuesForKeys(this.PresentableField, this.ViewField, this.TopmostParentPresentableObject, this.OptionDataProvider);
            }
            return editableValues;
        }

        /// <summary>
        /// Gets the values for the keys in a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to
        /// build control for</param>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>values for the keys in a presentable field</returns>
        private static List<string> GetValuesForKeys(IPresentableFieldForCollection presentableField, ViewFieldForMultiplePresentableObjectLookups viewField, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider) {
            var valuesForKeys = new List<string>();
            foreach (var valueAsObject in presentableField.GetValuesAsObject()) {
                valuesForKeys.Add(viewField.LookupProvider.FindValueForKey(valueAsObject as IPresentableObject, topmostParentPresentableObject, optionDataProvider));
            }
            return valuesForKeys;
        }

        /// <summary>
        /// Renders a read only paragraph showing the values.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="currentValues">list of selected values</param>
        /// <param name="comparativeValues">list of comparative
        /// values</param>
        protected override void RenderReadOnlyValue(HtmlWriter html, IList<string> currentValues, IList<string> comparativeValues) {
            // override of this method is necessary to get the presentable objects for rendering links
            bool isFirstValue = this.RenderComparativeValues(html, currentValues, comparativeValues);
            foreach (var key in this.PresentableField.GetValuesAsObject()) {
                var presentableObject = key as IPresentableObject;
                if (null != presentableObject) {
                    var currentValue = this.ViewField.LookupProvider.FindValueForKey(presentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
                    if (!string.IsNullOrEmpty(currentValue)) {
                        if (isFirstValue) {
                            isFirstValue = false;
                        } else {
                            html.Append(this.GetValueSeparator());
                        }
                        bool isDiffNew = null == comparativeValues || comparativeValues.Contains(currentValue);
                        if (!isDiffNew) {
                            html.AppendOpeningTag("span", "diffnew");
                        }
                        var href = this.ViewField.OnClickUrlDelegate?.Invoke(presentableObject);
                        if (!string.IsNullOrEmpty(href)) {
                            html.AppendOpeningTag("a", new KeyValuePair<string, string>[] {
                                new KeyValuePair<string, string>("href", href)
                            });
                        }
                        html.AppendHtmlEncoded(currentValue);
                        if (!string.IsNullOrEmpty(href)) {
                            html.AppendClosingTag("a");
                        }
                        if (!isDiffNew) {
                            html.AppendClosingTag("span");
                        }
                    }
                }
            }
            return;
        }

    }

}
