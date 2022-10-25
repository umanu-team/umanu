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

namespace Framework.Persistence.Rdbms.PostgreSql {

    using Exceptions;
    using Npgsql;
    using NpgsqlTypes;
    using System;
    using System.Data.Common;
    using System.Globalization;

    /// <summary>
    /// Mapper for mapping .NET data types to data types of
    /// PostgreSQL Server.
    /// </summary>
    internal sealed class PostgreSqlDataTypeMapper : DataTypeMapper {

        /// <summary>
        /// Data type to map binary large objects to.
        /// </summary>
        public override string DataTypeForBlob {
            get { return "bytea"; }
        }

        /// <summary>
        /// Data type to map "bool" to.
        /// </summary>
        public override string DataTypeForBool {
            get { return "boolean"; }
        }

        /// <summary>
        /// Data type to map "byte" to.
        /// </summary>
        public override string DataTypeForByte {
            get { return this.DataTypeForShort; }
        }

        /// <summary>
        /// Data type to map "char" to.
        /// </summary>
        public override string DataTypeForChar {
            get { return "character varying(1)"; }  // char(1) is not supported by nqgsql
        }

        /// <summary>
        /// Data type to map "decimal" to.
        /// </summary>
        public override string DataTypeForDecimal {
            get { return "numeric"; }
        }

        /// <summary>
        /// Data type to map "double" to.
        /// </summary>
        public override string DataTypeForDouble {
            get { return "double precision"; }
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
            get { return "uuid"; }
        }

        /// <summary>
        /// Data type to map "int" to.
        /// </summary>
        public override string DataTypeForInt {
            get { return "integer"; }
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
            get { return "text"; }
        }

        /// <summary>
        /// Data type to map "uint" to.
        /// </summary>
        public override string DataTypeForUInt {
            get { return this.DataTypeForLong; }
        }

        /// <summary>
        /// Data type to map "ulong" to.
        /// </summary>
        public override string DataTypeForULong {
            get { return "numeric(20,0)"; }
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
        public PostgreSqlDataTypeMapper()
            : base() {
            // nothing to do
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
                } else if (TypeOf.Char == type) {
                    if (default(char) == (char)value) {
                        value = string.Empty;
                    } else {
                        value = value.ToString();
                    }
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
            NpgsqlDbType? dbType;
            if (TypeOf.ByteArray == type) {
                dbType = NpgsqlDbType.Bytea;
            } else if (TypeOf.Bool == type || TypeOf.NullableBool == type) {
                dbType = NpgsqlDbType.Boolean;
            } else if (TypeOf.Byte == type || TypeOf.NullableByte == type) {
                dbType = NpgsqlDbType.Smallint;
            } else if (TypeOf.Char == type || TypeOf.NullableChar == type) {
                dbType = NpgsqlDbType.Varchar;
            } else if (TypeOf.DateTime == type || TypeOf.NullableDateTime == type) {
                dbType = NpgsqlDbType.Bigint;
            } else if (TypeOf.Decimal == type || TypeOf.NullableDecimal == type) {
                dbType = NpgsqlDbType.Numeric;
            } else if (TypeOf.Double == type || TypeOf.NullableDouble == type) {
                dbType = NpgsqlDbType.Double;
            } else if (TypeOf.Float == type || TypeOf.NullableFloat == type) {
                dbType = NpgsqlDbType.Real;
            } else if (TypeOf.Guid == type || TypeOf.NullableGuid == type) {
                dbType = NpgsqlDbType.Uuid;
            } else if (TypeOf.IndexableString == type) {
                dbType = NpgsqlDbType.Text;
            } else if (TypeOf.Int == type || TypeOf.NullableInt == type) {
                dbType = NpgsqlDbType.Integer;
            } else if (TypeOf.IUser == type) {
                dbType = NpgsqlDbType.Uuid;
            } else if (TypeOf.Long == type || TypeOf.NullableLong == type) {
                dbType = NpgsqlDbType.Bigint;
            } else if (TypeOf.SByte == type || TypeOf.NullableSByte == type) {
                dbType = NpgsqlDbType.Smallint;
            } else if (TypeOf.Short == type || TypeOf.NullableShort == type) {
                dbType = NpgsqlDbType.Smallint;
            } else if (TypeOf.String == type) {
                dbType = NpgsqlDbType.Text;
            } else if (TypeOf.UInt == type || TypeOf.NullableUInt == type) {
                dbType = NpgsqlDbType.Bigint;
            } else if (TypeOf.ULong == type || TypeOf.NullableULong == type) {
                dbType = NpgsqlDbType.Numeric;
            } else if (TypeOf.UShort == type || TypeOf.NullableUShort == type) {
                dbType = NpgsqlDbType.Integer;
            } else if (null == type) {
                dbType = null;
            } else {
                throw new TypeException(".NET type " + type + " cannot be mapped to database type.");
            }
            NpgsqlParameter parameter;
            if (dbType.HasValue) {
                parameter = new NpgsqlParameter(name, dbType) {
                    Value = this.ConvertPreWrite(value, type)
                };
            } else {
                parameter = new NpgsqlParameter(name, value);
            }
            return parameter;
        }

    }

}