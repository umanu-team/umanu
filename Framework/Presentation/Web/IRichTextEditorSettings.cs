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

    /// <summary>
    /// Settings for rich text editor.
    /// </summary>
    public interface IRichTextEditorSettings {

        /// <summary>
        /// URL of icon of align left button.
        /// </summary>
        string AlignTextLeftButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of align right button.
        /// </summary>
        string AlignTextRightButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of bold button.
        /// </summary>
        string BoldButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of bullets button.
        /// </summary>
        string BulletsButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of align center button.
        /// </summary>
        string CenterTextButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of decrease indent button.
        /// </summary>
        string DecreaseIndentButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of increase indent button.
        /// </summary>
        string IncreaseIndentButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of insert image button.
        /// </summary>
        string InsertImageButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of insert link button.
        /// </summary>
        string InsertLinkButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of insert table button.
        /// </summary>
        string InsertTableButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of insert table column button.
        /// </summary>
        string InsertTableColumnButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of insert table row button.
        /// </summary>
        string InsertTableRowButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of italic button.
        /// </summary>
        string ItalicButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of numbering button.
        /// </summary>
        string NumberingButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of remove format button.
        /// </summary>
        string RemoveFormatButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of remove link button.
        /// </summary>
        string RemoveLinkButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of remove table column button.
        /// </summary>
        string RemoveTableColumnButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of remove table row button.
        /// </summary>
        string RemoveTableRowButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of strikethrough button.
        /// </summary>
        string StrikethroughButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of subscript button.
        /// </summary>
        string SubscriptButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of superscript button.
        /// </summary>
        string SuperscriptButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of toggle full window button.
        /// </summary>
        string ToggleFullWindowButtonIconUrl { get; set; }

        /// <summary>
        /// URL of icon of underline button.
        /// </summary>
        string UnderlineButtonIconUrl { get; set; }

    }

}