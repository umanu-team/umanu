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

namespace Framework.Presentation.Web {

    using System.Collections.Generic;

    /// <summary>
    /// Simple control for representing an HTML tag.
    /// </summary>
    public sealed class WebControl : CascadedControl {

        /// <summary>
        /// HTML attributes of tag of control.
        /// </summary>
        public Dictionary<string, string> Attributes { get; private set; }

        /// <summary>
        /// True if this control has no child controls, false
        /// otherwise.
        /// </summary>
        public bool IsEmpty {
            get {
                return this.Controls.Count < 1;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tagName">HTML tag name of this control</param>
        public WebControl(string tagName)
            : base(tagName) {
            this.Attributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Adds a child control to this control.
        /// </summary>
        /// <param name="childControl">child control to add</param>
        /// <returns>current instance of this</returns>
        public WebControl AddChildControl(Control childControl) {
            this.Controls.Add(childControl);
            return this;
        }

        /// <summary>
        /// Adds child controls to this control.
        /// </summary>
        /// <param name="childControls">child controls to add</param>
        /// <returns>current instance of this</returns>
        public WebControl AddChildControls(IEnumerable<Control> childControls) {
            foreach (var childControl in childControls) {
                this.AddChildControl(childControl);
            }
            return this;
        }

        /// <summary>
        /// Removes all child controls of this control.
        /// </summary>
        public void Clear() {
            this.Controls.Clear();
            return;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            foreach (var attribute in this.Attributes) {
                yield return attribute;
            }
        }

    }

}