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

    using Framework.Diagnostics;
    using Framework.Persistence.Directories;
    using Npgsql;
    using System;

    /// <summary>
    /// Represents a PostgreSQL database.
    /// </summary>
    public class PostgreSqlDatabase : RelationalDatabase<NpgsqlConnection> {

        /// <summary>
        /// True if this persistentence mechanism has native support
        /// for byte values, false otherwise.
        /// </summary>
        protected internal sealed override bool HasNativeSupportForByte {
            get { return false; }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="connectionString">connection settings for
        /// accessing the SQL database</param>
        /// <param name="userDirectory">directory to use for user
        /// resolval</param>
        /// <param name="securityModel">security model to use for
        /// accessing this persistence mechanism</param>
        public PostgreSqlDatabase(string connectionString, UserDirectory userDirectory, SecurityModel securityModel)
            : base(new PostgreSqlDatabaseConnector(connectionString), userDirectory, securityModel) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="connectionString">connection settings for
        /// accessing the SQL database</param>
        /// <param name="userDirectory">directory to use for user
        /// resolval</param>
        /// <param name="securityModel">security model to use for
        /// accessing this persistence mechanism</param>
        /// <param name="log">log to use</param>
        public PostgreSqlDatabase(string connectionString, UserDirectory userDirectory, SecurityModel securityModel, ILog log)
            : base(new PostgreSqlDatabaseConnector(connectionString, log), userDirectory, securityModel) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="connectionString">connection settings for
        /// accessing the SQL database</param>
        /// <param name="userDirectory">directory to use for user
        /// resolval</param>
        /// <param name="securityModel">security model to use for
        /// accessing this persistence mechanism</param>
        /// <param name="versioningRepository">repository for
        /// versioning</param>
        public PostgreSqlDatabase(string connectionString, UserDirectory userDirectory, SecurityModel securityModel, PersistenceMechanism versioningRepository)
            : this(connectionString, userDirectory, securityModel) {
            this.VersioningRepository = versioningRepository;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="connectionString">connection settings for
        /// accessing the SQL database</param>
        /// <param name="userDirectory">directory to use for user
        /// resolval</param>
        /// <param name="securityModel">security model to use for
        /// accessing this persistence mechanism</param>
        /// <param name="versioningRepository">repository for
        /// versioning</param>
        /// <param name="log">log to use</param>
        public PostgreSqlDatabase(string connectionString, UserDirectory userDirectory, SecurityModel securityModel, PersistenceMechanism versioningRepository, ILog log)
            : this(connectionString, userDirectory, securityModel, log) {
            this.VersioningRepository = versioningRepository;
        }

        /// <summary>
        /// Adds or updates an object in full-text table.
        /// </summary>
        /// <param name="persistentObject">persistent object ot add
        /// or update in full-text table</param>
        protected sealed override void AddOrUpdateObjectInFullTextTable(PersistentObject persistentObject) {
            this.AddOrUpdateObjectInFullTextTable(persistentObject, false);
            return;
        }

        /// <summary>
        /// Copys this persistence mechanism and sets the security
        /// model of the copied instance to apply permissions. This
        /// method does not cache the copied persistence mechanism.
        /// </summary>
        /// <returns>copy of this persistence mechanism that applies
        /// all permissions</returns>
        protected override PersistenceMechanism CopyWithCurrentUserPrivilegesNoCache() {
            var restrictedCopy = new PostgreSqlDatabase(this.ConnectionString, this.UserDirectory, SecurityModel.ApplyPermissions, this.VersioningRepository, this.db.DbCommandLogger.Log);
            restrictedCopy.CopyPartiallyFrom(this);
            return restrictedCopy;
        }

        /// <summary>
        /// Copys this persistence mechanism and sets the security
        /// model of the copied instance to ignore permissions. This
        /// method does not cache the copied elevated persistence
        /// mechanism.
        /// </summary>
        /// <returns>copy of this persistence mechanism that ignores
        /// all permissions</returns>
        protected internal override PersistenceMechanism CopyWithElevatedPrivilegesNoCache() {
            var elevatedCopy = new PostgreSqlDatabase(this.ConnectionString, this.UserDirectory, SecurityModel.IgnorePermissions, this.VersioningRepository, this.db.DbCommandLogger.Log);
            elevatedCopy.CopyPartiallyFrom(this);
            return elevatedCopy;
        }

        /// <summary>
        /// Copys this persistence mechanism but replaces the user
        /// directory. This method does not cache the copied
        /// persistence mechanism.
        /// </summary>
        /// <param name="userDirectory">user directory to associate
        /// to copied persistence mechanism</param>
        /// <returns>copy of this persistence mechanism with replaced
        /// user directory</returns>
        public override PersistenceMechanism CopyWithReplacedUserDirectoryNoCache(UserDirectory userDirectory) {
            var modifiedCopy = new PostgreSqlDatabase(this.ConnectionString, userDirectory, this.SecurityModel, this.VersioningRepository, this.db.DbCommandLogger.Log);
            modifiedCopy.CopyPartiallyFrom(this);
            return modifiedCopy;
        }

        /// <summary>
        /// Generates a new internal table name for a type.
        /// </summary>
        /// <param name="type">type to generate internal table name
        /// for</param>
        /// <returns>generated internal table name for type</returns>
        protected sealed override string GenerateNewInternalTableNameFor(Type type) {
            string tableName;
            if (type.FullName.Length > 36) { // 63 (max length of table names) - 2 ("O_") - 25 ("_" followed by 24 characters for field names) = 36
                tableName = base.GenerateNewInternalTableNameFor(type);
            } else {
                tableName = "O_" + type.FullName.Replace('.', '_').Replace('`', '_');
            }
            return tableName;
        }

    }

}