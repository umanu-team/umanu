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

namespace Framework.Persistence {

    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using System;
    using System.IO;
    using System.Web;

    /// <summary>
    /// Temporary file to be stored in persistence mechanism.
    /// </summary>
    public class TemporaryFile : File {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public TemporaryFile()
            : base() {
            this.AllowedGroupsCascadedRemovalBehavior = CascadedRemovalBehavior.RemoveValuesForcibly;
            this.AllowedGroups = new AllowedGroups();
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <param name="mimeType">MIME type of file</param>
        /// <param name="bytes">file contents as binary data</param>
        public TemporaryFile(string name, string mimeType, byte[] bytes)
            : this() {
            this.Name = name;
            this.MimeType = mimeType;
            this.Bytes = bytes;
        }

        /// <summary>
        /// Loads a temporary file.
        /// </summary>
        /// <param name="id">guid of temporary file as string</param>
        /// <param name="optionDataProvider">data provider to use for
        /// loading the file</param>
        /// <returns>temporary file with given ID or null</returns>
        public static TemporaryFile Load(string id, IOptionDataProvider optionDataProvider) {
            TemporaryFile temporaryFile;
            if (Guid.TryParse(id, out Guid guid)) {
                var filterCriteria = new FilterCriteria(nameof(TemporaryFile.Id), RelationalOperator.IsEqualTo, guid);
                var temporaryFiles = optionDataProvider.Find<TemporaryFile>(filterCriteria, SortCriterionCollection.Empty);
                if (1 == temporaryFiles.Count) {
                    temporaryFile = temporaryFiles[0];
                } else {
                    temporaryFile = null;
                }
            } else {
                temporaryFile = null;
            }
            return temporaryFile;
        }

        /// <summary>
        /// Saves a temporary file.
        /// </summary>
        /// <param name="httpPostedFile">posted back file to save as
        /// temporary file</param>
        /// <param name="optionDataProvider">data provider to use for
        /// saving the file</param>
        /// <returns>temporary file</returns>
        public static TemporaryFile Save(HttpPostedFile httpPostedFile, IOptionDataProvider optionDataProvider) {
            int fileSize = httpPostedFile.ContentLength;
            byte[] bytes = new byte[fileSize];
            using (Stream fileStream = httpPostedFile.InputStream) {
                fileStream.Read(bytes, 0, fileSize);
            }
            string fileName = TemporaryFile.CleanName(httpPostedFile.FileName);
            return TemporaryFile.Save(fileName, httpPostedFile.ContentType, bytes, optionDataProvider);
        }

        /// <summary>
        /// Saves a temporary file.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <param name="mimeType">MIME type of file</param>
        /// <param name="bytes">file contents as binary data</param>
        /// <param name="optionDataProvider">data provider to use for
        /// saving the file</param>
        /// <returns>temporary file</returns>
        public static TemporaryFile Save(string name, string mimeType, byte[] bytes, IOptionDataProvider optionDataProvider) {
            TemporaryFile temporaryFile;
            try {
                var imageFile = new ImageFile(name, bytes); // optimizes file and detects MIME type in a more accurate way
                temporaryFile = new TemporaryFile(imageFile.Name, imageFile.MimeType, imageFile.Bytes);
            } catch (ArgumentException) {
                temporaryFile = new TemporaryFile(name, mimeType, bytes);
            }
            var group = new Group(name);
            group.Members.Add(optionDataProvider.UserDirectory.CurrentUser);
            temporaryFile.AllowedGroups.ForReading.Add(group);
            temporaryFile.AllowedGroups.ForWriting.Add(group);
            optionDataProvider.AddTemporaryFile(temporaryFile);
            return temporaryFile;
        }

    }

}