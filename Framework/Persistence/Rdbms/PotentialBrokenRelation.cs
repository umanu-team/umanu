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

namespace Framework.Persistence.Rdbms {

    using System;

    /// <summary>
    /// Represents a potential broken relation.
    /// </summary>
    internal sealed class PotentialBrokenRelation {

        /// <summary>
        /// ID of child object of relation.
        /// </summary>
        public Guid ChildId { get; private set; }

        /// <summary>
        /// Type of child object of relation.
        /// </summary>
        public string ChildType { get; set; }

        /// <summary>
        /// ID of parent object of relation.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Table of parent object of relation.
        /// </summary>
        public string ParentTable { get; set; }

        /// <summary>
        /// ID of relation.
        /// </summary>
        public Guid RelationId { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="relationId">ID of relation</param>
        /// <param name="parentTable">table of parent object of
        /// relation</param>
        /// <param name="parentId">ID of parent object of relation</param>
        /// <param name="childType">type of child object of relation</param>
        /// <param name="childId">ID of child object of relation</param>
        public PotentialBrokenRelation(Guid relationId, string parentTable, Guid parentId, string childType, Guid childId)
            : base() {
            this.ChildId = childId;
            this.ChildType = childType;
            this.ParentId = parentId;
            this.ParentTable = parentTable;
            this.RelationId = relationId;
        }

    }

}