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

    using System.Runtime.Serialization;

    /// <summary>
    /// Enum of possible calendar request status replies as defined
    /// in RFC2446.
    /// </summary>
    public enum RequestStatus {

        /// <summary>
        /// Success.
        /// </summary>
        [EnumMemberAttribute]
        None = 0,

        /// <summary>
        /// Success.
        /// </summary>
        [EnumMemberAttribute]
        Success = 20,

        /// <summary>
        /// Success but fallback taken on one or more property values.
        /// </summary>
        [EnumMemberAttribute]
        SuccessButFallbackTakenOnOneOrMorePropertyValues = 21

/*
|==============+============================+=========================|
| Short Return | Longer Return Status       | Offending Data          |
| Status Code  | Description                |                         |
|==============+============================+=========================|
| 2.0          | Success.                   | None.                   |
|==============+============================+=========================|
| 2.1          | Success but fallback taken | Property name and value |
|              | on one or more property    | MAY be specified.       |
|              | values.                    |                         |
|==============+============================+=========================|
| 2.2          | Success, invalid property  | Property name MAY be    |
|              | ignored.                   | specified.              |
|==============+============================+=========================|
| 2.3          | Success, invalid property  | Property parameter name |
|              | parameter ignored.         | and value MAY be        |
|              |                            | specified.              |
|==============+============================+=========================|
| 2.4          | Success, unknown non-      | Non-standard property   |
|              | standard property ignored. | name MAY be specified.  |
|==============+============================+=========================|
| 2.5          | Success, unknown non       | Property and non-       |
|              | standard property value    | standard value MAY be   |
|              | ignored.                   | specified.              |
|==============+============================+=========================|
| 2.6          | Success, invalid calendar  | Calendar component      |
|              | component ignored.         | sentinel (e.g., BEGIN:  |
|              |                            | ALARM) MAY be           |
|              |                            | specified.              |
|==============+============================+=========================|
| 2.7          | Success, request forwarded | Original and forwarded  |
|              | to Calendar User.          | caluser addresses MAY   |
|              |                            | be specified.           |
|==============+============================+=========================|
| 2.8          | Success, repeating event   | RRULE or RDATE property |
|              | ignored. Scheduled as a    | name and value MAY be   |
|              | single component.          | specified.              |
|==============+============================+=========================|
| 2.9          | Success, truncated end date| DTEND property value    |
|              | time to date boundary.     | MAY be specified.       |
|==============+============================+=========================|
| 2.10         | Success, repeating VTODO   | RRULE or RDATE property |
|              | ignored. Scheduled as a    | name and value MAY be   |
|              | single VTODO.              | specified.              |
|==============+============================+=========================|
| 2.11         | Success, unbounded RRULE   | RRULE property name and |
|              | clipped at some finite     | value MAY be specified. |
|              | number of instances        | Number of instances MAY |
|              |                            | also be specified.      |
|==============+============================+=========================|
| 3.0          | Invalid property name.     | Property name MAY be    |
|              |                            | specified.              |
|==============+============================+=========================|
| 3.1          | Invalid property value.    | Property name and value |
|              |                            | MAY be specified.       |
|==============+============================+=========================|
| 3.2          | Invalid property parameter.| Property parameter name |
|              |                            | and value MAY be        |
|              |                            | specified.              |
|==============+============================+=========================|
| 3.3          | Invalid property parameter | Property parameter name |
|              | value.                     | and value MAY be        |
|              |                            | specified.              |
|==============+============================+=========================|
| 3.4          | Invalid calendar component | Calendar component      |
|              | sequence.                  | sentinel MAY be         |
|              |                            | specified (e.g., BEGIN: |
|              |                            | VTIMEZONE).             |
|==============+============================+=========================|
| 3.5          | Invalid date or time.      | Date/time value(s) MAY  |
|              |                            | be specified.           |
|==============+============================+=========================|
| 3.6          | Invalid rule.              | Rule value MAY be       |
|              |                            | specified.              |
|==============+============================+=========================|
| 3.7          | Invalid Calendar User.     | Attendee property value |
|              |                            |MAY be specified.        |
|==============+============================+=========================|
| 3.8          | No authority.              | METHOD and Attendee     |
|              |                            | property values MAY be  |
|              |                            | specified.              |
|==============+============================+=========================|
| 3.9          | Unsupported version.       | VERSION property name   |
|              |                            | and value MAY be        |
|              |                            | specified.              |
|==============+============================+=========================|
| 3.10         | Request entity too large.  | None.                   |
|==============+============================+=========================|
| 3.11         | Required component or      | Component or property   |
|              | property missing.          | name MAY be specified.  |
|==============+============================+=========================|
| 3.12         | Unknown component or       | Component or property   |
|              | property found             | name MAY be specified   |
|==============+============================+=========================|
| 3.13         | Unsupported component or   | Component or property   |
|              | property found             | name MAY be specified   |
|==============+============================+=========================|
| 3.14         | Unsupported capability     | Method or action MAY    |
|              |                            | be specified            |
|==============+============================+=========================|
| 4.0          | Event conflict. Date/time  | DTSTART and DTEND       |
|              | is busy.                   | property name and values|
|              |                            | MAY be specified.       |
|==============+============================+=========================|
| 5.0          | Request MAY supported.     | Method property value   |
|              |                            | MAY be specified.       |
|==============+============================+=========================|
| 5.1          | Service unavailable.       | ATTENDEE property value |
|              |                            | MAY be specified.       |
|==============+============================+=========================|
| 5.2          | Invalid calendar service.  | ATTENDEE property value |
|              |                            | MAY be specified.       |
|==============+============================+=========================|
| 5.3          | No scheduling support for  | ATTENDEE property value |
|              | user.                      | MAY be specified.       |
|==============+============================+=========================|
*/

    }

}
