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

    using System;
    using System.Collections.Generic;
    using Framework.Presentation.Forms;

    /// <summary>
    /// Control for rendering panes for combining view fields or
    /// panes for one object in a dynamic container - the user can
    /// switch between which of the contained sections of view panes
    /// to display.
    /// </summary>
    public class FormGroupedPane : FormPane {

        /// <summary>
        /// Child form sections.
        /// </summary>
        protected IList<FormPaneWithTitle> FormSections { get; private set; }

        /// <summary>
        /// View pane to create form pane for.
        /// </summary>
        public ViewGroupedPane ViewPane { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
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
        public FormGroupedPane(IPresentableObject presentableObject, ViewGroupedPane viewPane, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory, WebFactory formFactory)
            : base("div", presentableObject, viewPane.Key, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, formFactory) {
            this.ViewPane = viewPane;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            this.FormSections = new List<FormPaneWithTitle>(this.ViewPane.Sections.Count);
            var presentableObjectToRenderPaneFor = this.GetPresentableObjectToRenderPaneFor();
            if (null != presentableObjectToRenderPaneFor) {
                foreach (ViewPaneWithTitle viewSection in this.ViewPane.Sections) {
                    if (viewSection.IsVisible) {
                        FormPaneWithTitle formSection = this.FormFactory.BuildPaneSectionFor(presentableObjectToRenderPaneFor, viewSection, this.TopmostParentPresentableObject, this.ComparisonDate, this.ClientFieldIdPrefix, this.ClientFieldIdSuffix, this.PostBackState, this.FileBaseDirectory, false);
                        string title = viewSection.Title;
                        if (!string.IsNullOrEmpty(title)) {
                            formSection.Attributes.Add("data-title", System.Web.HttpUtility.HtmlEncode(title));
                        }
                        this.Controls.Add(formSection);
                        this.FormSections.Add(formSection);
                    }
                }
            }
            base.CreateChildControls(httpRequest);
            this.SetHasReadOnlyFieldsOnly(this.FormSections);
            return;
        }

        /// <summary>
        /// Set validity state of post back values.
        /// </summary>
        internal sealed override void SetHasValidValue() {
            this.SetHasValidValue(this.FormSections);
            return;
        }

    }

}