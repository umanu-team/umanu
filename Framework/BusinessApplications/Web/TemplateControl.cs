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

namespace Framework.BusinessApplications.Web {

    using Framework.BusinessApplications.TemplateProcessing;
    using Framework.Presentation.Web;

    /// <summary>
    /// Control to be used on web pages based on an HTML template.
    /// For filling in values into the HTML template a subset of
    /// {{ mustache }} tags can be used: https://mustache.github.io/
    /// </summary>
    public class TemplateControl : Control {

        /// <summary>
        /// Dictionary of keys and values to be filled in into
        /// template.
        /// </summary>
        public TemplateDictionary Dictionary {
            get { return this.templateBuilder.Dictionary; }
        }

        /// <summary>
        /// HTML template with placeholders. The following tags are
        /// supported:<br />
        /// 
        /// Variables - A variable like {{foo}} will be replaced by
        /// the value with the key &quot;foo&quot; in the current
        /// context. If there is none, the parent context will be
        /// browsed from the bottom to the top. If still there is
        /// none, the variable will just not be rendered.<br />
        /// 
        /// Sections - A list section is introduced with {{#bar}} and
        /// ends with {{/bar}}. It may contain variables or further
        /// sections. If a value for the key &quot;bar&quot; exists
        /// and is either false or an empty enumerable the contents
        /// of the section will not be rendered. Otherwise, if the
        /// value is true the section will be endered once. If the
        /// value is a non-empty enumerable the contents of the
        /// section will be rendered once per list item and the
        /// context will be set to the current item for each
        /// iteration.
        /// </summary>
        public string Template {
            get { return this.templateBuilder.Template; }
            set { this.templateBuilder.Template = value; }
        }

        /// <summary>
        /// Template builder to be used for processing of template.
        /// </summary>
        private readonly TemplateBuilder templateBuilder;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tagName">HTML tag name of control</param>
        public TemplateControl(string tagName)
            : base(tagName) {
            this.templateBuilder = new TemplateBuilder();
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.Append(this.templateBuilder.ToString());
            return;
        }

    }

}