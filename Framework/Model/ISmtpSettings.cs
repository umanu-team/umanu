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

namespace Framework.Model {

    /// <summary>
    /// Represents SMTP settings.
    /// </summary>
    public interface ISmtpSettings {

        /// <summary>
        /// Indicates whether SSL encryption is to be used for SMTP.
        /// </summary>
        bool SmtpEnableSsl { get; set; }

        /// <summary>
        /// SMTP from address.
        /// </summary>
        string SmtpFrom { get; set; }

        /// <summary>
        /// SMTP host.
        /// </summary>
        string SmtpHost { get; set; }

        /// <summary>
        /// SMTP password.
        /// </summary>
        string SmtpPassword { get; set; }

        /// <summary>
        /// SMTP port.
        /// </summary>
        ushort SmtpPort { get; set; }

        /// <summary>
        /// SMTP user name.
        /// </summary>
        string SmtpUserName { get; set; }

    }

}