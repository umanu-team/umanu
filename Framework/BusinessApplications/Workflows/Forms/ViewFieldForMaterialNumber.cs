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

namespace Framework.BusinessApplications.Workflows.Forms {

    using Framework.Presentation.Forms;

    /// <summary>
    /// Field for material number to be presented in a view.
    /// </summary>
    public class ViewFieldForMaterialNumber : ViewFieldForSingleLineText {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForMaterialNumber()
            : base() {
            this.MaxLength = 11;
            this.Placeholder = "X-XX-XXXXXX";
            this.ValidationPattern = "^([0-9]\\-?){9}$";
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForMaterialNumber(string title, string key, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.Key = key;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForMaterialNumber(string title, string[] keyChain, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        private ViewFieldForMaterialNumber(string title, Mandatoriness mandatoriness)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
        }

    }

}