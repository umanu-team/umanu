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

namespace Framework.Presentation.Web.Controllers {

    using Converters;
    using Model.Calendar;
    using Persistence;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Lightweight controller for processing requests of dynamic
    /// iCalendar files.
    /// </summary>
    public abstract class VCalendarController : FileController {

        /// <summary>
        /// Absolute URL of dynamic iCalendar file - it may not be
        /// empty, not contain any special charaters except for,
        /// dashes, has to start with a slash and has to end with
        /// ".ics".
        /// </summary>
        public string AbsoluteUrl {
            get {
                return this.absoluteUrl;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of dynamic iCalendar file does not start with a slash.");
                }
                if (!value.EndsWith(".ics", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of dynamic iCalendar file does not end with \".ics\".");
                }
                this.absoluteUrl = value;
            }
        }
        private string absoluteUrl;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="absoluteUrl">absolute URL of dynamic
        /// iCalendar file - it may not be empty, not contain any
        /// special charaters except for, dashes, has to start with a
        /// dash and has to end with ".ics"</param>
        public VCalendarController(string absoluteUrl)
            : base(CacheControl.NoStore, false) {
            this.AbsoluteUrl = absoluteUrl;
        }

        /// <summary>
        /// Finds the events to be listed in dynamic iCalendar file.
        /// </summary>
        /// <returns>enumerable of events to be listed in dynamic
        /// iCalendar file</returns>
        protected abstract IEnumerable<Event> FindEvents();

        /// <summary>
        /// Finds the file for a specific URL.
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <returns>file for URL or null</returns>
        protected sealed override File FindFile(Uri url) {
            File file;
            if (url.AbsolutePath == this.AbsoluteUrl) {
                var iCalendar = new VCalendarWriter(Method.None, this.FindEvents());
                file = iCalendar.ToFile("calendar.ics");
            } else {
                file = null;
            }
            return file;
        }

    }

}