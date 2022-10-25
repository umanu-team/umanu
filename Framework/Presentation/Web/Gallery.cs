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
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>   
    /// Web control for rendering image galleries.
    /// </summary>
    public sealed class Gallery : MasterControl {

        /// <summary>
        /// Description to show on top.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL of base directory for files.
        /// </summary>
        public string FileBaseDirectory { get; private set; }

        /// <summary>
        /// Presentable objects to view/edit.
        /// </summary>
        public IEnumerable<IPresentableObject> PresentableObjects { get; private set; }

        /// <summary>
        /// List table view to render list table for.
        /// </summary>
        public GalleryView View { get; private set; }

        /// <summary>
        /// Factory for building list table controls.
        /// </summary>
        public WebFactory WebFactory { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableObjects">presentable objects to
        /// view/edit</param>
        /// <param name="view">view to render gallery for</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="buildingRule">rules for building controls
        /// for this list table</param>
        /// <param name="richTextEditorSettings">rich-text-editor
        /// settings</param>
        public Gallery(IEnumerable<IPresentableObject> presentableObjects, GalleryView view, string fileBaseDirectory, IOptionDataProvider optionDataProvider, BuildingRule buildingRule, IRichTextEditorSettings richTextEditorSettings)
            : this(presentableObjects, view, fileBaseDirectory, new WebFactory(FieldRenderMode.ListTable, optionDataProvider, buildingRule, richTextEditorSettings)) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableObjects">presentable objects to
        /// view/edit</param>
        /// <param name="view">view to render gallery for</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="webFactory">factory for building web
        /// controls</param>
        public Gallery(IEnumerable<IPresentableObject> presentableObjects, GalleryView view, string fileBaseDirectory, WebFactory webFactory)
            : base("div") {
            this.FileBaseDirectory = fileBaseDirectory;
            this.PresentableObjects = presentableObjects;
            this.WebFactory = webFactory;
            this.View = view;
            webFactory.SetCssClassesForGallery(this);
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            this.RenderDescriptionMessage(html, this.Description);
            foreach (var presentableObject in this.PresentableObjects) {
                var imageField = presentableObject.FindPresentableField(this.View.ViewFieldForImageFile.Key) as IPresentableFieldForElement<ImageFile>;
                if (null != imageField) {
                    var imageFile = imageField.Value;
                    if (null != imageFile) {
                        if (null != this.OnClickUrlDelegate) {
                            var linkAttributes = new KeyValuePair<string, string>[] {
                                new KeyValuePair<string, string>("data-height", imageFile.Height.ToString(CultureInfo.InvariantCulture.NumberFormat)),
                                new KeyValuePair<string, string>("data-width", imageFile.Width.ToString(CultureInfo.InvariantCulture.NumberFormat)),
                                new KeyValuePair<string, string>("href", this.OnClickUrlDelegate(presentableObject)),
                                new KeyValuePair<string, string>("target", "_blank")
                            };
                            html.AppendOpeningTag("a", linkAttributes);
                        }
                        var imageAttributes = new KeyValuePair<string, string>[] {
                            new KeyValuePair<string, string>("height", "128"),
                            new KeyValuePair<string, string>("src", Controllers.FileController.GetUrlOf(this.FileBaseDirectory, imageFile, ResizeType.CropToSquare, 128)),
                            new KeyValuePair<string, string>("width", "128")
                        };
                        html.AppendSelfClosingTag("img", imageAttributes);
                        if (null != this.OnClickUrlDelegate) {
                            html.AppendClosingTag("a");
                        }
                    }
                } else if (!this.WebFactory.IgnoreMissingFields) {
                    throw new KeyNotFoundException("Presentable field for view field with key \"" + this.View.ViewFieldForImageFile.Key + "\" cannot be found. This field is supposed to contain the gallery image of the object.");
                }
            }
            return;
        }

    }

}