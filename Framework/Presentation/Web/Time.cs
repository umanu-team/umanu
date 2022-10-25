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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Control for rendering time tags.
    /// </summary>
    public sealed class Time : Control {

        /// <summary>
        /// Date/time to render.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Format of date/time to apply.
        /// </summary>
        public DateTimeType DateTimeType { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Time()
            : base("time") {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="dateTime">date/time to render</param>
        /// <param name="dateTimeType">type of date/time to format
        /// date/time value as</param>
        public Time(DateTime dateTime, DateTimeType dateTimeType)
            : this() {
            this.DateTime = dateTime;
            this.DateTimeType = dateTimeType;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            if (DateTimeType.LocalDateAndTime != this.DateTimeType) { // avoids a bug in Chrome, which would convert local times to a wrong time zone
                var dateTimeFormat = Time.GetDateTimeFormat(this.DateTime, this.DateTimeType);
                yield return new KeyValuePair<string, string>("datetime", UtcDateTime.FormatAsIso8601Value(this.DateTime, dateTimeFormat));
                if (DateTimeType.DateAndTime != this.DateTimeType) {
                    yield return new KeyValuePair<string, string>("data-type", this.DateTimeType.ToString().ToLowerInvariant());
                }
            }
        }

        /// <summary>
        /// Gets the format of date/time to be used in datetime
        /// attribute.
        /// </summary>
        /// <param name="dateTime">date/time to build time tag for</param>
        /// <param name="dateTimeType">format of date/time to apply</param>
        /// <returns>format of date/time to be used in datetime
        /// attribute</returns>
        private static DateTimeType GetDateTimeFormat(DateTime dateTime, DateTimeType dateTimeType) {
            var dateTimeFormat = dateTimeType;
            if (DateTimeType.Time == dateTimeFormat && (dateTime.Year > 1 || dateTime.Month > 1 || dateTime.Day > 1)) {
                dateTimeFormat = DateTimeType.DateAndTime; // to enable automatic time conversion if dateTime was entered as date-and-time value and not as time-only value; however, if dateTime was entered as local-date-and-time value it won't be displayed as time-only value correctly
            }
            return dateTimeFormat;
        }

        /// <summary>
        /// Builds a date/time html time tag.
        /// </summary>
        /// <param name="dateTime">date/time to build time tag for</param>
        /// <param name="dateTimeType">format of date/time to apply</param>
        public static string RenderHtmlTag(DateTime dateTime, DateTimeType dateTimeType) {
            string htmlTag = "<time";
            if (DateTimeType.LocalDateAndTime != dateTimeType) { // avoids a bug in Chrome, which would convert local times to a wrong time zone
                var dateTimeFormat = Time.GetDateTimeFormat(dateTime, dateTimeType);
                htmlTag += " datetime=\"" + UtcDateTime.FormatAsIso8601Value(dateTime, dateTimeFormat) + '\"';
                if (DateTimeType.DateAndTime != dateTimeType) {
                    htmlTag += " data-type=\"" + dateTimeType.ToString().ToLowerInvariant() + '\"';
                }
            }
            htmlTag += '>' + UtcDateTime.FormatAsReadOnlyValue(dateTime, dateTimeType) + "</time>";
            return htmlTag;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.Append(UtcDateTime.FormatAsReadOnlyValue(this.DateTime, this.DateTimeType));
            return;
        }

    }

}