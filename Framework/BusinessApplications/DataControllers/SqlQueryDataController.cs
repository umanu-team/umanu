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

namespace Framework.BusinessApplications.DataControllers {

    using Framework.BusinessApplications.DataProviders;
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// Master/Detail data controller for SQL queries.
    /// </summary>
    /// <typeparam name="TConnection">type of connections</typeparam>
    public class SqlQueryController<TConnection> : ListTableDataController<SqlQueryRecord>
        where TConnection : DbConnection, new() {

        /// <summary>
        /// Type of chart to generate.
        /// </summary>
        public ChartType ChartType { get; set; }

        /// <summary>
        /// Title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="chartType">type of chart to generate</param>
        /// <param name="connectionString">connection settings for
        /// accessing the relational database</param>
        /// <param name="commandText">command text for querying
        /// providable objects</param>
        public SqlQueryController(string title, ChartType chartType, string connectionString, string commandText)
            : base(new SqlQueryDataProvider<TConnection>(connectionString, commandText)) {
            this.ChartType = chartType;
            this.Title = title;
        }

        /// <summary>
        /// Edit form for existing items.
        /// </summary>
        /// <param name="element">object to get form view
        /// for</param>
        public override FormView GetEditFormView(SqlQueryRecord element) {
            return null;
        }

        /// <summary>
        /// Gets the buttons to show on list table.
        /// </summary>
        /// <returns>buttons to show on list table</returns>
        public override IEnumerable<ActionButton> GetListTableButtons() {
            yield break;
        }

        /// <summary>
        /// Gets the view to use for list table.
        /// </summary>
        /// <returns>view to use for list table</returns>
        public override IListTableView GetListTableView() {
            var listView = new ListTableView();
            listView.ChartType = this.ChartType;
            listView.Title = this.Title;
            foreach (var sqlQueryRecord in this.DataProvider.GetAll()) {
                foreach (string key in sqlQueryRecord.SqlKeys) {
                    var presentableField = sqlQueryRecord.FindPresentableField(key);
                    if (TypeOf.DateTime == presentableField.ContentBaseType) {
                        listView.ViewFields.Add(new ViewFieldForDateTime(presentableField.Key, presentableField.Key, Mandatoriness.Optional, TimeSpan.Zero, DateTimeType.DateAndTime));
                    } else {
                        listView.ViewFields.Add(new ViewFieldForSingleLineText(presentableField.Key, presentableField.Key, Mandatoriness.Optional));
                    }
                }
                break;
            }
            return listView;
        }

    }

}