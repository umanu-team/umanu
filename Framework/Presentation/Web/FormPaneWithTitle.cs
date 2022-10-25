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

    /// <summary>
    /// Control for rendering panes with titles for combining fields
    /// or view panes visually.
    /// </summary>
    public abstract class FormPaneWithTitle : FormPane {

        /// <summary>
        /// CSS class to set if there is an input error within this
        /// pane.
        /// </summary>
        public string CssClassForPaneError { get; set; }

        /// <summary>
        /// True to force the rendering of an object ID, false
        /// otherwise.
        /// </summary>
        public bool ForceRenderingOfObjectId { get; set; }

        /// <summary>
        /// True if this is new from post-back, false otherwise.
        /// </summary>
        public bool IsNewFromPostBack { get; set; }

        /// <summary>
        /// True to force the rendering of an object ID, false
        /// otherwise.
        /// </summary>
        public FormPaneType PaneType { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="paneType">type of form pane</param>
        /// <param name="presentableObject">presentable object to
        /// show</param>
        /// <param name="key">internal key of field containing the
        /// child objects(s) to use</param>
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
        public FormPaneWithTitle(FormPaneType paneType, IPresentableObject presentableObject, string key, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory, WebFactory formFactory)
            : base(paneType == FormPaneType.StandAlone ? "fieldset" : "section", presentableObject, key, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, formFactory) {
            if (paneType == FormPaneType.StandAlone) {
                this.CssClasses.Add("fieldset");
            }
            this.PaneType = paneType;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            if (FormPaneType.Section == this.PaneType) {
                if (RemovalType.False == this.PresentableObject.RemoveOnUpdate) {
                    if (false == this.HasValidValue) {
                        if (!string.IsNullOrEmpty(this.CssClassForPaneError)) {
                            yield return new KeyValuePair<string,string>("data-header-css-class", this.CssClassForPaneError);
                        }
                        yield return new KeyValuePair<string, string>("data-selected", "1");
                    } else if (false == this.HasReadOnlyFieldsOnly) {
                        yield return new KeyValuePair<string, string>("data-selected", "2");
                    }
                } else {
                    this.CssClasses.Add("removed");
                }
            } else if (FormPaneType.StandAlone == this.PaneType) {
                if (false == this.HasValidValue && !string.IsNullOrEmpty(this.CssClassForPaneError)) {
                    this.CssClasses.Add(this.CssClassForPaneError);
                }
            }
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
        }

        /// <summary>
        /// Renders the title of the pane and the object ID if
        /// required.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="title">title of pane</param>
        protected virtual void RenderPaneTitleAndObjectId(HtmlWriter html, string title) {
            if (FormPaneType.Section != this.PaneType && !string.IsNullOrEmpty(title)) {
                html.AppendOpeningTag("span");
                html.Append(title);
                html.AppendClosingTag("span");
            }
            if (this.ForceRenderingOfObjectId || !string.IsNullOrEmpty(this.Key)) {
                var hiddenInputName = this.ClientFieldIdPrefix + this.ClientFieldIdSuffix;
                if (this.PresentableObject.IsNew) {
                    if (RemovalType.False != this.PresentableObject.RemoveOnUpdate) {
                        html.AppendHiddenInputTag(hiddenInputName, "R-N");
                    } else if (this.IsNewFromPostBack) {
                        html.AppendHiddenInputTag(hiddenInputName, "N");
                    } else {
                        html.AppendHiddenInputTag(hiddenInputName, "P");
                    }
                } else {
                    if (RemovalType.False == this.PresentableObject.RemoveOnUpdate) {
                        html.AppendHiddenInputTag(hiddenInputName, this.PresentableObject.Id.ToString("N"));
                    } else {
                        html.AppendHiddenInputTag(hiddenInputName, "R-" + this.PresentableObject.Id.ToString("N"));
                    }
                }
            }
            return;
        }

    }

}