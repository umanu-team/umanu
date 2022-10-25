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

namespace Framework.BusinessApplications.Web.Controllers {

    using Framework.Model;
    using Framework.Presentation.Web;
    using Framework.Properties;

    /// <summary>
    /// HTTP controller for responding help pages.
    /// </summary>
    public sealed class HelpPageController : EditableInfoPageController {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start with a slash</param>
        /// <param name="text">text of page</param>
        /// <param name="textRenderMode">indicates whether text is
        /// supposed to be rendered as plain text or as rich text</param>
        public HelpPageController(IBusinessApplication businessApplication, string absoluteUrl, MultilingualString text, TextRenderMode textRenderMode)
            : base(businessApplication, absoluteUrl, new InfoPage(new MultilingualString(Resources.Help), text), textRenderMode) {
            // nothing to do
        }

    }

}