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

    using Framework.Presentation.Forms;
    using Framework.Properties;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Form control for viewing/editing presentable objects.
    /// </summary>
    public class Form : CascadedControl, IForm {

        /// <summary>
        /// Absolute path and query of http request.
        /// </summary>
        private string pathAndQuery;

        /// <summary>
        /// Point in time to compare form data of read-only fields
        /// to or null to not compare any data.
        /// </summary>
        public DateTime? ComparisonDate { get; set; }

        /// <summary>
        /// CSS class to use for form description pane.
        /// </summary>
        public string CssClassForDescriptionPane { get; set; }

        /// <summary>
        /// CSS class to use for form error pane.
        /// </summary>
        public string CssClassForErrorPane { get; set; }

        /// <summary>
        /// CSS class to use for paragraph for created at, created
        /// by, modified at and modified by.
        /// </summary>
        public string CssClassForUpdateStatus { get; set; }

        /// <summary>
        /// Custom error message for current instance of form.
        /// </summary>
        public string ErrorMessage {
            get {
                return this.errorMessage;
            }
            set {
                this.errorMessage = value;
                if (string.IsNullOrEmpty(value)) {
                    this.SetHasValidValue();
                } else {
                    this.HasValidValue = false;
                }
            }
        }
        private string errorMessage;

        /// <summary>
        /// Base directory for files.
        /// </summary>
        public string FileBaseDirectory { get; private set; }

        /// <summary>
        /// List of form panes of this form.
        /// </summary>
        private List<FormPane> formPanes;

        /// <summary>
        /// True if all postback values of this form are valid, false
        /// otherwise. Be aware: This property is supposed to be set
        /// during "CreateChildControls" and will be set to null in
        /// any earlier state.
        /// </summary>
        public bool? HasValidValue { get; private set; }

        /// <summary>
        /// ID of current instance of form. Be aware: This property
        /// is supposed to be set during "CreateChildControls" and
        /// will be set to null in any earlier state.
        /// </summary>
        public string InstanceId { get; private set; }

        /// <summary>
        /// Static list of all instance IDs of all users - it will be
        /// recycled on each application pool reset.
        /// </summary>
        private static readonly ConcurrentDictionary<string, List<string>> instanceIds = new ConcurrentDictionary<string, List<string>>();

        /// <summary>
        /// Indicates whether modification info control is decoupled.
        /// </summary>
        public bool IsModificationInfoControlDecoupled { get; private set; }

        /// <summary>
        /// Presentable object to view/edit.
        /// </summary>
        public IPresentableObject PresentableObject { get; private set; }

        /// <summary>
        /// Post back state of this form. Be aware: This property
        /// is supposed to be set during "CreateChildControls" and
        /// will not be set in any earlier state.
        /// </summary>
        public PostBackState PostBackState { get; private set; }

        /// <summary>
        /// Warning message to be displayed on page unload with
        /// unsaved form changes.
        /// </summary>
        public string UnsavedChangesMessage { get; set; }

        /// <summary>
        /// Form view to render form for.
        /// </summary>
        public FormView View { get; private set; }

        /// <summary>
        /// Factory for building form controls.
        /// </summary>
        public WebFactory WebFactory { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// view</param>
        /// <param name="view">view to render form for</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="webFactory">factory for building form
        /// controls</param>
        public Form(IPresentableObject presentableObject, FormView view, string fileBaseDirectory, WebFactory webFactory)
            : base("form") {
            this.HasValidValue = null;
            this.IsModificationInfoControlDecoupled = false;
            this.PresentableObject = presentableObject;
            this.View = view;
            this.FileBaseDirectory = fileBaseDirectory;
            this.UnsavedChangesMessage = Resources.AllUnsafedChangesWillBeDiscardedIfYouLeaveThisPage;
            this.WebFactory = webFactory;
            webFactory.SetCssClassesForForm(this);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// view</param>
        /// <param name="view">view to render form for</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="buildingRule">rules for building controls
        /// for this form</param>
        /// <param name="richTextEditorSettings">rich-text-editor
        /// settings</param>
        public Form(IPresentableObject presentableObject, FormView view, string fileBaseDirectory, IOptionDataProvider optionDataProvider, BuildingRule buildingRule, IRichTextEditorSettings richTextEditorSettings)
            : this(presentableObject, view, fileBaseDirectory, new WebFactory(FieldRenderMode.Form, optionDataProvider, buildingRule, richTextEditorSettings)) {
            // nothing to do
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(HttpRequest httpRequest) {
            this.pathAndQuery = httpRequest.Url.PathAndQuery;
            this.SetInstanceIDAndPostBackState(httpRequest);
            this.formPanes = new List<FormPane>(this.View.ViewPanes.Count);
            foreach (var viewPane in this.View.ViewPanes) {
                if (viewPane.IsVisible) {
                    var formPane = this.WebFactory.BuildPaneFor(this.PresentableObject, viewPane, this.PresentableObject, this.ComparisonDate, string.Empty, string.Empty, this.PostBackState, this.FileBaseDirectory);
                    this.Controls.Add(formPane);
                    this.formPanes.Add(formPane);
                }
            }
            base.CreateChildControls(httpRequest);
            this.SetHasValidValue();
            return;
        }

        /// <summary>
        /// Decouples the modification info control if present.
        /// </summary>
        /// <returns>modification info control or null</returns>
        public Control DecoupleModificationInfoControl() {
            this.IsModificationInfoControlDecoupled = true;
            return this.GetModificationInfoControl();
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            yield return new KeyValuePair<string, string>("action", this.pathAndQuery);
            if (!this.View.HasAutocompletionEnabled) {
                yield return new KeyValuePair<string, string>("autocomplete", "off");
            }
            yield return new KeyValuePair<string, string>("enctype", "multipart/form-data");
            yield return new KeyValuePair<string, string>("method", "post");
            if (!string.IsNullOrEmpty(this.UnsavedChangesMessage)) {
                yield return new KeyValuePair<string, string>("data-unload-warning", this.UnsavedChangesMessage);
            }
        }

        /// <summary>
        /// Gets the control for modification info or null.
        /// </summary>
        /// <returns>control for modification info or null</returns>
        private Control GetModificationInfoControl() {
            ModificationInfoControl modificationInfoControl;
            if (this.View.HasModificationInfo && !this.PresentableObject.IsNew) {
                modificationInfoControl = new ModificationInfoControl(this.PresentableObject);
                modificationInfoControl.CssClasses.Add(this.CssClassForUpdateStatus);
            } else {
                modificationInfoControl = null;
            }
            return modificationInfoControl;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.AppendHiddenInputTag("instance", this.InstanceId);
            if (this.PresentableObject.IsNew) {
                html.AppendHiddenInputTag("object", "N");
            } else {
                html.AppendHiddenInputTag("object", this.PresentableObject.Id.ToString("N"));
            }
            this.RenderErrorMessage(html);
            this.RenderDescriptionMessage(html);
            foreach (var control in this.Controls) {
                var formPane = control as FormPane;
                if (null == formPane || !formPane.IsEmpty) {
                    control.Render(html);
                }
            }
            this.RenderModificationInfo(html);
            return;
        }

        /// <summary>
        /// Renders a description message if applicable.
        /// </summary>
        /// <param name="html">HTML response</param>
        private void RenderDescriptionMessage(HtmlWriter html) {
            string descriptionMessage;
            if (this.View.IsReadOnlyFor(this.PresentableObject)) {
                descriptionMessage = this.View.DescriptionForViewMode;
            } else {
                descriptionMessage = this.View.DescriptionForEditMode;
            }
            if (!string.IsNullOrEmpty(descriptionMessage)) {
                var infoPane = new InfoPane(descriptionMessage);
                infoPane.CssClasses.Add(this.CssClassForDescriptionPane);
                infoPane.Render(html);
            }
            return;
        }

        /// <summary>
        /// Renders an error message if applicable.
        /// </summary>
        /// <param name="html">HTML response</param>
        private void RenderErrorMessage(HtmlWriter html) {
            if (false == this.HasValidValue) {
                string message;
                if (string.IsNullOrEmpty(this.ErrorMessage)) {
                    message = Resources.PleaseCorrectTheInvalidInputValues;
                } else {
                    message = this.ErrorMessage;
                }
                var infoPane = new InfoPane(message);
                infoPane.CssClasses.Add(this.CssClassForErrorPane);
                infoPane.Render(html);
            }
            return;
        }

        /// <summary>
        /// Renders a paragraph for created at, created by, modified
        /// at and modified by if applicable.
        /// </summary>
        /// <param name="html">HTML response</param>
        private void RenderModificationInfo(HtmlWriter html) {
            if (!this.IsModificationInfoControlDecoupled) {
                var modificationInfoControl = this.GetModificationInfoControl();
                if (null != modificationInfoControl) {
                    modificationInfoControl.Render(html);
                }
            }
            return;
        }

        /// <summary>
        /// Set validity state of post back values.
        /// </summary>
        private void SetHasValidValue() {
            if (PostBackState.ValidPostBack == this.PostBackState) {
                this.HasValidValue = true;
                foreach (var formPane in this.formPanes) {
                    formPane.SetHasValidValue();
                    if (null == formPane.HasValidValue) {
                        this.HasValidValue = null;
                        break;
                    } else if (false == formPane.HasValidValue) {
                        this.HasValidValue = false;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Sets post back state and instance ID of the current
        /// instance of this form.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        private void SetInstanceIDAndPostBackState(HttpRequest httpRequest) {
            var userPrefix = httpRequest.UserHostAddress
                + "#" + this.WebFactory.OptionDataProvider.UserDirectory.CurrentUser.UserName
                + "#" + httpRequest.UserAgent;
            var userInstanceIds = Form.instanceIds.GetOrAdd(userPrefix, delegate (string key) {
                return new List<string>();
            });
            var postBackInstanceId = httpRequest.Form["instance"];
            if (null == postBackInstanceId) {
                this.PostBackState = PostBackState.NoPostBack;
            } else if (!userInstanceIds.Contains(postBackInstanceId)) {
                this.PostBackState = PostBackState.InvalidPostBack;
            } else {
                this.PostBackState = PostBackState.ValidPostBack;
                userInstanceIds.Remove(postBackInstanceId);
            }
            this.InstanceId = Guid.NewGuid().ToString("N");
            userInstanceIds.Add(this.InstanceId);
            return;
        }

    }

}