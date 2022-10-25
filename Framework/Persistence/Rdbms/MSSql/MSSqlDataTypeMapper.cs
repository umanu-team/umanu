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

namespace Framework.Persistence.Rdbms.MSSql {

    using Exceptions;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Globalization;

    /// <summary>
    /// Mapper for mapping .NET data types to data types of
    /// MS SQL Server.
    /// </summary>
    internal sealed class MSSqlDataTypeMapper : DataTypeMapper {

        /// <summary>
        /// Data type to map binary large objects to.
        /// </summary>
        public override string DataTypeForBlob {
            get { return "varbinary(max)"; }
        }

        /// <summary>
        /// Data type to map "bool" to.
        /// </summary>
        public override string DataTypeForBool {
            get { return "bit"; }
        }

        /// <summary>
        /// Data type to map "byte" to.
        /// </summary>
        public override string DataTypeForByte {
            get { return "tinyint"; }
        }

        /// <summary>
        /// Data type to map "char" to.
        /// </summary>
        public override string DataTypeForChar {
            get { return "nchar(1)"; }
        }

        /// <summary>
        /// Data type to map "decimal" to.
        /// </summary>
        public override string DataTypeForDecimal {
            get { return this.DataTypeForIndexableString; }
        }

        /// <summary>
        /// Data type to map "double" to.
        /// </summary>
        public override string DataTypeForDouble {
            get { return "float"; }
        }

        /// <summary>
        /// Data type to map "float" to.
        /// </summary>
        public override string DataTypeForFloat {
            get { return "real"; }
        }

        /// <summary>
        /// Data type to map "Guid" to.
        /// </summary>
        public override string DataTypeForGuid {
            get { return "uniqueidentifier"; }
        }

        /// <summary>
        /// Data type to map indexable "string" to.
        /// </summary>
        public override string DataTypeForIndexableString {
            get {
                return "nvarchar(4000)";
            }
        }

        /// <summary>
        /// Data type to map "int" to.
        /// </summary>
        public override string DataTypeForInt {
            get { return "int"; }
        }

        /// <summary>
        /// Data type to map "bool" to.
        /// </summary>
        public override string DataTypeForLong {
            get { return "bigint"; }
        }

        /// <summary>
        /// Data type to map "sbyte" to.
        /// </summary>
        public override string DataTypeForSByte {
            get { return this.DataTypeForShort; }
        }

        /// <summary>
        /// Data type to map "short" to.
        /// </summary>
        public override string DataTypeForShort {
            get { return "smallint"; }
        }

        /// <summary>
        /// Data type to map "string" to.
        /// </summary>
        public override string DataTypeForString {
            get { return "nvarchar(max)"; }
        }

        /// <summary>
        /// Data type to map "uint" to.
        /// </summary>
        public override string DataTypeForUInt {
            get { return "decimal(10,0)"; }
        }

        /// <summary>
        /// Data type to map "ulong" to.
        /// </summary>
        public override string DataTypeForULong {
            get { return "decimal(20,0)"; }
        }

        /// <summary>
        /// Data type to map "ushort" to.
        /// </summary>
        public override string DataTypeForUShort {
            get { return this.DataTypeForInt; }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public MSSqlDataTypeMapper()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Indicates whether two data types are equal.
        /// </summary>
        /// <param name="dataType1">first data type to compare</param>
        /// <param name="dataType2">second data type to compare</param>
        /// <returns>true if the two data types are equal, false
        /// otherwise</returns>
        public override bool AreDataTypesEqual(string dataType1, string dataType2) {
            var dataType1ToUpper = dataType1.ToUpperInvariant();
            var dataType2ToUpper = dataType2.ToUpperInvariant();
            bool areDataTypesEqual = dataType1ToUpper == dataType2ToUpper;
            if (!areDataTypesEqual) {
                var dataTypeForStringToUpper = this.DataTypeForString.ToUpperInvariant();
                areDataTypesEqual = (dataType1ToUpper == dataTypeForStringToUpper && dataType2ToUpper == "NVARCHAR")
                    || (dataType1ToUpper == "NVARCHAR" && dataType2ToUpper == dataTypeForStringToUpper);
            }
            if (!areDataTypesEqual) {
                var dataTypeForIndexableStringToUpper = this.DataTypeForIndexableString.ToUpperInvariant();
                areDataTypesEqual = (dataType1ToUpper == dataTypeForIndexableStringToUpper && dataType2ToUpper == "NVARCHAR")
                    || (dataType1ToUpper == "NVARCHAR" && dataType2ToUpper == dataTypeForIndexableStringToUpper);
            }
            return areDataTypesEqual;
        }

        /// <summary>
        /// Converts a value to be interted or updated into a
        /// persistence mechanism if necessary.
        /// </summary>
        /// <param name="value">value to be written</param>
        /// <param name="type">type of value</param>
        /// <returns>value converted to be written into a persistence
        /// mechnism</returns>
        protected override object ConvertPreWrite(object value, Type type) {
            value = base.ConvertPreWrite(value, type);
            if (DBNull.Value != value && null != value) {
                if (TypeOf.SByte == type || TypeOf.UShort == type || TypeOf.NullableSByte == type || TypeOf.NullableUShort == type) {
                    value = Convert.ToInt32(value, CultureInfo.InvariantCulture.NumberFormat);
                } else if (TypeOf.UInt == type || TypeOf.ULong == type || TypeOf.NullableUInt == type || TypeOf.NullableULong == type) {
                    value = Convert.ToDecimal(value, CultureInfo.InvariantCulture.NumberFormat);
                } else if (TypeOf.Decimal == type || TypeOf.NullableDecimal == type) {
                    value = Convert.ToDecimal(value, CultureInfo.InvariantCulture.NumberFormat).ToString(CultureInfo.InvariantCulture.NumberFormat);
                }
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
        public override DbParameter CreateDbParameter(string name, object value, Type type) {
            SqlDbType? dbType;
            if (TypeOf.ByteArray == type) {
                dbType = SqlDbType.VarBinary;
            } else if (TypeOf.Bool == type || TypeOf.NullableBool == type) {
                dbType = SqlDbType.Bit;
            } else if (TypeOf.Byte == type || TypeOf.NullableByte == type) {
                dbType = SqlDbType.TinyInt;
            } else if (TypeOf.Char == type || TypeOf.NullableChar == type) {
                dbType = SqlDbType.NChar;
            } else if (TypeOf.DateTime == type || TypeOf.NullableDateTime == type) {
                dbType = SqlDbType.BigInt;
            } else if (TypeOf.Decimal == type || TypeOf.NullableDecimal == type) {
                dbType = SqlDbType.NVarChar;
            } else if (TypeOf.Double == type || TypeOf.NullableDouble == type) {
                dbType = SqlDbType.Float;
            } else if (TypeOf.Float == type || TypeOf.NullableFloat == type) {
                dbType = SqlDbType.Real;
            } else if (TypeOf.Guid == type || TypeOf.NullableGuid == type) {
                dbType = SqlDbType.UniqueIdentifier;
            } else if (TypeOf.IndexableString == type) {
                dbType = SqlDbType.NVarChar;
            } else if (TypeOf.Int == type || TypeOf.NullableInt == type) {
                dbType = SqlDbType.Int;
            } else if (TypeOf.IUser == type) {
                dbType = SqlDbType.UniqueIdentifier;
            } else if (TypeOf.Long == type || TypeOf.NullableLong == type) {
                dbType = SqlDbType.BigInt;
            } else if (TypeOf.SByte == type || TypeOf.NullableSByte == type) {
                dbType = SqlDbType.SmallInt;
            } else if (TypeOf.Short == type || TypeOf.NullableShort == type) {
                dbType = SqlDbType.SmallInt;
            } else if (TypeOf.String == type) {
                dbType = SqlDbType.NVarChar;
            } else if (TypeOf.UInt == type || TypeOf.NullableUInt == type) {
                dbType = SqlDbType.Decimal;
            } else if (TypeOf.ULong == type || TypeOf.NullableULong == type) {
                dbType = SqlDbType.Decimal;
            } else if (TypeOf.UShort == type || TypeOf.NullableUShort == type) {
                dbType = SqlDbType.Int;
            } else if (null == type) {
                dbType = null;
            } else {
                throw new TypeException(".NET type " + type + " cannot be mapped to database type.");
            }
            SqlParameter parameter;
            if (dbType.HasValue) {
                parameter = new SqlParameter(name, dbType) {
                    Value = this.ConvertPreWrite(value, type)
                };
            } else {
                parameter = new SqlParameter(name, value);
            }
            return parameter;
        }

    }

}