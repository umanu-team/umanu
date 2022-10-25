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
    using System.Text;

    /// <summary>
    /// Field control for multiple elements.
    /// </summary>
    public abstract class WebFieldForCollection : WebFieldForEditableValue {

        /// <summary>
        /// Current postback values of this form field. Be aware:
        /// This property is supposed to be set during
        /// "CreateChildControls" and will be set to null in any
        /// earlier state.
        /// </summary>
        public List<string> PostBackValues { get; private set; }

        /// <summary>
        /// Presentable field to build control for.
        /// </summary>
        public IPresentableFieldForCollection PresentableField { get; private set; }

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
        /// View field to build control for.
        /// </summary>
        private ViewFieldForCollection viewFieldForCollection;

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
        public WebFieldForCollection(IPresentableFieldForCollection presentableField, ViewFieldForCollection viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.PostBackValues = new List<string>();
            this.PresentableField = presentableField;
            var stringValueBuilder = new StringBuilder();
            foreach (var stringValue in presentableField.GetValuesAsString()) {
                stringValueBuilder.Append(stringValue);
            }
            this.previousValue = stringValueBuilder.ToString();
            this.viewFieldForCollection = viewField;
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
                        if (!this.FieldValuesAreEqualTo(this.PostBackValues)) {
                            var hashedPreviousValue = httpRequest.Form[this.ClientFieldId + "::"];
                            var postBackValueBuilder = new StringBuilder();
                            foreach (var postBackValue in this.PostBackValues) {
                                postBackValueBuilder.Append(postBackValue);
                            }
                            if (WebFieldForElement.GetHashedValueFor(postBackValueBuilder.ToString()) == hashedPreviousValue) {
                                this.PostBackValues.Clear();
                                this.PostBackValues.AddRange(this.PresentableField.GetValuesAsString());
                            } else {
                                this.PresentableField.Clear();
                                foreach (string postBackValue in this.PostBackValues) {
                                    if (!this.PresentableField.TryAddString(postBackValue)) {
                                        this.ErrorMessage = this.viewFieldForCollection.GetDefaultErrorMessage();
                                    }
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
        protected bool FieldValuesAreEqualTo(IEnumerable<string> values) {
            bool valuesAreEqual = true;
            using (var newValuesEnumerator = values.GetEnumerator()) {
                using (var oldValuesEnumerator = this.PresentableField.GetValuesAsString().GetEnumerator()) {
                    while (valuesAreEqual && newValuesEnumerator.MoveNext()) {
                        if (oldValuesEnumerator.MoveNext()) {
                            valuesAreEqual = newValuesEnumerator.Current == oldValuesEnumerator.Current;
                        } else {
                            valuesAreEqual = false;
                        }
                    }
                    if (valuesAreEqual && (newValuesEnumerator.MoveNext() || oldValuesEnumerator.MoveNext())) {
                        valuesAreEqual = false;
                    }
                }
            }
            return valuesAreEqual;
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
                var fieldValues = this.PresentableField.GetSortableValues();
                if (null != fieldValues) {
                    var sortableValueBuilder = new StringBuilder();
                    foreach (string fieldValue in fieldValues) {
                        if (sortableValueBuilder.Length > 0) {
                            sortableValueBuilder.Append(',');
                        }
                        sortableValueBuilder.Append(System.Web.HttpUtility.HtmlEncode(fieldValue));
                    }
                    string sortableValue = sortableValueBuilder.ToString();
                    if (!string.IsNullOrEmpty(sortableValue)) {
                        yield return new KeyValuePair<string, string>("data-value", sortableValue);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the comparative values of this field if a comparison
        /// date is set.
        /// </summary>
        /// <returns>comparative values of this field if a comparison
        /// date is set, null otherwise</returns>
        protected IEnumerable<string> GetComparativeValues() {
            IEnumerable<string> comparativeValues;
            var comparativeField = this.PresentableField.GetVersionedField(this.ComparisonDate);
            if (null == comparativeField) {
                comparativeValues = null;
            } else {
                comparativeValues = this.viewFieldForCollection.GetReadOnlyValuesFor(comparativeField, this.TopmostParentPresentableObject, this.OptionDataProvider);
            }
            return comparativeValues;
        }

        /// <summary>
        /// Gets the editable values. Be aware: This property is
        /// supposed to be set during "CreateChildControls" and will
        /// be set to the old field value in any earlier state.
        /// </summary>
        /// <returns>editable values</returns>
        protected virtual IEnumerable<string> GetEditableValues() {
            IEnumerable<string> editableValues;
            if (PostBackState.ValidPostBack == this.PostBackState) {
                editableValues = this.PostBackValues;
            } else {
                editableValues = this.PresentableField.GetValuesAsString();
            }
            return editableValues;
        }

        /// <summary>
        /// Gets the read-only values.
        /// </summary>
        /// <returns>read-only values</returns>
        protected IEnumerable<string> GetReadOnlyValues() {
            return this.viewFieldForCollection.GetReadOnlyValuesFor(this.PresentableField, this.TopmostParentPresentableObject, this.OptionDataProvider);
        }

        /// <summary>
        /// Gets a value separator as string.
        /// </summary>
        /// <returns>value separator as string</returns>
        protected string GetValueSeparator() {
            return WebFieldForCollection.GetValueSeparator(this.viewFieldForCollection.GetValueSeparator(this.RenderMode));
        }

        /// <summary>
        /// Gets a value separator as string.
        /// </summary>
        /// <param name="valueSeparator">type of value separator to
        /// get</param>
        /// <returns>value separator as string</returns>
        internal static string GetValueSeparator(ValueSeparator valueSeparator) {
            string html = null;
            if (ValueSeparator.None == valueSeparator) {
                // nothing to do
            } else if (ValueSeparator.Comma == valueSeparator) {
                html = ", ";
            } else if (ValueSeparator.Semicolon == valueSeparator) {
                html = "; ";
            } else if (ValueSeparator.Space == valueSeparator) {
                html = " ";
            } else if (ValueSeparator.LineBreak == valueSeparator) {
                html = "<br />";
            } else {
                throw new PresentationException("Value seperator \"" + valueSeparator + "\" is unknown.");
            }
            return html;
        }

        /// <summary>
        /// Renders a read only paragraph showing comparative values.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="currentValues">list of selected values</param>
        /// <param name="comparativeValues">list of comparative
        /// values</param>
        /// <returns>true if comparative values were rendered, false
        /// otherwise</returns>
        protected bool RenderComparativeValues(HtmlWriter html, IList<string> currentValues, IList<string> comparativeValues) {
            bool isFirstValue = true;
            if (null != comparativeValues) {
                foreach (string comparativeValue in comparativeValues) {
                    if (!string.IsNullOrEmpty(comparativeValue)) {
                        if (!currentValues.Contains(comparativeValue)) {
                            if (isFirstValue) {
                                isFirstValue = false;
                            } else {
                                html.Append(this.GetValueSeparator());
                            }
                            html.AppendOpeningTag("span", "diffrm");
                            html.AppendHtmlEncoded(comparativeValue);
                            html.AppendClosingTag("span");
                        }
                    }
                }
            }
            return isFirstValue;
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            var currentValues = new List<string>(this.GetReadOnlyValues());
            List<string> comparativeValues;
            var comparativeValueEnumerable = this.GetComparativeValues();
            if (null == comparativeValueEnumerable) {
                comparativeValues = null;
            } else {
                comparativeValues = new List<string>(comparativeValueEnumerable);
            }
            this.RenderReadOnlyValue(html, currentValues, comparativeValues);
            return;
        }

        /// <summary>
        /// Renders a read only paragraph showing the values.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="currentValues">list of selected values</param>
        /// <param name="comparativeValues">list of comparative
        /// values</param>
        protected virtual void RenderReadOnlyValue(HtmlWriter html, IList<string> currentValues, IList<string> comparativeValues) {
            bool isFirstValue = this.RenderComparativeValues(html, currentValues, comparativeValues);
            foreach (string currentValue in currentValues) {
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
                    html.AppendHtmlEncoded(currentValue);
                    if (!isDiffNew) {
                        html.AppendClosingTag("span");
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Sets the list of post back values.
        /// </summary>
        /// <param name="postBackValue">comma separated string of
        /// post back values to set</param>
        /// <returns>false if comma separated string of post back
        /// values is null, true otherwise</returns>
        protected bool SetPostBackValues(string postBackValue) {
            this.PostBackValues.Clear();
            if (null != postBackValue) {
                string[] html;
                var valueSeparator = this.viewFieldForCollection.GetValueSeparator(this.RenderMode);
                if (ValueSeparator.Comma == valueSeparator) {
                    html = new string[] { "," };
                } else if (ValueSeparator.Semicolon == valueSeparator) {
                    html = new string[] { ";" };
                } else if (ValueSeparator.Space == valueSeparator) {
                    html = new string[] { " " };
                } else if (ValueSeparator.LineBreak == valueSeparator) {
                    html = new string[] { "\n", "\r\n" };
                } else {
                    throw new PresentationException("Value seperator \"" + valueSeparator + "\" is unknown.");
                }
                string[] values = postBackValue.Split(html, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string value in values) {
                    string cleanedValue = this.CleanPostBackValue(value, this.TopmostParentPresentableObject, this.OptionDataProvider);
                    if (!string.IsNullOrEmpty(cleanedValue)) {
                        this.PostBackValues.Add(cleanedValue);
                    }
                }
            }
            return null != postBackValue;
        }

        /// <summary>
        /// Set validity state of post back values.
        /// </summary>
        protected internal sealed override void SetHasValidValue() {
            if (this.IsIncludedInPostBack && !this.IsReadOnly && string.IsNullOrEmpty(this.ErrorMessage)) {
                // the foreach loop makes sure that optional lookup fields behave correctly on unresolvable input
                int i = 0;
                foreach (var valueAsObject in this.PresentableField.GetValuesAsObject()) {
                    if (null == valueAsObject && !string.IsNullOrEmpty(this.PostBackValues[i])) {
                        this.ErrorMessage = this.viewFieldForCollection.GetDefaultErrorMessage();
                        break;
                    }
                    i++;
                }
                if (string.IsNullOrEmpty(this.ErrorMessage)) {
                    this.ErrorMessage = this.viewFieldForCollection.Validate(this.PresentableField, ValidityCheck.Transitional, this.TopmostParentPresentableObject, this.OptionDataProvider);
                }
            }
            return;
        }

    }

}