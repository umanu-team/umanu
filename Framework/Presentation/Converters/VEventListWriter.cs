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

    using Framework.Model.Calendar;
    using Persistence;
    using Presentation;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Converter for event objects to iCalendar format.
    /// </summary>
    public sealed class VEventListWriter {

        /// <summary>
        /// List of events to be converted.
        /// </summary>
        public IList<IPresentableObject> Events {
            get { return this.events; }
        }
        private List<IPresentableObject> events;

        /// <summary>
        /// RFC 5546 method to be used.
        /// </summary>
        public Method Method { get; set; }

        /// <summary>
        /// Event list view to be applied for field mapping.
        /// </summary>
        public VEventListView View { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="view">event list view to be applied for
        /// field mapping</param>
        public VEventListWriter(VEventListView view)
            : this(view, Method.None) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="view">event list view to be applied for
        /// field mapping</param>
        /// <param name="method">RFC 5546 method to be used</param>
        public VEventListWriter(VEventListView view, Method method)
            : base() {
            this.events = new List<IPresentableObject>();
            this.Method = method;
            this.View = view;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="view">event list view to be applied for
        /// field mapping</param>
        /// <param name="events">events to be converted</param>
        public VEventListWriter(VEventListView view, IEnumerable<IPresentableObject> events)
            : this(view, Method.None, events) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="view">event list view to be applied for
        /// field mapping</param>
        /// <param name="method">RFC 5546 method to be used</param>
        /// <param name="events">events to be converted</param>
        public VEventListWriter(VEventListView view, Method method, IEnumerable<IPresentableObject> events)
            : this(view, method) {
            this.events.AddRange(events);
        }

        /// <summary>
        /// Returns an iCalendar string that represents the current
        /// object.
        /// </summary>
        /// <returns>iCalendar string that represents the current
        /// object</returns>
        public override string ToString() {
            var vCalendarBuilder = new VCalendarBuilder(this.Method);
            vCalendarBuilder.Append("BEGIN", "VCALENDAR");
            vCalendarBuilder.Append("VERSION", "2.0");
            vCalendarBuilder.Append("PRODID", "-//Framework//NONSGML Calendar//EN");
            if (Method.None != this.Method) {
                vCalendarBuilder.Append("METHOD", this.Method.ToString().ToUpperInvariant());
            }
            foreach (var item in this.Events) {
                vCalendarBuilder.Append(item, this.View);
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