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
    /// Control for rendering for combining view panes of one object
    /// in a static container - this is used for grouping view panes
    /// visually.
    /// </summary>
    public class FormPaneForPanes : FormPaneWithTitle {

        /// <summary>
        /// Child form panes.
        /// </summary>
        protected IList<FormPane> FormPanes { get; private set; }

        /// <summary>
        /// View pane to create form pane for.
        /// </summary>
        public ViewPaneForPanes ViewPane { get; private set; }

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
        public FormPaneForPanes(FormPaneType paneType, IPresentableObject presentableObject, ViewPaneForPanes viewPane, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory, WebFactory formFactory)
            : base(paneType, presentableObject, viewPane.Key, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, formFactory) {
            this.ViewPane = viewPane;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            this.FormPanes = new List<FormPane>(this.ViewPane.ViewPanes.Count);
            var presentableObjectToRenderPaneFor = this.GetPresentableObjectToRenderPaneFor();
            if (null != presentableObjectToRenderPaneFor) {
                foreach (ViewPane viewPane in this.ViewPane.ViewPanes) {
                    if (viewPane.IsVisible) {
                        FormPane formPane = this.FormFactory.BuildPaneFor(presentableObjectToRenderPaneFor, viewPane, this.TopmostParentPresentableObject, this.ComparisonDate, this.ClientFieldIdPrefix, this.ClientFieldIdSuffix, this.PostBackState, this.FileBaseDirectory);
                        this.Controls.Add(formPane);
                        this.FormPanes.Add(formPane);
                    }
                }
            }
            base.CreateChildControls(httpRequest);
            this.SetHasReadOnlyFieldsOnly(this.FormPanes);
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            string title = this.ViewPane.Title;
            this.RenderPaneTitleAndObjectId(html, title);
            base.RenderChildControls(html);
            return;
        }

        /// <summary>
        /// Set validity state of post back values.
        /// </summary>
        internal sealed override void SetHasValidValue() {
            this.SetHasValidValue(this.FormPanes);
            return;
        }

    }

}