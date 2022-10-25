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

namespace Framework.BusinessApplications.Web {

    using Framework.BusinessApplications.Workflows.Forms;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using System;

    /// <summary>
    /// Field control for a material number.
    /// </summary>
    public sealed class WebFieldForMaterialNumber : WebFieldForSingleLineText {

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
        public WebFieldForMaterialNumber(IPresentableFieldForElement presentableField, ViewFieldForMaterialNumber viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            // nothing to do
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
            string cleanedPostBackValue = base.CleanPostBackValue(value, presentableObject, optionDataProvider);
            if (!string.IsNullOrEmpty(cleanedPostBackValue)) {
                cleanedPostBackValue = WebFieldForMaterialNumber.ConvertToFormattedMaterialNumber(cleanedPostBackValue);
            }
            return cleanedPostBackValue;
        }

        /// <summary>
        /// Converts a material number to a formatted one.
        /// </summary>
        /// <param name="materialNumber">material number to convert</param>
        /// <returns>converted material number, e.g. 1-01-480600</returns>
        private static string ConvertToFormattedMaterialNumber(string materialNumber) {
            string formattedMaterialNumber;
            if (string.IsNullOrEmpty(materialNumber)) {
                formattedMaterialNumber = null;
            } else {
                formattedMaterialNumber = materialNumber.Replace("-", string.Empty).Replace("*", string.Empty).TrimStart('0');
                bool isNumber = true;
                foreach (char c in formattedMaterialNumber) {
                    if (!(c >= '0' && c <= '9')) {
                        isNumber = false;
                        break;
                    }
                }
                if (isNumber) {
                    if (formattedMaterialNumber.Length > 1) {
                        formattedMaterialNumber = formattedMaterialNumber.Substring(0, 1) + "-" + formattedMaterialNumber.Substring(1);
                        if (formattedMaterialNumber.Length > 4) {
                            formattedMaterialNumber = formattedMaterialNumber.Substring(0, 4) + "-" + formattedMaterialNumber.Substring(4);
                        }
                    }
                } else {
                    formattedMaterialNumber = materialNumber;
                }
            }
            return formattedMaterialNumber;
        }

    }

}