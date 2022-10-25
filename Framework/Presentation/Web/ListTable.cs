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
    using System.Globalization;

    /// <summary>
    /// Control for viewing a list of presentable objects as table.
    /// </summary>
    public class ListTable : MasterControl {

        /// <summary>
        /// CSS class to apply to inner table.
        /// </summary>
        public string CssClassForTable { get; set; }

        /// <summary>
        /// Base directory for files.
        /// </summary>
        public string FileBaseDirectory { get; private set; }

        /// <summary>
        /// True if data in list table is supposed to be filterable,
        /// false otherwise.
        /// </summary>
        public bool IsFilterable { get; set; }

        /// <summary>
        /// Presentable objects to view/edit.
        /// </summary>
        public IEnumerable<IPresentableObject> PresentableObjects { get; private set; }

        /// <summary>
        /// List table view to render list table for.
        /// </summary>
        public IListTableView View { get; private set; }

        /// <summary>
        /// Factory for building list table controls.
        /// </summary>
        public WebFactory WebFactory { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableObjects">presentable objects to
        /// view/edit</param>
        /// <param name="view">view to render list table for</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="buildingRule">rules for building controls
        /// for list table</param>
        /// <param name="richTextEditorSettings">rich-text-editor
        /// settings</param>
        public ListTable(IEnumerable<IPresentableObject> presentableObjects, IListTableView view, string fileBaseDirectory, IOptionDataProvider optionDataProvider, BuildingRule buildingRule, IRichTextEditorSettings richTextEditorSettings)
            : this(presentableObjects, view, fileBaseDirectory, new WebFactory(FieldRenderMode.ListTable, optionDataProvider, buildingRule, richTextEditorSettings)) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableObjects">presentable objects to
        /// view/edit</param>
        /// <param name="view">view to render list table for</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="webFactory">factory for building web
        /// controls</param>
        public ListTable(IEnumerable<IPresentableObject> presentableObjects, IListTableView view, string fileBaseDirectory, WebFactory webFactory)
            : base("div") {
            this.IsFilterable = true;
            this.FileBaseDirectory = fileBaseDirectory;
            this.WebFactory = webFactory;
            this.PresentableObjects = presentableObjects;
            this.View = view;
            webFactory.SetCssClassesForListTable(this);
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            foreach (var presentableObject in this.PresentableObjects) {
                var listTableRow = this.WebFactory.BuildRowFor(presentableObject, this.View, this.FileBaseDirectory);
                if (null != this.OnClickUrlDelegate) {
                    listTableRow.Attributes.Add("data-href", this.OnClickUrlDelegate(presentableObject));
                }
                this.Controls.Add(listTableRow);
            }
            base.CreateChildControls(httpRequest);
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            this.RenderDescriptionMessage(html, this.View.Description);
            var attributes = new Dictionary<string, string>(7);
            if (!string.IsNullOrEmpty(this.CssClassForTable)) {
                attributes.Add("class", this.CssClassForTable);
            }
            if (ChartType.None != this.View.ChartType) {
                attributes.Add("data-chart", this.View.ChartType.ToString().ToLowerInvariant());
            }
            if (this.View.HasCounts) {
                attributes.Add("data-count", "1");
            }
            attributes.Add("data-expanded", this.View.ExpansionDepth.ToString(CultureInfo.InvariantCulture));
            if (this.IsFilterable) {
                attributes.Add("data-filterable", "1");
            }
            attributes.Add("data-groupings", this.View.Groupings.ToString(CultureInfo.InvariantCulture));
            if (!this.View.IsListVisible) {
                attributes.Add("style", "display:none;");
            }
            html.AppendOpeningTag("table", attributes);
            html.AppendOpeningTag("thead");
            this.RenderHeaderRow(html);
            html.AppendClosingTag("thead");
            base.RenderChildControls(html);
            html.AppendClosingTag("table");
            return;
        }

        /// <summary>
        /// Renders the header row of list table.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected virtual void RenderHeaderRow(HtmlWriter html) {
            html.AppendOpeningTag("tr");
            foreach (var viewField in this.View.ViewFields) {
                var attributes = new Dictionary<string, string>(3);
                if (this.View.HasTotals) {
                    byte? decimalPlaces = null;
                    var viewFieldForNumber = viewField as ViewFieldForNumber;
                    if (null == viewFieldForNumber) {
                        var viewFieldForMultipleNumbers = viewField as ViewFieldForMultipleNumbers;
                        if (null != viewFieldForMultipleNumbers) {
                            attributes.Add("data-total", "sum");
                            decimalPlaces = viewFieldForNumber.DecimalPlaces;
                        }
                    } else {
                        attributes.Add("data-total", "sum");
                        decimalPlaces = viewFieldForNumber.DecimalPlaces;
                        if (!string.IsNullOrEmpty(viewFieldForNumber.Unit)) {
                            string unit = viewFieldForNumber.Unit;
                            if (!string.IsNullOrEmpty(unit)) {
                                attributes.Add("data-unit", unit);
                            }
                        }
                    }
                    if (decimalPlaces.HasValue) {
                        attributes.Add("data-places", decimalPlaces.Value.ToString(CultureInfo.InvariantCulture));
                    }
                }
                html.AppendOpeningTag("th", attributes);
                html.Append(viewField.Title);
                html.AppendClosingTag("th");
            }
            html.AppendClosingTag("tr");
            return;
        }

    }

}