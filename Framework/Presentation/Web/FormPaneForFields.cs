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

    using Framework.Presentation.Exceptions;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Control for rendering panes for combining fields of one
    /// object in a static container - this is used for grouping
    /// fields visually.
    /// </summary>
    public class FormPaneForFields : FormPaneWithTitle {

        /// <summary>
        /// Child form fields.
        /// </summary>
        protected IList<WebFieldForEditableValue> FormFields { get; private set; }

        /// <summary>
        /// View pane to create form pane for.
        /// </summary>
        public ViewPaneForFields ViewPane { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="paneType">type of form pane</param>
        /// <param name="presentableObject">presentable object to
        /// show</param>
        /// <param name="viewPane">view pane to render form pane for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="formFactory">factory for building form
        /// controls</param>
        public FormPaneForFields(FormPaneType paneType, IPresentableObject presentableObject, ViewPaneForFields viewPane, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory, WebFactory formFactory)
            : base(paneType, presentableObject, viewPane.Key, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, formFactory) {
            this.ViewPane = viewPane;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(HttpRequest httpRequest) {
            this.FormFields = new List<WebFieldForEditableValue>(this.ViewPane.ViewFields.Count);
            var presentableObjectToRenderPaneFor = this.GetPresentableObjectToRenderPaneFor();
            if (null != presentableObjectToRenderPaneFor) {
                foreach (var viewField in this.ViewPane.ViewFields) {
                    if (viewField.IsVisible) {
                        var viewFieldForEditableValue = viewField as ViewFieldForEditableValue;
                        if (null == viewFieldForEditableValue) {
                            var webField = this.FormFactory.BuildFieldFor(viewField, this.TopmostParentPresentableObject, this.ComparisonDate, this.ClientFieldIdPrefix, this.ClientFieldIdSuffix, this.PostBackState, this.FileBaseDirectory);
                            this.Controls.Add(webField);
                        } else {
                            var presentableField = presentableObjectToRenderPaneFor.FindPresentableField(viewFieldForEditableValue.KeyChain);
                            if (null == presentableField && !this.FormFactory.IgnoreMissingFields && this.IsFieldMissing(presentableObjectToRenderPaneFor, viewFieldForEditableValue.KeyChain)) {
                                throw new Framework.Presentation.Exceptions.KeyNotFoundException("Presentable field for view field with key \"" + viewFieldForEditableValue.Key + "\" cannot be found.");
                            }
                            if (null != presentableField) {
                                var webFieldForEditableValue = this.FormFactory.BuildFieldFor(presentableField, viewFieldForEditableValue, this.TopmostParentPresentableObject, this.ComparisonDate, this.ClientFieldIdPrefix, this.ClientFieldIdSuffix, this.PostBackState, this.FileBaseDirectory);
                                this.Controls.Add(webFieldForEditableValue);
                                this.FormFields.Add(webFieldForEditableValue);
                            }
                        }
                    }
                }
            }
            base.CreateChildControls(httpRequest);
            this.SetHasReadOnlyFieldsOnly();
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            if (this.ViewPane.HasTwoColumnsInWideWindows && !string.IsNullOrEmpty(this.ViewPane.Title)) {
                throw new InvalidOperationException("View pane for fields cannot be rendered with two columns if a title is set. Please do not set a title for view pane and encapsulate it into a view pane for panes instead, which has the title to be displayed set.");
            }
            string title = this.ViewPane.Title;
            this.RenderPaneTitleAndObjectId(html, title);
            if (FormPaneType.Section == this.PaneType) {
                html.AppendOpeningTag("fieldset", "fieldset");
            }
            base.RenderChildControls(html);
            if (FormPaneType.Section == this.PaneType) {
                html.AppendClosingTag("fieldset");
            }
            return;
        }

        /// <summary>
        /// Set whether any contained child field is writable.
        /// </summary>
        protected void SetHasReadOnlyFieldsOnly() {
            this.HasReadOnlyFieldsOnly = true;
            foreach (var formField in this.FormFields) {
                if (!formField.IsReadOnly) {
                    this.HasReadOnlyFieldsOnly = false;
                    break;
                }
            }
            return;
        }

        /// <summary>
        /// Set validity state of post back values.
        /// </summary>
        internal sealed override void SetHasValidValue() {
            this.HasValidValue = true;
            foreach (var formField in this.FormFields) {
                formField.SetHasValidValue();
                if (!formField.IsIncludedInPostBack && !this.PresentableObject.IsNew) {
                    throw new PresentationException("Form cannot be validated because expected web field with key \"" + formField.ClientFieldId + "\" is not included in post back data. Typically this occures if the post back data was manipulated.");
                } else if (!string.IsNullOrEmpty(formField.ErrorMessage)) {
                    this.HasValidValue = false;
                }
            }
            return;
        }

    }

}