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

namespace Framework.BusinessApplications {

    using Framework.Presentation.Web;

    /// <summary>
    /// Settings for business web pages.
    /// </summary>
    public class BusinessPageSettings : IPageSettings, IRichTextEditorSettings {

        /// <summary>
        /// Path to additional JavaScript file.
        /// </summary>
        public string AdditionalJavaScriptUrl { get; set; }

        /// <summary>
        /// Path to additional style sheet file.
        /// </summary>
        public string AdditionalStyleSheetUrl { get; set; }

        /// <summary>
        /// URL of icon of align left button.
        /// </summary>
        public string AlignTextLeftButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of align right button.
        /// </summary>
        public string AlignTextRightButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of bold button.
        /// </summary>
        public string BoldButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of bullets button.
        /// </summary>
        public string BulletsButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of align center button.
        /// </summary>
        public string CenterTextButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of decrease indent button.
        /// </summary>
        public string DecreaseIndentButtonIconUrl { get; set; }

        /// <summary>
        /// Path to favicon file.
        /// </summary>
        public string FaviconUrl { get; set; }

        /// <summary>
        /// Path to icon with size of 192 x 192 pixels.
        /// </summary>
        public string Icon192Url { get; set; }

        /// <summary>
        /// Path to icon with size of 512 x 512 pixels.
        /// </summary>
        public string Icon512Url { get; set; }

        /// <summary>
        /// URL of icon of increase indent button.
        /// </summary>
        public string IncreaseIndentButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of insert image button.
        /// </summary>
        public string InsertImageButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of insert link button.
        /// </summary>
        public string InsertLinkButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of insert table button.
        /// </summary>
        public string InsertTableButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of insert table column button.
        /// </summary>
        public string InsertTableColumnButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of insert table row button.
        /// </summary>
        public string InsertTableRowButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of italic button.
        /// </summary>
        public string ItalicButtonIconUrl { get; set; }

        /// <summary>
        /// Path to JavaScript file.
        /// </summary>
        public string JavaScriptUrl { get; set; }

        /// <summary>
        /// URL of icon of numbering button.
        /// </summary>
        public string NumberingButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of remove format button.
        /// </summary>
        public string RemoveFormatButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of remove link button.
        /// </summary>
        public string RemoveLinkButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of remove table column button.
        /// </summary>
        public string RemoveTableColumnButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of remove table row button.
        /// </summary>
        public string RemoveTableRowButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of strikethrough button.
        /// </summary>
        public string StrikethroughButtonIconUrl { get; set; }

        /// <summary>
        /// Path to style sheet file.
        /// </summary>
        public string StyleSheetUrl { get; set; }

        /// <summary>
        /// URL of icon of subscript button.
        /// </summary>
        public string SubscriptButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of superscript button.
        /// </summary>
        public string SuperscriptButtonIconUrl { get; set; }

        /// <summary>
        /// Name of tag to use for surrounding the content of the
        /// HTML body - this may be null or empty.
        /// </summary>
        public string SurroundingTagForBodyContent { get; set; }

        /// <summary>
        /// URL of icon of toggle full windwow button.
        /// </summary>
        public string ToggleFullWindowButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of underline button.
        /// </summary>
        public string UnderlineButtonIconUrl { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public BusinessPageSettings() {
            // nothing to do
        }

    }

}