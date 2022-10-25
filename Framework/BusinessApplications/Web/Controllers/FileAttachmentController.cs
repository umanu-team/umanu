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

    using Framework.Persistence;
    using Framework.Presentation;
    using Framework.Presentation.Web.Controllers;
    using Model;
    using System;

    /// <summary>
    /// HTTP controller for responding file attachments.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class FileAttachmentController<T> : FileController where T : class, IProvidableObject {

        /// <summary>
        /// Absolute URL of parent list page - it may not be empty,
        /// not contain any special charaters except for dashes and
        /// has to start and end with a slash.
        /// </summary>
        public string AbsoluteListPageUrl {
            get {
                return this.absoluteListPageUrl;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of list page does not start with a slash.");
                }
                if (!value.EndsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of list page does not end with a slash.");
                }
                this.absoluteListPageUrl = value;
            }
        }
        private string absoluteListPageUrl;

        /// <summary>
        /// Business application to process.
        /// </summary>
        public IBusinessApplication BusinessApplication { get; private set; }

        /// <summary>
        /// Data provider for files.
        /// </summary>
        public DataProvider<T> DataProvider { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteListTablePageUrl">absolute URL of
        /// parent list table page - it may not be empty, not contain
        /// any special charaters except for dashes and has to start
        /// and end with a slash</param>
        /// <param name="fileDataProvider">data provider for files</param>
        public FileAttachmentController(IBusinessApplication businessApplication, string absoluteListTablePageUrl, DataProvider<T> fileDataProvider)
            : base(CacheControl.NoRevalidation, businessApplication.PersistenceMechanism.UserDirectory.IsCurrentUserAnonymous) {
            this.AbsoluteListPageUrl = absoluteListTablePageUrl;
            this.BusinessApplication = businessApplication;
            this.DataProvider = fileDataProvider;
        }

        /// <summary>
        /// Finds the file for a specific URL.
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <returns>file for URL or null</returns>
        protected override File FindFile(Uri url) {
            File file = null;
            var urlSegments = url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.None);
            long absoluteListPageUrlSegmentsLength = this.AbsoluteListPageUrl.Split(new char[] { '/' }, StringSplitOptions.None).LongLength;
            if (absoluteListPageUrlSegmentsLength < urlSegments.LongLength && urlSegments[absoluteListPageUrlSegmentsLength] + '/' == this.BusinessApplication.FileBaseDirectory) {
                long urlSegmentsLength = urlSegments.LongLength;
                bool isImageFile = false;
                if (absoluteListPageUrlSegmentsLength + 3L == urlSegmentsLength || (absoluteListPageUrlSegmentsLength + 4L == urlSegmentsLength && (isImageFile = FileController.DecodeImageParameters(urlSegments[urlSegmentsLength - 2], out ResizeType _, out int _) || FileController.DecodeImageParameters(urlSegments[urlSegmentsLength - 2], out int _, out int _)))) {
                    if (Guid.TryParse(urlSegments[absoluteListPageUrlSegmentsLength + 1], out Guid fileId)) {
                        string fileName = Uri.UnescapeDataString(urlSegments[urlSegmentsLength - 1]);
                        file = this.DataProvider.FindFile(fileId, fileName);
                        if (null == file && isImageFile && fileName.EndsWith(".jpg", StringComparison.Ordinal)) {
                            file = this.DataProvider.FindFile(fileId, fileName.Substring(0, fileName.Length - 4)); // necessary because some browsers do not respect MIME types
                        }
                    }
                }
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
            bool success = false;
            var urlSegments = url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            long absoluteListPageUrlSegmentsLength = this.AbsoluteListPageUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LongLength;
            if (urlSegments.LongLength > absoluteListPageUrlSegmentsLength) {
                string parentPersistentObjectId = urlSegments[absoluteListPageUrlSegmentsLength];
                var parentPersistentObject = this.DataProvider.FindOne(parentPersistentObjectId) as PersistentObject;
                if (null != parentPersistentObject) {
                    var elevatedPersistentObject = parentPersistentObject.GetWithElevatedPrivileges();
                    if (!elevatedPersistentObject.IsChanged) {
                        elevatedPersistentObject.ModifiedAt = UtcDateTime.Now;
                        elevatedPersistentObject.Update();
                    }
                    success = true;
                }
            }
            return success;
        }

    }

}