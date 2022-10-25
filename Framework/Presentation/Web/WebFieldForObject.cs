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
    using Persistence;
    using System;

    /// <summary>
    /// Base class of field controls for combination of values.
    /// </summary>
    public abstract class WebFieldForObject : WebFieldForEditableValue {

        /// <summary>
        /// Lock for this object.
        /// </summary>
        private readonly object thisLock = new object();

        /// <summary>
        /// Presentable field to build control for.
        /// </summary>
        private readonly IPresentableFieldForElement presentableField;

        /// <summary>
        /// View field to build control for.
        /// </summary>
        private readonly ViewFieldForElement viewField;

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
        public WebFieldForObject(IPresentableFieldForElement presentableField, ViewFieldForElement viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.presentableField = presentableField;
            this.viewField = viewField;
        }

        /// <summary>
        /// Gets the comparative value of this field if a comparison
        /// date is set.
        /// </summary>
        /// <returns>comparative value of this field if comparison
        /// date is set, null otherwise</returns>
        protected string GetComparativeValue() {
            string comparativeValue;
            var currentObject = this.presentableField.ValueAsObject;
            var comparativeObject = (currentObject as PersistentObject)?.GetVersionValue(this.ComparisonDate);
            if (null == comparativeObject) {
                comparativeValue = null;
            } else {
                lock (this.thisLock) {
                    try {
                        this.presentableField.ValueAsObject = comparativeObject;
                        comparativeValue = this.viewField.GetReadOnlyValueFor(this.presentableField, this.TopmostParentPresentableObject, this.OptionDataProvider);
                    } finally {
                        this.presentableField.ValueAsObject = currentObject;
                    }
                }
            }
            return comparativeValue;
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            string value = this.viewField.GetReadOnlyValueFor(this.presentableField, this.TopmostParentPresentableObject, this.OptionDataProvider);
            bool isDiffNew;
            if (this.ComparisonDate.HasValue) {
                string comparativeValue = this.GetComparativeValue();
                isDiffNew = string.IsNullOrEmpty(comparativeValue) || comparativeValue != value;
                if (isDiffNew && !string.IsNullOrEmpty(comparativeValue)) {
                    html.AppendOpeningTag("span", "diffrm");
                    html.AppendHtmlEncoded(comparativeValue);
                    html.AppendClosingTag("span");
                    if (!string.IsNullOrEmpty(value)) {
                        html.Append(' ');
                    }
                }
            } else {
                isDiffNew = false;
            }
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
                this.ErrorMessage = this.viewField.Validate(this.presentableField, ValidityCheck.Transitional, this.TopmostParentPresentableObject, this.OptionDataProvider);
            }
            return;
        }

    }

}