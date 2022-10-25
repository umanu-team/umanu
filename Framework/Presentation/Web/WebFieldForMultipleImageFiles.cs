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
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Field control for multiple image files.
    /// </summary>
    public class WebFieldForMultipleImageFiles : WebFieldForMultipleFiles<ImageFile> {

        /// <summary>
        /// View field to build control for.
        /// </summary>
        private readonly ViewFieldForMultipleImageFiles viewFieldForMultipleImageFiles;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableField">presentable field to
        /// build control for</param>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to ID of
        /// field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to ID of
        /// field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        public WebFieldForMultipleImageFiles(IPresentableFieldForCollection presentableField, ViewFieldForMultipleImageFiles viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.CssClasses.Add("imagefield");
            this.viewFieldForMultipleImageFiles = viewField;
        }

        /// <summary>
        /// Gets the attributes of links to a file.
        /// </summary>
        /// <param name="fileId">id of file to get anchor attributes
        /// for</param>
        /// <param name="file">file to get anchor attributes for</param>
        /// <returns>anchor attributes for file</returns>
        protected override IDictionary<string, string> GetFileLinkAnchorAttributesFor(Guid fileId, ImageFile file) {
            if (FieldRenderMode.ListTable == this.RenderMode) {
                throw new InvalidOperationException("File links cannot be rendered for list tables.");
            }
            return WebFieldForImageFile.GetFileLinkAnchorAttributesFor(this.FileBaseDirectory, fileId, file, this.viewFieldForMultipleImageFiles.HasAutomaticRotationEnabled);
        }

        /// <summary>
        /// Renders a paragraph showing a file name.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="fileId">id of file to render name for</param>
        /// <param name="file">file to render name for</param>
        protected override void RenderFileName(HtmlWriter html, Guid fileId, ImageFile file) {
            var src = Controllers.FileController.GetUrlOf(this.FileBaseDirectory, fileId, file, ResizeType.CropToSquare, 96);
            if (FieldRenderMode.ListTable == this.RenderMode) {
                src = this.TopmostParentPresentableObject.Id.ToString("N") + "/" + src;
            }
            var imageAttributes = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("class", "imagefield"),
                new KeyValuePair<string, string>("height", "96"),
                new KeyValuePair<string, string>("src", src),
                new KeyValuePair<string, string>("title", file.Name),
                new KeyValuePair<string, string>("width", "96")
            };
            html.AppendSelfClosingTag("img", imageAttributes);
            return;
        }

    }

}