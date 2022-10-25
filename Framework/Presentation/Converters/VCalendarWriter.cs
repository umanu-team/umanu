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

namespace Framework.Presentation.Converters {

    using Model.Calendar;
    using Persistence;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Converter for calendar objects to iCalendar format.
    /// </summary>
    public sealed class VCalendarWriter {

        /// <summary>
        /// List of events to be converted.
        /// </summary>
        public IList<Event> Events {
            get { return this.events; }
        }
        private readonly List<Event> events;

        /// <summary>
        /// True to include attachments in output, false to skip
        /// attachments.
        /// </summary>
        public bool IsWritingAttachments { get; set; }

        /// <summary>
        /// RFC 5546 method to be used.
        /// </summary>
        public Method Method { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public VCalendarWriter()
            : this(Method.None) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="method">RFC 5546 method to be used</param>
        public VCalendarWriter(Method method)
            : base() {
            this.events = new List<Event>();
            this.IsWritingAttachments = true;
            this.Method = method;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="method">RFC 5546 method to be used</param>
        /// <param name="events">events to be converted</param>
        public VCalendarWriter(Method method, IEnumerable<Event> events)
            : this(method) {
            this.events.AddRange(events);
        }

        /// <summary>
        /// Returns an iCalendar string that represents the current
        /// object.
        /// </summary>
        /// <returns>iCalendar string that represents the current
        /// object</returns>
        public override string ToString() {
            var vCalendarBuilder = new VCalendarBuilder(this.Method) {
                IsWritingAttachments = this.IsWritingAttachments
            };
            vCalendarBuilder.Append("BEGIN", "VCALENDAR");
            vCalendarBuilder.Append("VERSION", "2.0");
            vCalendarBuilder.Append("PRODID", "-//Framework//NONSGML Calendar//EN");
            if (Method.None != this.Method) {
                vCalendarBuilder.Append("METHOD", this.Method.ToString().ToUpperInvariant());
            }
            var timeZones = new List<TimeZoneInfo>();
            foreach (var item in this.Events) {
                if (TimeZoneInfo.Utc != item.TimeZone && !timeZones.Contains(item.TimeZone)) {
                    vCalendarBuilder.Append(item.TimeZone);
                    timeZones.Add(item.TimeZone);
                }
            }
            foreach (var item in this.Events) {
                vCalendarBuilder.Append(item);
            }
            vCalendarBuilder.Append("END", "VCALENDAR");
            return vCalendarBuilder.ToString();
        }

        /// <summary>
        /// Returns an iCalendar file that represents the current
        /// object.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <returns>iCalendar file that represents the current
        /// object</returns>
        public File ToFile(string name) {
            return new File(name, "text/calendar", Encoding.UTF8.GetBytes(this.ToString()));
        }

    }

}