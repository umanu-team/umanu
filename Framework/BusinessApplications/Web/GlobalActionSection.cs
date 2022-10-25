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

namespace Framework.BusinessApplications.Web {

    using Framework.Presentation.Web;

    /// <summary>
    /// Section control for global actions.
    /// </summary>
    public sealed class GlobalActionSection : CascadedControl {

        /// <summary>
        /// True if section is empty, false otherwise.
        /// </summary>
        public bool IsEmpty {
            get {
                return this.Controls.Count < 1;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public GlobalActionSection()
            : base("div") {
            // nothing to do
        }

        /// <summary>
        /// Adds a global action button.
        /// </summary>
        /// <param name="button">global action button to add</param>
        internal void AddButton(ClientSideWebButton button) {
            this.Controls.Add(button);
            return;
        }

        /// <summary>
        /// Gets a value indicating whether control is supposed to be
        /// rendered.
        /// </summary>
        /// <returns>true if control is supposed to be rendered,
        /// false otherwise</returns>
        protected override bool GetIsVisible() {
            return !this.IsEmpty;
        }

    }

}