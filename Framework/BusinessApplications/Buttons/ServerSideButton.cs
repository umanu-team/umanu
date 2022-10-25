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
    using Framework.Presentation.Forms;
    using System;
    using System.Globalization;

    /// <summary>
    /// Button of action bar with server-side on-click action.
    /// </summary>
    public abstract class ServerSideButton : ActionButton {

        /// <summary>
        /// Confirmation message to display on click.
        /// </summary>
        public string ConfirmationMessage { get; set; }

        /// <summary>
        /// Indicates whether button prompts for user
        /// input. This is DEPRECATED - please use an
        /// ActionFieldButton instead.
        /// </summary>
        public bool PromptsForUserInput { get; set; }

        /// <summary>
        /// URL to redirect to after button click. A value of
        /// string.Empty causes a redirect to the same page, whereas
        /// null suppresses any redirection.
        /// </summary>
        public string RedirectionTarget { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        public ServerSideButton(string title)
            : base(title) {
            this.PromptsForUserInput = false;
            this.RedirectionTarget = "../";
        }

        /// <summary>
        /// Gets the hash of this action button dependent on type,
        /// title, icon url and number and title of allowed groups
        /// for reading.
        /// </summary>
        /// <returns>hash of this action button</returns>
        public override int GetHashCode() {
            string s = base.GetHashCode().ToString(CultureInfo.InvariantCulture) + Environment.NewLine;
            if (!string.IsNullOrEmpty(this.ConfirmationMessage)) {
                s += this.ConfirmationMessage;
            }
            return s.GetHashCode();
        }

        /// <summary>
        /// Server-side action to execute on click.
        /// </summary>
        /// <param name="form">current form or null</param>
        /// <param name="promptInput">prompt input of user</param>
        public abstract void OnClick(IForm form, string promptInput);

    }

}