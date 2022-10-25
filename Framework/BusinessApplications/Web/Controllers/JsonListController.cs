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

namespace Framework.BusinessApplications.Web.Controllers {

    using Framework.BusinessApplications.DataControllers;
    using Framework.Persistence;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Presentation.Converters;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// HTTP controller for responding dynamic JSON files based on
    /// list table views.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class JsonListController<T> : ListFileController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Indicates whether fields with null/empty values are
        /// supposed to be written at all.
        /// </summary>
        public bool IsWritingEmptyValues { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of dynamic JSON
        /// file - it may not be empty, not contain any special
        /// charaters except for dashes and has to start with a slash</param>
        /// <param name="listTableDataController">data controller for
        /// dynamic JSON file</param>
        public JsonListController(IBusinessApplication businessApplication, string absoluteUrl, ListTableDataController<T> listTableDataController)
            : base(businessApplication, absoluteUrl, listTableDataController) {
            this.IsWritingEmptyValues = true;
        }

        /// <summary>
        /// Finds the file for a specific URL.
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <returns>file for URL or null</returns>
        protected override File FindFile(Uri url) {
            File dynamicFile = null;
            if (url.AbsolutePath == this.AbsoluteUrl) {
                var listView = this.ListTableDataController.GetListTableView();
                if (null != listView) {
                    var fileName = url.Segments[url.Segments.LongLength - 1];
                    var optionDataProvider = new OptionDataProvider(this.BusinessApplication.PersistenceMechanism);
                    var queryResolver = new SubsetQueryResolver<T>(url, this.ListTableDataController.DataProvider, ulong.MaxValue);
                    var providableObjects = queryResolver.FindProvidableObjects();
                    if (providableObjects.Count > 0) {
                        this.ListTableDataController.DataProvider.Preload(providableObjects, this.ListTableDataController.GetKeyChainsToPreloadForListTableView());
                    }
                    dynamicFile = this.GetFile(fileName, listView, providableObjects, optionDataProvider);
                }
            }
            return dynamicFile;
        }

        /// <summary>
        /// Gets the dynamic file to be responded.
        /// </summary>
        /// <param name="fileName">name of file</param>
        /// <param name="listView">list view to apply to data in file</param>
        /// <param name="providableObjects">preloaded providable
        /// objects to write into file if property
        /// IsApplyingSkipAndTop is set to true, all matching
        /// providable objects non-preloaded otherwise</param>
        /// <param name="optionDataProvider">provider to use for
        /// querying additional data</param>
        /// <returns>dynamic file to be responded</returns>
        protected virtual File GetFile(string fileName, IListTableView listView, ICollection<T> providableObjects, OptionDataProvider optionDataProvider) {
            var jsonWriter = new JsonWriter(listView, optionDataProvider, this.BusinessApplication.FileBaseDirectory) {
                IsWritingEmptyValues = this.IsWritingEmptyValues
            };
            return jsonWriter.WriteFile(fileName, providableObjects);
        }

    }

}