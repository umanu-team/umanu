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

    /// <summary>
    /// Represents an RFC 5546 method.
    /// </summary>
    public enum Method {

        /// <summary>
        /// Used not to use any RFC 5546 method.
        /// </summary>
        None = 0,

        /// <summary>
        /// Used to publish an iCalendar object to one or more
        /// "Calendar Users". There is no interactivity between the
        /// publisher and any other "Calendar User". An example might
        /// include a baseball team publishing its schedule to the
        /// public.
        /// </summary>
        Publish = 1,

        /// <summary>
        /// Used to schedule an iCalendar object with other "Calendar
        /// Users". Requests are interactive in that they require the
        /// receiver to respond using the reply methods. Meeting
        /// requests, busy-time requests, and the assignment of tasks
        /// to other "Calendar Users" are all examples. Requests are
        /// also used by the Organizer to update the status of an
        /// iCalendar object.
        /// </summary>
        Request = 2,

        /// <summary>
        /// A reply is used in response to a request to convey
        /// Attendee status to the Organizer. Replies are commonly
        /// used to respond to meeting and task requests.
        /// </summary>
        Reply = 3,

        /// <summary>
        /// Add one or more new instances to an existing recurring
        /// iCalendar object.
        /// </summary>
        Add = 4,

        /// <summary>
        /// Cancel one or more instances of an existing iCalendar
        /// object.
        /// </summary>
        Cancel = 5,

        /// <summary>
        /// Used by an Attendee to request the latest version of an
        /// iCalendar object.
        /// </summary>
        Refresh = 6,

        /// <summary>
        /// Used by an Attendee to negotiate a change in an iCalendar
        /// object. Examples include the request to change a proposed
        /// event time or change the due date for a task.
        /// </summary>
        Counter = 7,

        /// <summary>
        /// Used by the Organizer to decline the proposed counter
        /// proposal.
        /// </summary>
        DeclineCounter = 8

    }

}