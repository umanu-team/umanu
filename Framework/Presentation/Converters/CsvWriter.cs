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

namespace Framework.Presentation.Converters {

    using Forms;
    using Persistence;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Converter for presentable objects to CSV format.
    /// </summary>
    public class CsvWriter {

        /// <summary>
        /// Line break character(s) to be useed.
        /// </summary>
        public string NewLine { get; set; }

        /// <summary>
        /// Data provider to use for option providers.
        /// </summary>
        public IOptionDataProvider OptionDataProvider { get; private set; }

        /// <summary>
        /// CSV value separator to be used.
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        /// List view to be applied for field mapping.
        /// </summary>
        public IListTableView View { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="view">list view to be applied for field
        /// mapping</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        public CsvWriter(IListTableView view, IOptionDataProvider optionDataProvider) {
            this.NewLine = Environment.NewLine;
            this.OptionDataProvider = optionDataProvider;
            this.Separator = ';';
            this.View = view;
        }

        /// <summary>
        /// Gets a CSV builder for presentable objects to be
        /// converted to CSV.
        /// </summary>
        /// <param name="presentableObjects">presentable objects to
        /// be converted</param>
        /// <returns>CSV builder for presentable objects to be
        /// converted to CSV</returns>
        private CsvBuilder CreateCsvBuilderFor(IEnumerable<IPresentableObject> presentableObjects) {
            var csvBuilder = new CsvBuilder(this.NewLine, this.Separator);
            this.WriteCsvHead(csvBuilder);
            foreach (var item in presentableObjects) {
                csvBuilder.AppendNewLine();
                this.WriteCsvLine(csvBuilder, item);
            }
            return csvBuilder;
        }

        /// <summary>
        /// Writes a CSV head.
        /// </summary>
        /// <param name="csvBuilder">CSV builder to write to</param>
        private void WriteCsvHead(CsvBuilder csvBuilder) {
            foreach (var viewField in this.View.ViewFields) {
                csvBuilder.AppendValue(viewField.Title);
            }
            return;
        }

        /// <summary>
        /// Writes the CSV line.
        /// </summary>
        /// <param name="csvBuilder">CSV builder to write to</param>
        /// <param name="item">item to write data for</param>
        private void WriteCsvLine(CsvBuilder csvBuilder, IPresentableObject item) {
            foreach (var viewField in this.View.ViewFields) {
                if (viewField.IsVisible) {
                    IPresentableField presentableField;
                    if (viewField is ViewFieldForEditableValue viewFieldForEditableValue) {
                        presentableField = item.FindPresentableField(viewFieldForEditableValue.Key);
                    } else {
                        presentableField = null;
                    }
                    if (null != presentableField) {
                        csvBuilder.AppendValue(viewField.GetReadOnlyValueFor(presentableField, item, this.OptionDataProvider));
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Returns a CSV file that represents the current object.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <param name="presentableObjects">presentable objects to
        /// be converted</param>
        /// <returns>CSV file that represents the objects</returns>
        public File WriteFile(string name, IEnumerable<IPresentableObject> presentableObjects) {
            return this.CreateCsvBuilderFor(presentableObjects).ToFile(name);
        }

        /// <summary>
        /// Returns a CSV string that represents the current object.
        /// </summary>
        /// <param name="presentableObjects">presentable objects to
        /// be converted</param>
        /// <returns>CSV string that represents the objects</returns>
        public string WriteString(IEnumerable<IPresentableObject> presentableObjects) {
            return this.CreateCsvBuilderFor(presentableObjects).ToString();
        }

    }

}