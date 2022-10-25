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

namespace Framework.Model.Calendar {

    using Framework.Persistence;
    using Framework.Persistence.Fields;

    /// <summary>
    /// Represents an iCalendar e-mail alarm as defined in RFC5545.
    /// </summary>
    public class EmailAlarm : Alarm {

        /// <summary>
        /// Attachments (ATTACH).
        /// This property provides the capability to associate a
        /// document object with an event.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<File> Attachments { get; private set; }

        /// <summary>
        /// Attendees (ATTENDEE).
        /// </summary>
        public PersistentFieldForIUserCollection Attendees { get; private set; }

        /// <summary>
        /// Description (DESCRIPTION).
        /// This property provides a  description of the alarm.
        /// </summary>
        public string Description {
            get { return this.description.Value; }
            set { this.description.Value = value; }
        }
        private readonly PersistentFieldForString description =
            new PersistentFieldForString(nameof(Description));

        /// <summary>
        /// Title (SUMMARY).
        /// This property defines a short summary or subject for the
        /// alarm.
        /// </summary>
        public string Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PersistentFieldForString title =
            new PersistentFieldForString(nameof(Title));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public EmailAlarm()
            : base() {
            this.Attachments = new PersistentFieldForPersistentObjectCollection<File>(nameof(Attachments), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.Attachments);
            this.Attendees = new PersistentFieldForIUserCollection(nameof(Attendees));
            this.RegisterPersistentField(this.Attendees);
            this.RegisterPersistentField(this.description);
            this.RegisterPersistentField(this.title);
        }

    }

}