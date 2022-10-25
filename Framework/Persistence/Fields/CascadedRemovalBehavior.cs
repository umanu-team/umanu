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

namespace Framework.Persistence.Fields {

    using System.Runtime.Serialization;
    
    /// <summary>
    /// Specifies the behavior to apply on cascaded removal of parent
    /// objects of persistent fields.
    /// </summary>
    public enum CascadedRemovalBehavior {

        /// <summary>
        /// Do not remove field values on cascaded removal of parent
        /// object.
        /// </summary>
        [EnumMemberAttribute]
        DoNotRemoveValues = 0,

        /// <summary>
        /// Remove field values if they are not referenced by other
        /// objects any more on cascaded removal of parent object.
        /// </summary>
        [EnumMemberAttribute]
        RemoveValuesIfTheyAreNotReferenced = 1,

        /// <summary>
        /// Force removal of field values on cascaded removal of
        /// parent object.
        /// </summary>
        [EnumMemberAttribute]
        RemoveValuesForcibly = 2

    }

}