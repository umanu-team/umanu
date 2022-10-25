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

namespace Framework.Presentation.Web.Controllers {

    using Persistence;
    using System;

    /// <summary>
    /// HTTP controller for responding a local file.
    /// </summary>
    public class LocalFileController : FileController {

        /// <summary>
        /// Path and name of file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fileName">path and name of file</param>
        /// <param name="cacheControl">indicates how local caching of
        /// files on client-side is supposed to be handled</param>
        public LocalFileController(string fileName, CacheControl cacheControl)
            : base(cacheControl, true) {
            this.FileName = fileName;
        }

        /// <summary>
        /// Finds the file for a specific URL.
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <returns>file for URL or null</returns>
        protected override File FindFile(Uri url) {
            File file;
            if (url.AbsolutePath == this.FileName) {
                file = new File(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, this.FileName);
            } else {
                file = null;
            }
            return file;
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