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

namespace Framework.BusinessApplications.Buttons {

    using Framework.BusinessApplications.Web.Controllers;
    using Framework.Model;
    using Framework.Presentation;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using Framework.Properties;
    using System.Collections.Generic;

    /// <summary>
    /// Button of action bar for help page.
    /// </summary>
    public class HelpButton : Presentation.Buttons.LinkButton {

        /// <summary>
        /// Text to be displayed on help page.
        /// </summary>
        public MultilingualString HelpText { get; private set; }

        /// <summary>
        /// Indicates whether text is supposed to be rendered as
        /// plain text or as rich text.
        /// </summary>
        public TextRenderMode TextRenderMode { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        ///<param name="targetUrl">target URL of link</param>
        ///<param name="helpText">text to be displayed on help page</param>
        public HelpButton(string targetUrl, MultilingualString helpText)
            : this(Resources.Help, targetUrl, helpText) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        ///<param name="targetUrl">target URL of link</param>
        ///<param name="helpText">text to be displayed on help page</param>
        public HelpButton(string title, string targetUrl, MultilingualString helpText)
            : base(title, targetUrl) {
            this.HelpText = helpText;
            this.TextRenderMode = TextRenderMode.RichTextWithAutomaticHyperlinkDetection;
        }

        /// <summary>
        /// Gets the child controllers for action button.
        /// </summary>
        /// <param name="element">object to get child controllers for</param>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of parent page</param>
        /// <returns>child controllers for form</returns>
        public override IEnumerable<IHttpController> GetChildControllers(IProvidableObject element, IBusinessApplication businessApplication, string absoluteUrl) {
            foreach (var childController in base.GetChildControllers(element, businessApplication, absoluteUrl)) {
                yield return childController;
            }
            yield return new HelpPageController(businessApplication, this.TargetUrl, this.HelpText, this.TextRenderMode);
        }

    }

}