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

namespace Framework.BusinessApplications.Buttons {

    using Framework.Presentation.Buttons;

    /// <summary>
    /// Template for server-side action button.
    /// </summary>
    public sealed class ServerSideButtonTemplate : ActionButton {

        /// <summary>
        /// Confirmation message to display on click.
        /// </summary>
        public string ConfirmationMessage { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        public ServerSideButtonTemplate(string title)
            : base(title) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of button</param>
        /// <param name="confirmationMessage">confirmation message to
        /// display on click</param>
        public ServerSideButtonTemplate(string title, string confirmationMessage)
            : this(title) {
            this.ConfirmationMessage = confirmationMessage;
        }

    }

}