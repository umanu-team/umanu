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

    using Framework.Persistence.Directories;
    using Framework.Presentation.Forms;
    using System;
    using System.Data.Common;

    /// <summary>
    /// Wrapper class for a field of type IUser to be stored in
    /// persistence mechanism.
    /// </summary>
    public sealed class PersistentFieldForIUser : PersistentFieldForElement<IUser>, IPresentableFieldForIUser {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.IUser;
            }
        }

        /// <summary>
        /// Indicates whether value from DbDataReader is set already.
        /// </summary>
        private bool? isValueFromDbDataReaderSet;

        /// <summary>
        /// Directory to use for user resolval.
        /// </summary>
        public UserDirectory UserDirectory {
            get {
                if (null == this.userDirectory) {
                    this.userDirectory = this.ParentPersistentObject.ParentPersistentContainer.ParentPersistenceMechanism.UserDirectory;
                }
                return this.userDirectory;
            }
            set {
                this.userDirectory = value;
            }
        }
        private UserDirectory userDirectory;

        /// <summary>
        /// Value loaded from DbDataReader.
        /// </summary>
        private Guid? valueFromDbDataReader;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        public PersistentFieldForIUser(string key)
            : base(key) {
            this.userDirectory = null;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        /// <param name="value">value of persistent field</param>
        public PersistentFieldForIUser(string key, IUser value)
            : this(key) {
            this.Value = value;
        }

        /// <summary>
        /// Ensures that this field was retrieved from persistence at
        /// least once.
        /// </summary>
        protected override void EnsureRetrieval() {
            base.EnsureRetrieval();
            if (false == this.isValueFromDbDataReaderSet) {
                this.isValueFromDbDataReaderSet = true;
                if (null == this.valueFromDbDataReader || null == this.ParentPersistentObject?.ParentPersistentContainer) {
                    this.SetValueUnsafe(null);
                } else if (Guid.Empty == this.valueFromDbDataReader.Value) {
                    this.SetValueUnsafe(UserDirectory.AnonymousUser);
                } else {
                    this.SetValueUnsafe(this.UserDirectory.FindOne(this.valueFromDbDataReader.Value, nameof(IUser.DisplayName), nameof(IUser.UserName)));
                }
                this.valueFromDbDataReader = null;
            }
            return;
        }

        /// <summary>
        /// Converts the value of this field to a string value.
        /// </summary>
        /// <returns>value of this field as string</returns>
        protected internal override string GetValueAsString() {
            return this.Value?.UserName;
        }

        /// <summary>
        /// Loads a new value for persistent field from a specified
        /// index of a DbDataReader, but does not set the value yet.
        /// </summary>
        /// <param name="dataReader">data reader to load value from</param>
        /// <param name="ordinal">index of data reader to load value
        /// from</param>
        internal override void LoadValueFromDbDataReader(DbDataReader dataReader, int ordinal) {
            if (dataReader.IsDBNull(ordinal)) {
                this.valueFromDbDataReader = null;
            } else {
                this.valueFromDbDataReader = dataReader.GetGuid(ordinal);
            }
            return;
        }

        /// <summary>
        /// Sets the new value for persistent field which has been
        /// loaded from DbDataReader before.
        /// </summary>
        internal override void SetValueFromDbDataReader() {
            this.isValueFromDbDataReaderSet = false;
            return;
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of value of this field and sets it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">new value to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public override bool TrySetValueAsString(string value) {
            this.Value = this.UserDirectory.FindOneByVagueTerm(value, FilterScope.UserName);
            return string.IsNullOrEmpty(value) || null != this.Value;
        }

        /// <summary>
        /// Determines whether two value are different.
        /// </summary>
        /// <param name="x">first value to compare</param>
        /// <param name="y">second value to compare</param>
        /// <returns>true if the specified values are different,
        /// false otherwise</returns>
        protected override bool ValuesAreDifferent(IUser x, IUser y) {
            return x?.Id != y?.Id;
        }

    }

}