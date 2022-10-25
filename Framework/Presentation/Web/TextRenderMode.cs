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

    using System.Runtime.Serialization;

    /// <summary>
    /// Render mode for text.
    /// </summary>
    public enum TextRenderMode {

        /// <summary>
        /// Text is supposed to be rendered as plain text.
        /// </summary>
        [EnumMemberAttribute]
        PlainText = 0,

        /// <summary>
        /// Text is supposed to be rendered as rich text with
        /// automatic hyperlink detection.
        /// </summary>
        [EnumMemberAttribute]
        RichTextWithAutomaticHyperlinkDetection = 1,

        /// <summary>
        /// Text is supposed to be rendered as rich text without
        /// automatic hyperlink detection.
        /// </summary>
        [EnumMemberAttribute]
        RichTextWithoutAutomaticHyperlinkDetection = 2

    }

}