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

    using System;
    using Framework.Persistence;
    using Framework.Persistence.Fields;

    /// <summary>
    /// Represents a version.
    /// </summary>
    public sealed class Version : PersistentObject {

        /// <summary>
        /// ID of source object.
        /// </summary>
        public Guid SourceId {
            get { return this.sourceId.Value; }
            private set { this.sourceId.Value = value; }
        }
        private readonly PersistentFieldForGuid sourceId =
            new PersistentFieldForGuid(nameof(SourceId));

        /// <summary>
        /// Versioned value.
        /// </summary>
        public PersistentObject Value { get; internal set; }

        /// <summary>
        /// ID of value object.
        /// </summary>
        public Guid ValueId {
            get { return this.valueId.Value; }
            private set { this.valueId.Value = value; }
        }
        private readonly PersistentFieldForGuid valueId =
            new PersistentFieldForGuid(nameof(ValueId));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Version()
            : base() {
            this.sourceId.IsIndexed = true;
            this.RegisterPersistentField(this.sourceId);
            this.RegisterPersistentField(this.valueId);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="sourceId">ID of source object</param>
        /// <param name="versionId">ID of version object</param>
        internal Version(Guid sourceId, Guid versionId)
            : this() {
            this.SourceId = sourceId;
            this.ValueId = versionId;
        }

    }

}