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
    using Persistence;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Control for viewing a list of presentable objects as cards.
    /// </summary>
    public class CardPane : MasterControl {

        /// <summary>
        /// Description to show on top.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Base directory for files.
        /// </summary>
        public string FileBaseDirectory { get; set; }

        /// <summary>
        /// Providable objects to be displayed as cards.
        /// </summary>
        public IEnumerable<IProvidableObject> ProvidableObjects { get; private set; }

        /// <summary>
        /// View to apply to card pane.
        /// </summary>
        public CardPaneView View { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="providableObjects">providable objects to be
        /// displayed as cards</param>
        /// <param name="view">view to apply to card pane</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="buildingRule">rules for building controls
        /// for this list table</param>
        /// <param name="richTextEditorSettings">rich-text-editor
        /// settings</param>
        public CardPane(IEnumerable<IProvidableObject> providableObjects, CardPaneView view, IOptionDataProvider optionDataProvider, BuildingRule buildingRule, IRichTextEditorSettings richTextEditorSettings)
            : this(providableObjects, view, new WebFactory(FieldRenderMode.ListTable, optionDataProvider, buildingRule, richTextEditorSettings)) {
            this.FileBaseDirectory = string.Empty;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="providableObjects">providable objects to be
        /// displayed as cards</param>
        /// <param name="view">view to apply to card pane</param>
        /// <param name="webFactory">factory for building web
        /// controls</param>
        public CardPane(IEnumerable<IProvidableObject> providableObjects, CardPaneView view, WebFactory webFactory)
            : base("div") {
            this.ProvidableObjects = providableObjects;
            this.View = view;
            webFactory.SetCssClassesForCardPane(this);
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
                    var attributes = new List<KeyValuePair<string, string>>(2);
                    var cssClasses = new List<string>(this.View.GetCssClasses(providableObject));
                    if (cssClasses.Count > 0) {
                        attributes.Add(new KeyValuePair<string, string>("class", string.Join(" ", cssClasses)));
                    }
                    if (null != this.OnClickUrlDelegate) {
                        attributes.Add(new KeyValuePair<string, string>("href", this.OnClickUrlDelegate(providableObject)));
                    }
                    html.AppendOpeningTag("a", attributes);
                    int maxLengthOfDescription;
                    if (this.RenderImage(html, providableObject)) {
                        maxLengthOfDescription = this.View.MaxLengthOfShortDescription;
                    } else {
                        maxLengthOfDescription = this.View.MaxLengthOfLongDescription;
                    }
                    this.RenderTitle(html, providableObject);
                    this.RenderDescription(html, providableObject, maxLengthOfDescription);
                    html.AppendClosingTag("a");
                }
            }
            return;
        }

        /// <summary>
        /// Renders a description text.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="providableObject">providable object to
        /// render description text for</param>
        /// <param name="maxLength">maximum length of description
        /// text</param>
        protected virtual void RenderDescription(HtmlWriter html, IProvidableObject providableObject, int maxLength) {
            string description;
            var descriptionField = providableObject.FindPresentableField(this.View.DescriptionKey) as IPresentableFieldForElement;
            if (null == descriptionField) {
                description = null;
            } else {
                description = descriptionField.GetValueAsPlainText();
            }
            if (!string.IsNullOrEmpty(description)) {
                html.AppendOpeningTag("p");
                description = TextUtility.Truncate(description, maxLength);
                html.AppendHtmlEncoded(description);
                html.AppendClosingTag("p");
            }
            return;
        }

        /// <summary>
        /// Renders an image.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="providableObject">providable object to
        /// render image for</param>
        /// <returns>true if image was rendered, false otherwise</returns>
        protected virtual bool RenderImage(HtmlWriter html, IProvidableObject providableObject) {
            bool isImageRendered = false;
            var imageField = providableObject.FindPresentableField(this.View.ImageKey) as IPresentableFieldForElement<ImageFile>;
            if (null != imageField?.Value) {
                var attributes = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("height", this.View.SideLength.ToString(CultureInfo.InvariantCulture)),
                    new KeyValuePair<string, string>("src", providableObject.Id.ToString("N") + "/" + Controllers.FileController.GetUrlOf(this.FileBaseDirectory, imageField.Value, ResizeType.CropToSquare, this.View.SideLength)),
                    new KeyValuePair<string, string>("width", this.View.SideLength.ToString(CultureInfo.InvariantCulture))
                };
                html.AppendSelfClosingTag("img", attributes);
                isImageRendered = true;
            }
            return isImageRendered;
        }

        /// <summary>
        /// Renders a title text.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="providableObject">providable object to
        /// render title text for</param>
        protected virtual void RenderTitle(HtmlWriter html, IProvidableObject providableObject) {
            html.AppendOpeningTag("h1");
            html.AppendHtmlEncoded(providableObject.GetTitle());
            html.AppendClosingTag("h1");
            return;
        }

    }

}