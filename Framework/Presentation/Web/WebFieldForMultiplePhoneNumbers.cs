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
    /// Field control for multiple phone numbers.
    /// </summary>
    public class WebFieldForMultiplePhoneNumbers : WebFieldForMultipleSingleLineTexts {

        /// <summary>
        /// HTML type of input field.
        /// </summary>
        protected override string InputType {
            get {
                return "tel";
            }
        }

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
        public WebFieldForMultiplePhoneNumbers(IPresentableFieldForCollection presentableField, ViewFieldForMultiplePhoneNumbers viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            // nothing to do
        }

        /// <summary>
        /// Renders a read only paragraph showing the values.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="currentValues">list of selected values</param>
        /// <param name="comparativeValues">list of comparative
        /// values</param>
        protected override void RenderReadOnlyValue(HtmlWriter html, IList<string> currentValues, IList<string> comparativeValues) {
            if (FieldRenderMode.ListTable == this.RenderMode) {
                base.RenderReadOnlyValue(html, currentValues, comparativeValues);
            } else {
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
                                WebFieldForPhoneNumber.RenderPhoneNumber(html, comparativeValue);
                                html.AppendClosingTag("span");
                            }
                        }
                    }
                }
                foreach (string currentValue in currentValues) {
                    if (!string.IsNullOrEmpty(currentValue)) {
                        if (isFirstValue) {
                            isFirstValue = false;
                        } else {
                            html.Append(this.GetValueSeparator());
                        }
                        bool isSelectedKeyContainedInComparativeKeys = null == comparativeValues || comparativeValues.Contains(currentValue);
                        if (!isSelectedKeyContainedInComparativeKeys) {
                            html.AppendOpeningTag("span", "diffnew");
                        }
                        WebFieldForPhoneNumber.RenderPhoneNumber(html, currentValue);
                        if (!isSelectedKeyContainedInComparativeKeys) {
                            html.AppendClosingTag("span");
                        }
                    }
                }
            }
            return;
        }

    }

}