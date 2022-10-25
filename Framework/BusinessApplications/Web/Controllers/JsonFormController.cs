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
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web.Controllers;
    using Persistence;
    using Presentation.Converters;
    using System;

    /// <summary>
    /// HTTP controller for responding dynamic JSON files based on
    /// form views.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public sealed class JsonFormController<T> : FileController where T : class, IProvidableObject {

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
        /// Indicates whether fields with null/empty values are
        /// supposed to be written at all.
        /// </summary>
        public bool IsWritingEmptyValues { get; set; }

        /// <summary>
        /// Url file name of JSON file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Data controller for dynamic JSON file.
        /// </summary>
        public FormDataController<T> FormDataController { get; private set; }

        /// <summary>
        /// Indicates what kind of form JSON is to be created for.
        /// </summary>
        public FormType FormType { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteListPageUrl">absolute URL of parent
        /// list page - it may not be empty, not contain any special
        /// charaters except for dashes and has to start and end with
        /// a slash</param>
        /// <param name="formDataController">data controller to use
        /// for form</param>
        /// <param name="fileName">url file name of JSON</param>
        /// <param name="formType">indicates what kind of form JSON
        /// is to be created for</param>
        public JsonFormController(IBusinessApplication businessApplication, string absoluteListPageUrl, FormDataController<T> formDataController, string fileName, FormType formType)
            : base(CacheControl.NoStore, false) {
            this.AbsoluteListPageUrl = absoluteListPageUrl;
            this.BusinessApplication = businessApplication;
            this.FormDataController = formDataController;
            this.IsWritingEmptyValues = true;
            this.FileName = fileName;
            this.FormType = formType;
        }

        /// <summary>
        /// Finds the file for a specific URL.
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <returns>file for URL or null</returns>
        protected override File FindFile(Uri url) {
            File jsonFile = null;
            var urlSegments = ProvidableObjectPageController.GetListPageRelativeUrlSegments(url.AbsolutePath, this.AbsoluteListPageUrl);
            if (2L == urlSegments.LongLength) {
                if (this.FileName == urlSegments[1] && !string.IsNullOrEmpty(this.FileName)) {
                    var presentableObject = this.FormDataController.DataProvider.FindOne(urlSegments[0]);
                    if (null != presentableObject) {
                        FormView formView;
                        if (FormType.View == this.FormType) {
                            formView = this.FormDataController.GetViewFormView(presentableObject);
                        } else if (FormType.Edit == this.FormType) {
                            formView = this.FormDataController.GetEditFormView(presentableObject);
                        } else {
                            formView = null;
                        }
                        if (null != formView) {
                            jsonFile = this.GetJsonFile(presentableObject, formView);
                        }
                    }
                }
            }
            return jsonFile;
        }

        /// <summary>
        /// Gets the JSON file for a combination of presentable
        /// object and form view.
        /// </summary>
        /// <param name="presentableObject">presentable object to get
        /// JSON file for</param>
        /// <param name="formView">form view to get JSON file for</param>
        /// <returns>JSON file for a combination of presentable
        /// object and form view</returns>
        private File GetJsonFile(T presentableObject, Presentation.Forms.FormView formView) {
            File jsonFile;
            var optionDataProvider = new OptionDataProvider(this.BusinessApplication.PersistenceMechanism);
            var jsonWriter = new JsonWriter(formView, optionDataProvider, this.BusinessApplication.FileBaseDirectory) {
                IsWritingEmptyValues = this.IsWritingEmptyValues
            };
            jsonFile = jsonWriter.WriteFile(string.Empty, presentableObject);
            return jsonFile;
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