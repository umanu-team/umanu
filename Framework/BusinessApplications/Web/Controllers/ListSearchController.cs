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
    using Framework.Presentation;
    using Framework.Presentation.Web.Controllers;
    using System;

    /// <summary>
    /// HTTP controller for responding dynamic files based on list
    /// table views and search queries.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public abstract class ListSearchController<T> : FileController where T : class, IProvidableObject {

        /// <summary>
        /// Absolute URL of dynamic file - it may not be empty, not
        /// contain any special charaters except for dashes and has
        /// to start with a slash.
        /// </summary>
        public string AbsoluteUrl {
            get {
                return this.absoluteUrl;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of dynamic file does not start with a slash.");
                }
                this.absoluteUrl = value;
            }
        }
        private string absoluteUrl;

        /// <summary>
        /// Business application to process.
        /// </summary>
        public IBusinessApplication BusinessApplication { get; private set; }

        /// <summary>
        /// Data controller for dynamic file.
        /// </summary>
        public ListTableDataController<T> ListTableDataController { get; private set; }

        /// <summary>
        /// Controller for search.
        /// </summary>
        public ISearchController<T> SearchController { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of dynamic file -
        /// it may not be empty, not contain any special charaters
        /// except for dashes and has to start with a slash</param>
        /// <param name="listTableDataController">data controller for
        /// dynamic file</param>
        /// <param name="searchController">controller for search</param>
        public ListSearchController(IBusinessApplication businessApplication, string absoluteUrl, ListTableDataController<T> listTableDataController, ISearchController<T> searchController)
            : base(CacheControl.NoStore, false) {
            this.AbsoluteUrl = absoluteUrl;
            this.BusinessApplication = businessApplication;
            this.ListTableDataController = listTableDataController;
            this.SearchController = searchController;
        }

        /// <summary>
        /// Updates the parent persistent object of a file for a
        /// specific URL.
        /// </summary>
        /// <param name="url">URL of file to update parent persistent
        /// object for</param>
        /// <returns>true on success, false otherwise</returns>
        protected override bool UpdateParentPersistentObjectOfFile(Uri url) {
            return false;
        }

    }

}