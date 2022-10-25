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

namespace Framework.Presentation.Forms {

    using Framework.Persistence.Fields;
    using Framework.Properties;
    using Persistence;
    using System;
    using System.Web;

    /// <summary>
    /// Field for single file to be presented in a view.
    /// </summary>
    public class ViewFieldForFile : ViewFieldForElement {

        /// <summary>
        /// List of MIME types to accept, e.g. ".doc", "application/msword",
        /// "application/vnd.openxmlformats-officedocument.wordprocessingml.document".
        /// </summary>
        public PersistentFieldForStringCollection AcceptedMimeTypes { get; private set; }

        /// <summary>
        /// Maximum size of an uploaded file, in bytes.
        /// </summary>
        public int MaxFileSize {
            get { return this.maxFileSize.Value; }
            set { this.maxFileSize.Value = value; }
        }
        private readonly PersistentFieldForInt maxFileSize =
            new PersistentFieldForInt(nameof(MaxFileSize), 2000000000); // 2 GB (not 2 GiB)

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForFile()
            : base() {
            this.AcceptedMimeTypes = new PersistentFieldForStringCollection(nameof(this.AcceptedMimeTypes));
            this.RegisterPersistentField(this.AcceptedMimeTypes);
            this.RegisterPersistentField(this.maxFileSize);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForFile(string title, string key, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.Key = key;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForFile(string title, string[] keyChain, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        private ViewFieldForFile(string title, Mandatoriness mandatoriness)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
        }

        /// <summary>
        /// Creates a presentable field that can hold the value of
        /// view field.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of created field</param>
        /// <returns>presentable field that can hold the value of
        /// view field</returns>
        public override IPresentableFieldForElement CreatePresentableField(IPresentableObject parentPresentableObject) {
            return new PresentableFieldForObject(parentPresentableObject, this.Key);
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage;
            decimal maxFileSizeInMiB = Math.Round(this.MaxFileSize / 1048576m);
            if (this.AcceptedMimeTypes.Count < 1) {
                errorMessage = string.Format(Resources.PleaseSelectAFileWithAMaximumFileSizeOf0MB, maxFileSizeInMiB);
            } else {
                errorMessage = string.Format(Resources.PleaseSelectAFileOfAnAllowedTypeWithAMaximumFileSizeOf0MB, maxFileSizeInMiB);
            }
            errorMessage += ' ' + this.GetInfoMessageAboutManditoriness();
            return errorMessage;
        }

        /// <summary>
        /// Returns null if the specified value is valid, an error
        /// message otherwise.
        /// </summary>
        /// <param name="file">file to be validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <returns>null if the specified value is valid, error
        /// message otherwise</returns>
        public string Validate(HttpPostedFile file, ValidityCheck validityCheck) {
            string errorMessage = null;
            if (null == file || file.ContentLength < 1) {
                bool isMandatory = Mandatoriness.Required == this.Mandatoriness || (ValidityCheck.Strict == validityCheck && Mandatoriness.Desired == this.Mandatoriness);
                if (isMandatory) {
                    errorMessage = this.GetDefaultErrorMessage();
                }
            } else {
                if (this.AcceptedMimeTypes.Count > 0) {
                    string fileContentTypeToUpper = file.ContentType.ToUpperInvariant();
                    string fileNameToUpper = file.FileName.ToUpperInvariant();
                    bool isMatchingAcceptedTypes = false;
                    foreach (string acceptedMimeType in this.AcceptedMimeTypes) {
                        string acceptedMimeTypeToUpper = acceptedMimeType.ToUpperInvariant();
                        if (acceptedMimeTypeToUpper == fileContentTypeToUpper
                            || (acceptedMimeTypeToUpper.EndsWith("*", StringComparison.Ordinal) && acceptedMimeTypeToUpper.Length > 1 && fileContentTypeToUpper.StartsWith(acceptedMimeTypeToUpper.Substring(0, acceptedMimeTypeToUpper.Length - 1), StringComparison.Ordinal))
                            || (acceptedMimeTypeToUpper.StartsWith(".", StringComparison.Ordinal) && fileNameToUpper.EndsWith(acceptedMimeTypeToUpper, StringComparison.Ordinal))) {
                            isMatchingAcceptedTypes = true;
                            break;
                        }
                    }
                    if (!isMatchingAcceptedTypes) {
                        errorMessage = this.GetDefaultErrorMessage();
                    }
                }
                if (file.ContentLength > this.MaxFileSize) {
                    errorMessage = this.GetDefaultErrorMessage();
                } else {
                    string fileName = File.CleanName(file.FileName);
                    if (fileName.Contains("<")
                        || fileName.Contains(">")
                        || fileName.Contains("*")
                        || fileName.Contains("%")
                        || fileName.Contains("&")
                        || fileName.Contains(":")
                        || fileName.Contains("\\")) {
                        errorMessage = Resources.PleaseMakeSureFileNamesDoNotContainAnyOfTheFollowingCharacters + ' ' + this.GetInfoMessageAboutManditoriness();
                    }
                }
            }
            return errorMessage;
        }

    }

}