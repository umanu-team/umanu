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
    /// Action button for printing current page.
    /// </summary>
    public sealed class PrintButton : ClientSideButton {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        public PrintButton(string title)
            : base(title) {
            // nothing to do
        }

        /// <summary>
        /// Gets the client action to execute on click - it may be
        /// null or empty.
        /// </summary>
        /// <param name="positionId">ID of parent widget or null</param>
        /// <returns>client action to execute on click - it may be
        /// null or empty</returns>
        public override string GetOnClientClick(ulong? positionId) {
            return "javascript:window.print();";
        }

    }

}