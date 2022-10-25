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

namespace Framework.Presentation.Forms {

    using Framework.Persistence;
    using Framework.Persistence.Fields;

    /// <summary>
    /// Object to be used in views.
    /// </summary>
    public abstract class ViewObject : PersistentObject {

        /// <summary>
        /// True if this object is supposed to be shown in views,
        /// false otherwise.
        /// </summary>
        public bool IsVisible {
            get { return this.isVisible.Value; }
            set { this.isVisible.Value = value; }
        }
        private readonly PersistentFieldForBool isVisible =
            new PersistentFieldForBool(nameof(IsVisible), true);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewObject()
            : base() {
            this.RegisterPersistentField(this.isVisible);
        }

    }

}