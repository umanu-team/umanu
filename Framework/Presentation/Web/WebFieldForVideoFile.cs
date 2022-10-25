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
    /// Field control for an video file.
    /// </summary>
    public class WebFieldForVideoFile : WebFieldForFile<File> {

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
        public WebFieldForVideoFile(IPresentableFieldForElement presentableField, ViewFieldForVideoFile viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            if (FieldRenderMode.Form == this.RenderMode) {
                this.CssClasses.Add("videofield");
            }
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            if (FieldRenderMode.ListTable == this.RenderMode) {
                base.RenderReadOnlyValue(html);
            } else if (this.PresentableField.ValueAsObject is File file && !string.IsNullOrEmpty(file.Name)) {
                this.RenderVideoPlayer(html, file);
            }
            return;
        }

        /// <summary>
        /// Renders a read only video player.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="videoFile">video file to be loaded by player</param>
        private void RenderVideoPlayer(HtmlWriter html, File videoFile) {
            var isDiffNew = this.ComparisonDate.HasValue && null == videoFile.GetVersionValue(this.ComparisonDate);
            if (isDiffNew) {
                html.AppendOpeningTag("span", "diffnew");
            }
            var videoAttributes = new Dictionary<string, string>(2) {
                { "controls", "controls" },
                { "preload", "auto" }
            };
            html.AppendOpeningTag("video", videoAttributes);
            var sourceAttributes = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("src", Controllers.FileController.GetUrlOf(this.FileBaseDirectory, videoFile)),
                new KeyValuePair<string, string>("type", videoFile.MimeType)
            };
            html.AppendSelfClosingTag("source", sourceAttributes);
            html.AppendClosingTag("video");
            if (isDiffNew) {
                html.AppendClosingTag("span");
            }
            return;
        }

    }

}