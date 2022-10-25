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
    using System;
    using System.Web;

    /// <summary>
    /// Field for collection of files to be presented in a view.
    /// </summary>
    public class ViewFieldForMultipleFiles : ViewFieldForCollection {

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
            new PersistentFieldForInt(nameof(MaxFileSize), 1024 * 1048576);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForMultipleFiles()
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
        public ViewFieldForMultipleFiles(string title, string key, Mandatoriness mandatoriness)
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
        public ViewFieldForMultipleFiles(string title, string[] keyChain, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        private ViewFieldForMultipleFiles(string title, Mandatoriness mandatoriness)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage;
            decimal maxFileSizeInMB = Math.Round(this.MaxFileSize / 1048576m);
            if (this.Limit < 2) {
                if (this.AcceptedMimeTypes.Count < 1) {
                    errorMessage = string.Format(Resources.PleaseSelectAFileWithAMaximumFileSizeOf0MB, maxFileSizeInMB);
                } else {
                    errorMessage = string.Format(Resources.PleaseSelectAFileOfAnAllowedTypeWithAMaximumFileSizeOf0MB, maxFileSizeInMB);
                }
            } else {
                if (this.AcceptedMimeTypes.Count < 1) {
                    errorMessage = string.Format(Resources.PleaseSelectFilesWithAMaximumFileSizeOf0MB, maxFileSizeInMB);
                } else {
                    errorMessage = string.Format(Resources.PleaseSelectFilesOfAllowedTypesWithAMaximumFileSizeOf0MB, maxFileSizeInMB);
                }
                if (this.Limit < uint.MaxValue) {
                    errorMessage += ' ';
                    errorMessage += string.Format(Resources.UpTo0FilesAreAllowed, this.Limit);
                }
            }
            errorMessage += ' ' + this.GetInfoMessageAboutManditoriness();
            return errorMessage;
        }

        /// <summary>
        /// Gets the characters to use for separating values.
        /// </summary>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <returns>characters to use for separating values</returns>
        public override ValueSeparator GetValueSeparator(FieldRenderMode renderMode) {
            ValueSeparator valueSeparator;
            if (FieldRenderMode.Form == renderMode) {
                valueSeparator = ValueSeparator.LineBreak;
            } else {
                valueSeparator = ValueSeparator.Comma;
            }
            return valueSeparator;
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
            var viewFieldForFile = new ViewFieldForFile(this.Key, this.Title, Mandatoriness.Optional);
            viewFieldForFile.AcceptedMimeTypes.AddRange(this.AcceptedMimeTypes);
            viewFieldForFile.DescriptionForEditMode = this.DescriptionForEditMode;
            viewFieldForFile.DescriptionForViewMode = this.DescriptionForViewMode;
            viewFieldForFile.MaxFileSize = this.MaxFileSize;
            return viewFieldForFile.Validate(file, validityCheck);
        }

    }

}