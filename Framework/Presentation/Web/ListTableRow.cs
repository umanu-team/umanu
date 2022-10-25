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
    using System.Collections.Generic;

    /// <summary>
    /// Control for rendering a row of a list table.
    /// </summary>
    public class ListTableRow : CascadedControl {

        /// <summary>
        /// HTML attributes of tag of control.
        /// </summary>
        public Dictionary<string, string> Attributes { get; private set; }

        /// <summary>
        /// Base directory for files.
        /// </summary>
        public string FileBaseDirectory { get; private set; }

        /// <summary>
        /// Presentable object to view/edit.
        /// </summary>
        public IPresentableObject PresentableObject { get; private set; }

        /// <summary>
        /// List table view to render list table for.
        /// </summary>
        public IListTableView View { get; private set; }

        /// <summary>
        /// Factory for building form controls.
        /// </summary>
        public WebFactory WebFactory { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// view/edit</param>
        /// <param name="view">view to render form for</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="webFactory">factory for building web
        /// controls</param>
        public ListTableRow(IPresentableObject presentableObject, IListTableView view, string fileBaseDirectory, WebFactory webFactory)
            : base("tr") {
            this.Attributes = new Dictionary<string, string>();
            this.FileBaseDirectory = fileBaseDirectory;
            this.WebFactory = webFactory;
            this.PresentableObject = presentableObject;
            this.View = view;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            foreach (var viewField in this.View.ViewFields) {
                if (viewField.IsVisible) {
                    var viewFieldForEditableValue = viewField as ViewFieldForEditableValue;
                    if (null == viewFieldForEditableValue) {
                        var webField = this.WebFactory.BuildFieldFor(viewField, this.PresentableObject, null, string.Empty, string.Empty, PostBackState.NoPostBack, this.FileBaseDirectory);
                        this.Controls.Add(webField);
                    } else {
                        var presentableField = this.PresentableObject.FindPresentableField(viewFieldForEditableValue.Key);
                        if (null != presentableField) {
                            var webField = this.WebFactory.BuildFieldFor(presentableField, viewFieldForEditableValue, this.PresentableObject, null, string.Empty, string.Empty, PostBackState.NoPostBack, this.FileBaseDirectory);
                            this.Controls.Add(webField);
                        } else { // missing fields are always ignored in list table rows
                            this.Controls.Add(new WebControl("td"));
                        }
                    }
                }
            }
            base.CreateChildControls(httpRequest);
            return;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            foreach (var attribute in this.Attributes) {
                yield return attribute;
            }
        }

    }

}