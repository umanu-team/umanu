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

namespace Framework.BusinessApplications.DataProviders {

    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// Read-only data provider for SQL queries.
    /// </summary>
    /// <typeparam name="TConnection">type of connections</typeparam>
    public class SqlQueryDataProvider<TConnection> : ReadOnlyDataProvider<SqlQueryRecord>
        where TConnection : DbConnection, new() {

        /// <summary>
        /// Connection settings for accessing the relational
        /// database.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Command text for querying providable objects.
        /// </summary>
        public string CommandText { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="connectionString">connection settings for
        /// accessing the relational database</param>
        /// <param name="commandText">command text for querying
        /// providable objects</param>
        public SqlQueryDataProvider(string connectionString, string commandText)
            : base() {
            this.ConnectionString = connectionString;
            this.CommandText = commandText;
        }

        /// <summary>
        /// Finds all matching results of this data provider.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>all matching objects from this container</returns>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <returns>matching objects from this data provider</returns>
        public override ICollection<SqlQueryRecord> Find(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            var sqlQueryRecords = new List<SqlQueryRecord>();
            using (var connection = new TConnection()) {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand()) {
                    command.CommandText = this.CommandText;
                    using (var dataReader = command.ExecuteReader()) {
                        while (dataReader.Read()) {
                            var sqlQueryRecord = new SqlQueryRecord();
                            for (int i = 0; i < dataReader.FieldCount; i++) {
                                string key = dataReader.GetName(i);
                                if (TypeOf.DateTime == dataReader.GetFieldType(i)) {
                                    sqlQueryRecord.AddOrUpdatePresentableField(new PresentableFieldForDateTime(sqlQueryRecord, key, dataReader.GetDateTime(i)));
                                } else {
                                    sqlQueryRecord.AddOrUpdatePresentableField(new PresentableFieldForString(sqlQueryRecord, key, dataReader.GetValue(i).ToString()));
                                }
                                sqlQueryRecord.SqlKeys.Add(key);
                            }
                            sqlQueryRecords.Add(sqlQueryRecord);
                        }
                    }
                }
            }
            DataProvider<SqlQueryRecord>.FilterSortAndSubsetList(sqlQueryRecords, filterCriteria, sortCriteria, startPosition, maxResults);
            return sqlQueryRecords;
        }

        /// <summary>
        /// Finds the object with a specific ID.
        /// </summary>
        /// <param name="id">ID to get object for</param>
        /// <returns>object with specific ID or null if no
        /// match was found</returns>
        public override SqlQueryRecord FindOne(string id) {
            return null;
        }

        /// <summary>
        /// Finds the file with a specific ID and file name.
        /// </summary>
        /// <param name="fileId">ID to get file for</param>
        /// <param name="fileName">name of requested file</param>
        /// <returns>file with specific ID and file name</returns>
        public override File FindFile(System.Guid fileId, string fileName) {
            return null;
        }

        /// <summary>
        /// Preloads the state of multiple objects.
        /// </summary>
        /// <param name="elements">objects to preload</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        public override void Preload(IEnumerable<SqlQueryRecord> elements, IEnumerable<string[]> keyChains) {
            // nothing to do
            return;
        }

    }

}