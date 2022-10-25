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

namespace Framework.Presentation.Web {

    using Framework.Persistence;
    using Framework.Presentation.Forms;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Field control for multiple files.
    /// </summary>
    public class WebFieldForMultipleFiles : WebFieldForMultipleFiles<File> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableField">presentable field to
        /// build control for</param>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to ID of
        /// field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to ID of
        /// field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        public WebFieldForMultipleFiles(IPresentableFieldForCollection presentableField, ViewFieldForMultipleFiles viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            // nothing to do
        }

    }

    /// <summary>
    /// Field control for multiple files be used in form controls.
    /// </summary>
    /// <typeparam name="T">type of file</typeparam>
    public abstract class WebFieldForMultipleFiles<T> : WebFieldForCollection where T : File, new() {

        /// <summary>
        /// Base directory for files.
        /// </summary>
        public string FileBaseDirectory { get; set; }

        /// <summary>
        /// True if file link anchors are supposed to be rendered,
        /// false to render file names only.
        /// </summary>
        public bool HasFileLinkAnchors { get; set; }

        /// <summary>
        /// List of files to be removed.
        /// </summary>
        protected IList<T> RemovedFiles { get; private set; }

        /// <summary>
        /// List of current temporary files.
        /// </summary>
        protected IList<TemporaryFile> TemporaryFiles { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForMultipleFiles ViewField { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableField">presentable field to
        /// build control for</param>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to ID of
        /// field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to ID of
        /// field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        public WebFieldForMultipleFiles(IPresentableFieldForCollection presentableField, ViewFieldForMultipleFiles viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.FileBaseDirectory = string.Empty;
            this.HasFileLinkAnchors = FieldRenderMode.ListTable != renderMode;
            this.RemovedFiles = new List<T>();
            this.TemporaryFiles = new List<TemporaryFile>();
            this.ViewField = viewField;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(HttpRequest httpRequest) {
            this.ErrorMessage = null;
            this.IsIncludedInPostBack = false;
            if (FieldRenderMode.Form == this.RenderMode && PostBackState.ValidPostBack == this.PostBackState) {
                this.IsIncludedInPostBack = true;
                if (!this.IsReadOnly) {
                    for (int i = 0; true; i++) {
                        string removedIdAsString = httpRequest.Form[this.ClientFieldId + "RID" + i];
                        if (string.IsNullOrEmpty(removedIdAsString)) {
                            break;
                        } else if (Guid.TryParse(removedIdAsString, out Guid removedId)) {
                            foreach (var currentObject in this.PresentableField.GetValuesAsObject()) {
                                var file = currentObject as T;
                                if (file.Id == removedId) {
                                    this.RemovedFiles.Add(file);
                                    file.RemoveOnUpdate = RemovalType.RemoveCascadedly;
                                    break;
                                }
                            }
                        }
                    }
                    for (int i = 0; true; i++) {
                        var temporaryFile = TemporaryFile.Load(httpRequest.Form[this.ClientFieldId + "TID" + i], this.OptionDataProvider);
                        if (null == temporaryFile) {
                            break;
                        } else {
                            this.TemporaryFiles.Add(temporaryFile);
                            T file = new T();
                            file.CopyFrom(temporaryFile, CopyBehaviorForAllowedGroups.DoNotCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.DeepCopy);
                            this.PresentableField.AddObject(file);
                        }
                    }
                    bool hasValidValue = true;
                    for (int i = 0; i < httpRequest.Files.Count; i++) {
                        if (httpRequest.Files.Keys[i] == this.ClientFieldId) {
                            HttpPostedFile postBackFile = httpRequest.Files[i];
                            if (null != postBackFile && postBackFile.ContentLength > 0) {
                                var errorMessage = this.ViewField.Validate(postBackFile, ValidityCheck.Transitional);
                                if (string.IsNullOrEmpty(errorMessage)) {
                                    var temporaryFile = TemporaryFile.Save(postBackFile, this.OptionDataProvider);
                                    this.TemporaryFiles.Add(temporaryFile);
                                    try {
                                        T file = new T();
                                        file.CopyFrom(temporaryFile, CopyBehaviorForAllowedGroups.DoNotCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.DeepCopy);
                                        this.PresentableField.AddObject(file);
                                    } catch (ArgumentException) {
                                        hasValidValue = false;
                                    }
                                } else {
                                    hasValidValue = false;
                                    this.ErrorMessage = errorMessage;
                                }
                            }
                        }
                    }
                    hasValidValue = hasValidValue && (Mandatoriness.Required != this.ViewField.Mandatoriness || this.PresentableField.Count - this.RemovedFiles.Count > 0);
                    if (!hasValidValue && string.IsNullOrEmpty(this.ErrorMessage)) {
                        this.ErrorMessage = this.ViewField.GetDefaultErrorMessage();
                    }
                }
            } else if (FieldRenderMode.ListTable == this.RenderMode) {
                base.CreateChildControls(httpRequest);
            }
            return;
        }

        /// <summary>
        /// Gets the attributes of links to a file.
        /// </summary>
        /// <param name="fileId">id of file to get anchor attributes
        /// for</param>
        /// <param name="file">file to get anchor attributes for</param>
        /// <returns>anchor attributes for file</returns>
        protected virtual IDictionary<string, string> GetFileLinkAnchorAttributesFor(Guid fileId, T file) {
            if (!this.HasFileLinkAnchors) {
                throw new InvalidOperationException("File link anchors are disabled for this web field.");
            }
            var anchorAttributes = new Dictionary<string, string>(2) {
                { "href", Controllers.FileController.GetUrlOf(this.FileBaseDirectory, fileId, file.Name) },
                { "target", "_blank" }
            };
            return anchorAttributes;
        }

        /// <summary>
        /// Renders a control for editing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderEditableValue(HtmlWriter html) {
            var attributes = new Dictionary<string, string>(7) {
                { "id", this.ClientFieldId },
                { "type", "file" },
                { "name", this.ClientFieldId },
                { "multiple", "multiple" }
            };
            var acceptedMimeTypes = new StringBuilder();
            foreach (string acceptedMimeType in this.ViewField.AcceptedMimeTypes) {
                if (acceptedMimeTypes.Length > 0) {
                    acceptedMimeTypes.Append(' ');
                }
                acceptedMimeTypes.Append(System.Web.HttpUtility.HtmlEncode(acceptedMimeType));
            }
            attributes.Add("accept", acceptedMimeTypes.ToString());
            if (this.ViewField.IsAutofocused) {
                attributes.Add("autofocus", "autofocus");
            }
            if (Mandatoriness.Required == this.ViewField.Mandatoriness) {
                attributes.Add("required", "required");
            }
            html.AppendSelfClosingTag("input", attributes);
            if (this.PresentableField.Count - this.RemovedFiles.Count > 0) {
                bool isFirstFile = true;
                uint temporaryFileCount = 0;
                foreach (var currentObject in this.PresentableField.GetValuesAsObject()) {
                    var file = currentObject as T;
                    if (null != file && RemovalType.False == file.RemoveOnUpdate && !string.IsNullOrEmpty(file.Name)) {
                        if (isFirstFile) {
                            isFirstFile = false;
                        } else if (FieldRenderMode.ListTable == this.RenderMode) {
                            html.Append(", ");
                        }
                        this.RenderFileLinks(html, file, true, false, ref temporaryFileCount);
                    }
                }
            }
            for (int i = 0; i < this.RemovedFiles.Count; i++) {
                var removedFile = this.RemovedFiles[i];
                html.AppendHiddenInputTag(this.ClientFieldId + "RID" + i, removedFile.Id.ToString("N"));
            }
            return;
        }

        /// <summary>
        /// Renders a paragraph showing the file links.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="file">file to render link for</param>
        /// <param name="hasDeleteIcon">true to render delete
        /// button, false otherwise</param>
        /// <param name="isDiffNew">true if file is new in
        /// comparison, false otherwise</param>
        /// <param name="temporaryFileCount">number of temporary
        /// files</param>
        protected void RenderFileLinks(HtmlWriter html, T file, bool hasDeleteIcon, bool isDiffNew, ref uint temporaryFileCount) {
            if (FieldRenderMode.Form == this.RenderMode) {
                html.AppendOpeningTag("span", "listitem");
            }
            if (isDiffNew) {
                html.AppendOpeningTag("span", "diffnew");
            }
            Guid fileId = file.Id;
            string onClickAction = "$(this).parent().parent().append($(document.createElement('input')).attr('type', 'hidden').attr('name', '" + this.ClientFieldId + "RID' + $('input[name^=\\'" + this.ClientFieldId + "RID\\']').length).attr('value', '" + fileId.ToString("N") + "')); $(this).parent().remove();";
            foreach (var temporaryFile in this.TemporaryFiles) {
                if (temporaryFile.Name == file.Name && temporaryFile.MimeType == file.MimeType && temporaryFile.Size == file.Size) {
                    fileId = temporaryFile.Id;
                    onClickAction = "$(this).parent().remove();";
                    if (hasDeleteIcon) {
                        html.AppendHiddenInputTag(this.ClientFieldId + "TID" + temporaryFileCount, temporaryFile.Id.ToString("N"));
                        temporaryFileCount++;
                    }
                    break;
                }
            }
            if (this.HasFileLinkAnchors) {
                var anchorAttributes = this.GetFileLinkAnchorAttributesFor(fileId, file);
                html.AppendOpeningTag("a", anchorAttributes);
            }
            this.RenderFileName(html, fileId, file);
            if (this.HasFileLinkAnchors) {
                html.AppendClosingTag("a");
            }
            if (hasDeleteIcon) {
                html.Append("&nbsp;");
                string onClickConfirmationMessage = HttpUtility.HtmlEncode(string.Format(Resources.WouldYouReallyLikeToMarkTheFile0ForRemoval, file.Name.Replace("'", "\\'")));
                var iconAttributes = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("class", "listaction"),
                    new KeyValuePair<string, string>("onclick", "javascript:if(window.confirm('" + onClickConfirmationMessage + "')){" + onClickAction + "}")
                };
                html.AppendOpeningTag("span", iconAttributes);
                html.Append("&#xE15C;");
                html.AppendClosingTag("span");
            }
            if (isDiffNew) {
                html.AppendClosingTag("span");
            }
            if (FieldRenderMode.Form == this.RenderMode) {
                html.AppendClosingTag("span");
            }
            return;
        }

        /// <summary>
        /// Renders a paragraph showing a file name.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="fileId">id of file to render name for</param>
        /// <param name="file">file to render name for</param>
        protected virtual void RenderFileName(HtmlWriter html, Guid fileId, T file) {
            html.AppendHtmlEncoded(file.Name);
            return;
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            bool isFirstValue = true;
            uint temporaryFileCount = 0;
            foreach (var currentObject in this.PresentableField.GetValuesAsObject()) {
                T currentFile = currentObject as T;
                if (null != currentFile && !string.IsNullOrEmpty(currentFile.Name)) {
                    if (isFirstValue) {
                        isFirstValue = false;
                    } else if (FieldRenderMode.ListTable == this.RenderMode) {
                        html.Append(", ");
                    }
                    bool isDiffNew = this.ComparisonDate.HasValue && null == currentFile.GetVersionValue(this.ComparisonDate);
                    this.RenderFileLinks(html, currentFile, false, isDiffNew, ref temporaryFileCount);
                }
            }
            return;
        }

    }

}