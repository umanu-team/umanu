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

    /// <summary>
    /// Field control for a single lookup value of type string.
    /// </summary>
    public class WebFieldForStringLookup : WebFieldForLookup {

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
                if (null == this.ViewField.LookupProvider) {
                    throw new InvalidOperationException("Lookup provider of view field for key \"" + this.ViewField.Key + "\" must not be null.");
                } else if (PostBackState.ValidPostBack == this.PostBackState) {
                    value = this.ViewField.LookupProvider.FindValueForKey(this.PostBackValue, this.TopmostParentPresentableObject, this.OptionDataProvider);
                    if (null == value) {
                        value = this.PostBackValue;
                    }
                } else {
                    value = this.ViewField.LookupProvider.FindValueForKey(this.PresentableField.ValueAsString, this.TopmostParentPresentableObject, this.OptionDataProvider);
                    if (null == value) {
                        value = this.PresentableField.ValueAsString;
                    }
                }
                return value;
            }
        }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForStringLookup ViewField { get; private set; }

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
        public WebFieldForStringLookup(IPresentableFieldForElement presentableField, ViewFieldForStringLookup viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.ViewField = viewField;
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
            string uniqueValue = this.ViewField.LookupProvider.FindUniqueValueByVagueTerm(cleanedValue, presentableObject, optionDataProvider);
            string key;
            if (null == uniqueValue) {
                key = null;
            } else {
                key = this.ViewField.LookupProvider.FindKeyForValue(uniqueValue, presentableObject, optionDataProvider);
            }
            if (string.IsNullOrEmpty(key)) {
                key = cleanedValue;
            }
            return key;
        }

    }

}