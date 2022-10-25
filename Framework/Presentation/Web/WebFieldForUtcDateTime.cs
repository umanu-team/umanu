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
    /// Field control for date and/or time with support for non-local
    /// values in UTC.
    /// </summary>
    public class WebFieldForUtcDateTime : WebFieldForDateTime {

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
        public WebFieldForUtcDateTime(IPresentableFieldForElement presentableField, ViewFieldForDateTime viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            // nothing to do
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            if (DateTimeType.DateAndTime == this.ViewField.DateTimeType) {
                this.ErrorMessage = null;
                this.IsIncludedInPostBack = false;
                if (FieldRenderMode.Form == this.RenderMode && PostBackState.ValidPostBack == this.PostBackState) {
                    if (this.IsReadOnly) {
                        this.IsIncludedInPostBack = true;
                    } else {
                        var postBackDateString = httpRequest.Form[this.ClientFieldId];
                        var postBackTimeString = httpRequest.Form["Time_" + this.ClientFieldId];
                        if (null != postBackDateString && null != postBackTimeString) {
                            this.IsIncludedInPostBack = true;
                            postBackDateString = this.CleanPostBackValue(postBackDateString, this.TopmostParentPresentableObject, this.OptionDataProvider);
                            postBackTimeString = this.CleanPostBackValue(postBackTimeString, this.TopmostParentPresentableObject, this.OptionDataProvider);
                            string postBackDateTimeString;
                            if (string.IsNullOrEmpty(postBackDateString) && string.IsNullOrEmpty(postBackTimeString)) {
                                postBackDateTimeString = null;
                            } else if (string.IsNullOrEmpty(postBackTimeString) || string.IsNullOrEmpty(postBackDateString)) {
                                postBackDateTimeString = " ";
                            } else {
                                postBackDateTimeString = postBackDateString + 'T' + postBackTimeString;
                            }
                            if (DateTime.TryParse(postBackDateTimeString, out DateTime postBackDateTime)) {
                                postBackDateTime = DateTime.SpecifyKind(postBackDateTime, DateTimeKind.Utc);
                                this.PostBackValue = UtcDateTime.FormatAsIso8601Value(postBackDateTime, this.ViewField.DateTimeType);
                            } else {
                                this.PostBackValue = postBackDateTimeString;
                            }
                            if (this.PresentableField.ValueAsString != this.PostBackValue) {
                                var hashedPreviousValue = httpRequest.Form[this.ClientFieldId + "::"];
                                if (WebFieldForElement.GetHashedValueFor(this.PostBackValue) == hashedPreviousValue) {
                                    this.PostBackValue = this.PresentableField.ValueAsString;
                                } else {
                                    if (!this.PresentableField.TrySetValueAsString(this.PostBackValue)) {
                                        this.ErrorMessage = this.ViewField.GetDefaultErrorMessage();
                                    }
                                }
                            }

                        }
                    }
                }
            } else {
                base.CreateChildControls(httpRequest);
            }
            return;
        }

        /// <summary>
        /// Renders a control for editing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderEditableValue(HtmlWriter html) {
            if (DateTimeType.DateAndTime == this.ViewField.DateTimeType) {
                // date
                var dateAttributes = new Dictionary<string, string>(10) {
                    { "id", this.ClientFieldId },
                    { "type", "date" },
                    { "name", this.ClientFieldId }
                };
                if (this.ViewField.IsAutofocused) {
                    dateAttributes.Add("autofocus", "autofocus");
                }
                if (Mandatoriness.Required == this.ViewField.Mandatoriness) {
                    dateAttributes.Add("required", "required");
                }
                dateAttributes.Add("max", UtcDateTime.FormatAsIso8601Value(this.ViewField.MaxValue, DateTimeType.Date));
                dateAttributes.Add("min", UtcDateTime.FormatAsIso8601Value(this.ViewField.MinValue, DateTimeType.Date));
                IDictionary<string, string> options;
                if (null == this.ViewField.OptionProvider) {
                    options = new Dictionary<string, string>(0);
                } else {
                    options = this.ViewField.OptionProvider.GetOptionDictionary(this.ParentPresentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
                }
                if (options.Count > 0) {
                    dateAttributes.Add("list", "o" + this.ClientFieldId);
                }
                var dateStep = Math.Round(this.ViewField.Step.TotalDays);
                if (dateStep > 0) {
                    dateAttributes.Add("step", dateStep.ToString(CultureInfo.InvariantCulture));
                }
                string dateValue;
                if (UtcDateTime.TryParse(this.EditableValue, out DateTime dateTime)) {
                    dateValue = UtcDateTime.FormatAsIso8601Value(dateTime, DateTimeType.Date);
                } else {
                    dateValue = this.EditableValue;
                }
                dateAttributes.Add("value", dateValue);
                html.AppendSelfClosingTag("input", dateAttributes);
                this.RenderDataList(html, "o" + this.ClientFieldId, options);
                // time
                var timeAttributes = new Dictionary<string, string>(6) {
                    { "id", "Time_" + this.ClientFieldId },
                    { "type", "time" },
                    { "name", "Time_" + this.ClientFieldId }
                };
                if (Mandatoriness.Required == this.ViewField.Mandatoriness) {
                    timeAttributes.Add("required", "required");
                }
                var timeStep = Math.Round(this.ViewField.Step.TotalSeconds);
                if (timeStep > 0) {
                    timeAttributes.Add("step", timeStep.ToString(CultureInfo.InvariantCulture));
                }
                string timeValue;
                if (UtcDateTime.TryParse(this.EditableValue, out DateTime time)) {
                    timeValue = UtcDateTime.FormatAsIso8601Value(time, DateTimeType.Time);
                } else {
                    timeValue = null;
                }
                timeAttributes.Add("value", timeValue);
                html.AppendSelfClosingTag("input", timeAttributes);
            } else {
                base.RenderEditableValue(html);
            }
            return;
        }

    }

}