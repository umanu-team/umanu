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
    /// Field control for an e-mail address.
    /// </summary>
    public class WebFieldForEmailAddress : WebFieldForSingleLineText {

        /// <summary>
        /// HTML type of input field.
        /// </summary>
        protected override string InputType {
            get {
                return "email";
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
        public WebFieldForEmailAddress(IPresentableFieldForElement presentableField, ViewFieldForEmailAddress viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            // nothing to do
        }

        /// <summary>
        /// Renders an email address.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="emailAddress">email address to render</param>
        public static void RenderEmailAddress(HtmlWriter html, string emailAddress) {
            string value = System.Web.HttpUtility.HtmlEncode(emailAddress);
            if (!string.IsNullOrEmpty(value)) {
                var attributes = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("href", "mailto:" + value)
                };
                html.AppendOpeningTag("a", attributes);
                html.Append(value);
                html.AppendClosingTag("a");
            }
            return;
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            if (FieldRenderMode.ListTable == this.RenderMode) {
                base.RenderReadOnlyValue(html);
            } else {
                string value = this.GetReadOnlyValue();
                bool isDiffNew;
                if (this.ComparisonDate.HasValue) {
                    string comparativeValue = this.GetComparativeValue();
                    isDiffNew = string.IsNullOrEmpty(comparativeValue) || comparativeValue != value;
                    if (isDiffNew && !string.IsNullOrEmpty(comparativeValue)) {
                        html.AppendOpeningTag("span", "diffrm");
                        WebFieldForEmailAddress.RenderEmailAddress(html, comparativeValue);
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
                    WebFieldForEmailAddress.RenderEmailAddress(html, value);
                    if (isDiffNew) {
                        html.AppendClosingTag("span");
                    }
                }
            }
            return;
        }

    }

}