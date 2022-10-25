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

namespace Framework.Persistence {

    using Framework.Model;
    using Framework.Persistence.Exceptions;
    using System;
    using System.Data.Common;

    /// <summary>
    /// Mapper for mapping .NET data types to data types of
    /// persistence mechnanisms.
    /// </summary>
    public abstract class DataTypeMapper {

        /// <summary>
        /// Gets the mapping as string for a .NET data type.
        /// </summary>
        public string this[Type type] {
            get {
                return this.GetMappingForType(type);
            }
        }

        /// <summary>
        /// Data type to map binary large objects to.
        /// </summary>
        public abstract string DataTypeForBlob { get; }

        /// <summary>
        /// Data type to map "bool" to.
        /// </summary>
        public abstract string DataTypeForBool { get; }

        /// <summary>
        /// Data type to map "byte" to.
        /// </summary>
        public abstract string DataTypeForByte { get; }

        /// <summary>
        /// Data type to map "char" to.
        /// </summary>
        public abstract string DataTypeForChar { get; }

        /// <summary>
        /// Data type to map "DateTime" to.
        /// </summary>
        public virtual string DataTypeForDateTime {
            get {
                return this.DataTypeForLong;
            }
        }

        /// <summary>
        /// Data type to map "decimal" to.
        /// </summary>
        public abstract string DataTypeForDecimal { get; }

        /// <summary>
        /// Data type to map "double" to.
        /// </summary>
        public abstract string DataTypeForDouble { get; }

        /// <summary>
        /// Data type to map "float" to.
        /// </summary>
        public abstract string DataTypeForFloat { get; }

        /// <summary>
        /// Data type to map "Guid" to.
        /// </summary>
        public abstract string DataTypeForGuid { get; }

        /// <summary>
        /// Data type to map indexable "string" to.
        /// </summary>
        public virtual string DataTypeForIndexableString {
            get {
                return this.DataTypeForString;
            }
        }

        /// <summary>
        /// Data type to map "int" to.
        /// </summary>
        public abstract string DataTypeForInt { get; }

        /// <summary>
        /// Data type to map "IUser" to.
        /// </summary>
        public virtual string DataTypeForIUser {
            get {
                return this.DataTypeForGuid;
            }
        }

        /// <summary>
        /// Data type to map "long" to.
        /// </summary>
        public abstract string DataTypeForLong { get; }

        /// <summary>
        /// Data type to map "sbyte" to.
        /// </summary>
        public abstract string DataTypeForSByte { get; }

        /// <summary>
        /// Data type to map "short" to.
        /// </summary>
        public abstract string DataTypeForShort { get; }

        /// <summary>
        /// Data type to map "string" to.
        /// </summary>
        public abstract string DataTypeForString { get; }

        /// <summary>
        /// Data type to map "uint" to.
        /// </summary>
        public abstract string DataTypeForUInt { get; }

        /// <summary>
        /// Data type to map "ulong" to.
        /// </summary>
        public abstract string DataTypeForULong { get; }

        /// <summary>
        /// Data type to map "ushort" to.
        /// </summary>
        public abstract string DataTypeForUShort { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public DataTypeMapper() {
            // nothing to do
        }

        /// <summary>
        /// Indicates whether two data types are equal.
        /// </summary>
        /// <param name="dataType1">first data type to compare</param>
        /// <param name="dataType2">second data type to compare</param>
        /// <returns>true if the two data types are equal, false
        /// otherwise</returns>
        public virtual bool AreDataTypesEqual(string dataType1, string dataType2) {
            var dataType1ToUpper = dataType1.ToUpperInvariant();
            var dataType2ToUpper = dataType2.ToUpperInvariant();
            return dataType1ToUpper == dataType2ToUpper;
        }

        /// <summary>
        /// Converts a value to be interted or updated into a
        /// persistence mechanism if necessary.
        /// </summary>
        /// <param name="value">value to be written</param>
        /// <param name="type">type of value</param>
        /// <returns>value converted to be written into a persistence
        /// mechnism</returns>
        protected virtual object ConvertPreWrite(object value, Type type) {
            if (null == value) {
                value = DBNull.Value;
            } else if (TypeOf.DateTime == type || TypeOf.NullableDateTime == type) {
                var dateTime = (DateTime)value;
                dateTime = UtcDateTime.ConvertToUniversalTime(dateTime);
                value = dateTime.Ticks;
            } else if (TypeOf.IUser == type) {
                var user = value as IUser;
                if (user.IsNew) {
                    throw new ObjectNotPersistentException("Link to user \"" + user.DisplayName + "\" cannot be saved because user is not stored in a directory.");
                }
                value = user.Id;
            }
            return value;
        }

        /// <summary>
        /// Creates a new database parameter.
        /// </summary>
        /// <param name="name">name of parameter</param>
        /// <param name="value">value of parameter</param>
        /// <param name="type">.NET data type</param>
        /// <returns>new database parameter</returns>
        public abstract DbParameter CreateDbParameter(string name, object value, Type type);

        /// <summary>
        /// Gets the mapping as string for a .NET data type.
        /// </summary>
        /// <param name="type">.NET data type</param>
        /// <returns>mapping as string</returns>
        public string GetMappingForType(Type type) {
            string mapping;
            if (TypeOf.ByteArray == type) {
                mapping = this.DataTypeForBlob;
            } else if (TypeOf.Bool == type || TypeOf.NullableBool == type) {
                mapping = this.DataTypeForBool;
            } else if (TypeOf.Byte == type || TypeOf.NullableByte == type) {
                mapping = this.DataTypeForByte;
            } else if (TypeOf.Char == type || TypeOf.NullableChar == type) {
                mapping = this.DataTypeForChar;
            } else if (TypeOf.DateTime == type || TypeOf.NullableDateTime == type) {
                mapping = this.DataTypeForDateTime;
            } else if (TypeOf.Decimal == type || TypeOf.NullableDecimal == type) {
                mapping = this.DataTypeForDecimal;
            } else if (TypeOf.Double == type || TypeOf.NullableDouble == type) {
                mapping = this.DataTypeForDouble;
            } else if (TypeOf.Float == type || TypeOf.NullableFloat == type) {
                mapping = this.DataTypeForFloat;
            } else if (TypeOf.Guid == type || TypeOf.NullableGuid == type) {
                mapping = this.DataTypeForGuid;
            } else if (TypeOf.IndexableString == type) {
                mapping = this.DataTypeForIndexableString;
            } else if (TypeOf.Int == type || TypeOf.NullableInt == type) {
                mapping = this.DataTypeForInt;
            } else if (TypeOf.IUser == type) {
                mapping = this.DataTypeForIUser;
            } else if (TypeOf.Long == type || TypeOf.NullableLong == type) {
                mapping = this.DataTypeForLong;
            } else if (TypeOf.SByte == type || TypeOf.NullableSByte == type) {
                mapping = this.DataTypeForSByte;
            } else if (TypeOf.Short == type || TypeOf.NullableShort == type) {
                mapping = this.DataTypeForShort;
            } else if (TypeOf.String == type) {
                mapping = this.DataTypeForString;
            } else if (TypeOf.UInt == type || TypeOf.NullableUInt == type) {
                mapping = this.DataTypeForUInt;
            } else if (TypeOf.ULong == type || TypeOf.NullableULong == type) {
                mapping = this.DataTypeForULong;
            } else if (TypeOf.UShort == type || TypeOf.NullableUShort == type) {
                mapping = this.DataTypeForUShort;
            } else {
                throw new TypeException(".NET type " + type +
                    " cannot be mapped to a type in persistence mechanism.");
            }
            return mapping;
        }

    }

}