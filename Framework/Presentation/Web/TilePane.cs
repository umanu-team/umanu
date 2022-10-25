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
    /// Control for viewing a list of presentable objects as tiles.
    /// </summary>
    public class TilePane : MasterControl {

        /// <summary>
        /// Description to show on top.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Providable objects to be displayed as tiles.
        /// </summary>
        public IEnumerable<IProvidableObject> ProvidableObjects { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="providableObjects">providable objects to be
        /// displayed as tiles</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="buildingRule">rules for building controls
        /// for this list table</param>
        /// <param name="richTextEditorSettings">rich-text-editor
        /// settings</param>
        public TilePane(IEnumerable<IProvidableObject> providableObjects, IOptionDataProvider optionDataProvider, BuildingRule buildingRule, IRichTextEditorSettings richTextEditorSettings)
            : this(providableObjects, new WebFactory(FieldRenderMode.ListTable, optionDataProvider, buildingRule, richTextEditorSettings)) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="providableObjects">providable objects to be
        /// displayed as tiles</param>
        /// <param name="webFactory">factory for building web
        /// controls</param>
        public TilePane(IEnumerable<IProvidableObject> providableObjects, WebFactory webFactory)
            : base("div") {
            this.ProvidableObjects = providableObjects;
            webFactory.SetCssClassesForTilePane(this);
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            this.RenderDescriptionMessage(html, this.Description);
            base.RenderChildControls(html);
            foreach (var providableObject in this.ProvidableObjects) {
                if (null != providableObject) {
                    var attributes = new List<KeyValuePair<string, string>>(1);
                    if (null != this.OnClickUrlDelegate) {
                        attributes.Add(new KeyValuePair<string, string>("href", this.OnClickUrlDelegate(providableObject)));
                    }
                    html.AppendOpeningTag("a", attributes);
                    html.AppendHtmlEncoded(providableObject.GetTitle());
                    html.AppendClosingTag("a");
                }
            }
            return;
        }

    }

}