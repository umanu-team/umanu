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

    using Framework.Model;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Field control for date and/or time.
    /// </summary>
    public class WebFieldForDateTime : WebFieldForElement {

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForDateTime ViewField { get; private set; }

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
        public WebFieldForDateTime(IPresentableFieldForElement presentableField, ViewFieldForDateTime viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.ViewField = viewField;
        }

        /// <summary>
        /// Renders a control for editing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderEditableValue(HtmlWriter html) {
            string inputType;
            double step;
            if (DateTimeType.Date == this.ViewField.DateTimeType) {
                inputType = "date";
                step = Math.Round(this.ViewField.Step.TotalDays);
            } else if (DateTimeType.LocalDateAndTime == this.ViewField.DateTimeType) {
                inputType = "datetime-local";
                step = Math.Round(this.ViewField.Step.TotalSeconds);
            } else if (DateTimeType.Month == this.ViewField.DateTimeType) {
                inputType = "month";
                step = Math.Round(this.ViewField.Step.TotalSeconds / 2629746);
            } else if (DateTimeType.Time == this.ViewField.DateTimeType) {
                inputType = "time";
                step = Math.Round(this.ViewField.Step.TotalSeconds);
            } else if (DateTimeType.Week == this.ViewField.DateTimeType) {
                inputType = "week";
                step = Math.Round(this.ViewField.Step.TotalDays / 7);
            } else {
                throw new ArgumentException("DateTimeType \"" + this.ViewField.DateTimeType.ToString() + "\" is not known.");
            }
            var attributes = new Dictionary<string, string>(10);
            attributes.Add("id", this.ClientFieldId);
            attributes.Add("type", inputType);
            attributes.Add("name", this.ClientFieldId);
            if (this.ViewField.IsAutofocused) {
                attributes.Add("autofocus", "autofocus");
            }
            if (Mandatoriness.Required == this.ViewField.Mandatoriness) {
                attributes.Add("required", "required");
            }
            attributes.Add("max", UtcDateTime.FormatAsIso8601Value(this.ViewField.MaxValue, this.ViewField.DateTimeType));
            attributes.Add("min", UtcDateTime.FormatAsIso8601Value(this.ViewField.MinValue, this.ViewField.DateTimeType));
            IDictionary<string, string> options;
            if (null == this.ViewField.OptionProvider) {
                options = new Dictionary<string, string>(0);
            } else {
                options = this.ViewField.OptionProvider.GetOptionDictionary(this.ParentPresentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
            }
            if (options.Count > 0) {
                attributes.Add("list", "o" + this.ClientFieldId);
            }
            if (step > 0) {
                attributes.Add("step", step.ToString(CultureInfo.InvariantCulture));
            }
            string value;
            if (UtcDateTime.TryParse(this.EditableValue, out DateTime dateTime)) {
                value = UtcDateTime.FormatAsIso8601Value(dateTime, this.ViewField.DateTimeType);
            } else {
                value = this.EditableValue;
            }
            attributes.Add("value", value);
            html.AppendSelfClosingTag("input", attributes);
            this.RenderDataList(html, "o" + this.ClientFieldId, options);
            return;
        }

        /// <summary>
        /// Renders the read-only value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            DateTime? value = this.PresentableField.ValueAsObject as DateTime?;
            bool isDiffNew;
            if (this.ComparisonDate.HasValue) {
                DateTime? comparativeValue = null;
                var comparativeField = this.PresentableField.GetVersionedField(this.ComparisonDate);
                if (null != comparativeField) {
                    comparativeValue = comparativeField.ValueAsObject as DateTime?;
                }
                isDiffNew = !comparativeValue.HasValue || comparativeValue != value;
                if (isDiffNew && comparativeValue.HasValue) {
                    html.AppendOpeningTag("span", "diffrm");
                    html.Append(Time.RenderHtmlTag(comparativeValue.Value, this.ViewField.DateTimeType));
                    html.AppendClosingTag("span");
                    if (value.HasValue) {
                        html.Append(' ');
                    }
                }
            } else {
                isDiffNew = false;
            }
            if (value.HasValue) {
                if (isDiffNew) {
                    html.AppendOpeningTag("span", "diffnew");
                }
                html.Append(Time.RenderHtmlTag(value.Value, this.ViewField.DateTimeType));
                if (isDiffNew) {
                    html.AppendClosingTag("span");
                }
            }
            return;
        }

    }

}