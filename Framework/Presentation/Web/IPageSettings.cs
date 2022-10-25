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
    /// Settings for web pages.
    /// </summary>
    public interface IPageSettings {

        /// <summary>
        /// Path to additional JavaScript file.
        /// </summary>
        string AdditionalJavaScriptUrl { get; set; }

        /// <summary>
        /// Path to additional style sheet file.
        /// </summary>
        string AdditionalStyleSheetUrl { get; set; }

        /// <summary>
        /// Path to favicon file.
        /// </summary>
        string FaviconUrl { get; set; }

        /// <summary>
        /// Path to icon with size of 192 x 192 pixels.
        /// </summary>
        string Icon192Url { get; set; }

        /// <summary>
        /// Path to icon with size of 512 x 512 pixels.
        /// </summary>
        string Icon512Url { get; set; }

        /// <summary>
        /// Path to JavaScript file.
        /// </summary>
        string JavaScriptUrl { get; set; }

        /// <summary>
        /// Path to style sheet file.
        /// </summary>
        string StyleSheetUrl { get; set; }

        /// <summary>
        /// Name of tag to use for surrounding the content of the
        /// HTML body - this may be null or empty.
        /// </summary>
        string SurroundingTagForBodyContent { get; set; }

    }

}