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
    /// Field control for a file.
    /// </summary>
    public class WebFieldForFile : WebFieldForFile<File> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableField">presentable field to
        /// build control for</param>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost presentable
        /// parent object to build form for</param>
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
        public WebFieldForFile(IPresentableFieldForElement presentableField, ViewFieldForFile viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            // nothing to do
        }

    }

    /// <summary>
    /// Field control for a file to be used in form controls.
    /// </summary>
    /// <typeparam name="T">type of file</typeparam>
    public abstract class WebFieldForFile<T> : WebFieldForElement where T : File, new() {

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
        protected T RemovedFile { get; private set; }

        /// <summary>
        /// Current temporary file.
        /// </summary>
        protected TemporaryFile TemporaryFile { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForFile ViewField { get; private set; }

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
        public WebFieldForFile(IPresentableFieldForElement presentableField, ViewFieldForFile viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.FileBaseDirectory = string.Empty;
            this.HasFileLinkAnchors = FieldRenderMode.ListTable != renderMode;
            this.RemovedFile = null;
            this.TemporaryFile = null;
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
                if (this.IsReadOnly) {
                    // nothing to do
                } else {
                    bool hasValidValue = true;
                    HttpPostedFile postBackFile = httpRequest.Files[this.ClientFieldId];
                    if (null != postBackFile && postBackFile.ContentLength > 0) {
                        this.ErrorMessage = this.ViewField.Validate(postBackFile, ValidityCheck.Transitional);
                        if (string.IsNullOrEmpty(this.ErrorMessage)) {
                            this.TemporaryFile = TemporaryFile.Save(postBackFile, this.OptionDataProvider);
                            try {
                                T file = new T();
                                file.CopyFrom(this.TemporaryFile, CopyBehaviorForAllowedGroups.DoNotCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.DeepCopy);
                                this.RemovedFile = this.PresentableField.ValueAsObject as T;
                                this.PresentableField.ValueAsObject = file;
                            } catch (ArgumentException) {
                                hasValidValue = false;
                            }
                        } else {
                            hasValidValue = false;
                        }
                    } else {
                        var temporaryFile = TemporaryFile.Load(httpRequest.Form[this.ClientFieldId + "TID"], this.OptionDataProvider);
                        if (null != temporaryFile) {
                            this.TemporaryFile = temporaryFile;
                            T file = new T();
                            file.CopyFrom(temporaryFile, CopyBehaviorForAllowedGroups.DoNotCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.DeepCopy);
                            this.RemovedFile = this.PresentableField.ValueAsObject as T;
                            this.PresentableField.ValueAsObject = file;
                        } else {
                            if (Guid.TryParse(httpRequest.Form[this.ClientFieldId + "RID"], out Guid removedId)) {
                                this.RemovedFile = this.PresentableField.ValueAsObject as T;
                                this.PresentableField.ValueAsObject = null;
                            }
                        }
                    }
                    hasValidValue = hasValidValue && (Mandatoriness.Required != this.ViewField.Mandatoriness || null != this.PresentableField.ValueAsObject);
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
            var attributes = new Dictionary<string, string>(6) {
                { "id", this.ClientFieldId },
                { "type", "file" },
                { "name", this.ClientFieldId }
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
            var file = this.PresentableField.ValueAsObject as T;
            if (null != file && RemovalType.False == file.RemoveOnUpdate && !string.IsNullOrEmpty(file.Name)) {
                this.RenderFileLink(html, file, true, false);
            }
            if (null != this.RemovedFile) {
                html.AppendHiddenInputTag(this.ClientFieldId + "RID", this.RemovedFile.Id.ToString("N"));
            }
            return;
        }

        /// <summary>
        /// Renders a paragraph showing the file link.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="file">file to render link for</param>
        /// <param name="hasDeleteIcon">true to render delete
        /// button, false otherwise</param>
        /// <param name="isDiffNew">true if file is new in
        /// comparison, false otherwise</param>
        protected void RenderFileLink(HtmlWriter html, T file, bool hasDeleteIcon, bool isDiffNew) {
            if (FieldRenderMode.Form == this.RenderMode) {
                html.AppendOpeningTag("span", "listitem");
            }
            if (isDiffNew) {
                html.AppendOpeningTag("span", "diffnew");
            }
            Guid fileId;
            string onClickAction;
            if (null == this.TemporaryFile) {
                fileId = file.Id;
                onClickAction = "$(this).parent().parent().append($(document.createElement('input')).attr('type', 'hidden').attr('name', '" + this.ClientFieldId + "RID').attr('value', '" + fileId.ToString("N") + "')); $(this).parent().remove();";
            } else {
                fileId = this.TemporaryFile.Id;
                onClickAction = "$(this).parent().remove();";
                if (hasDeleteIcon) {
                    html.AppendHiddenInputTag(this.ClientFieldId + "TID", this.TemporaryFile.Id.ToString("N"));
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
            T file = this.PresentableField.ValueAsObject as T;
            if (null != file && !string.IsNullOrEmpty(file.Name)) {
                if (this.ComparisonDate.HasValue && null == file.GetVersionValue(this.ComparisonDate)) {
                    this.RenderFileLink(html, file, false, true);
                } else {
                    this.RenderFileLink(html, file, false, false);
                }
            }
            return;
        }

    }

}