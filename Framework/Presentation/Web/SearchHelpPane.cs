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

    using Framework.Properties;

    /// <summary>
    /// Control for search results.
    /// </summary>
    public sealed class SearchHelpPane : Control {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public SearchHelpPane()
            : base("article") {
            this.CssClasses.Add("results");
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            SearchHelpPane.RenderHelpBoxDiv(html, "AND", Resources.SearchHelpAndDescription, Resources.SearchHelpAndExample);
            SearchHelpPane.RenderHelpBoxDiv(html, "OR", Resources.SearchHelpOrDescription, Resources.SearchHelpOrExample);
            SearchHelpPane.RenderHelpBoxDiv(html, "AND NOT", Resources.SearchHelpAndNotDescription, Resources.SearchHelpAndNotExample);
            SearchHelpPane.RenderHelpBoxDiv(html, Resources.QuotationMarks, Resources.SearchHelpQuotationMarksDescription, Resources.SearchHelpQuotationMarksExample);
            SearchHelpPane.RenderHelpBoxDiv(html, Resources.Asterisk, Resources.SearchHelpAsteriskCharaktersDescription, Resources.SearchHelpAsteriskCharaktersExample);
            return;
        }

        /// <summary>
        /// Renders a div of a help box.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="headline">headline text</param>
        /// <param name="description">description text</param>
        /// <param name="example">example text</param>
        private static void RenderHelpBoxDiv(HtmlWriter html, string headline, string description, string example) {
            html.AppendOpeningTag("div");
            html.AppendOpeningTag("h1");
            html.AppendHtmlEncoded(headline);
            html.AppendClosingTag("h1");
            html.AppendOpeningTag("p");
            html.AppendHtmlEncoded(description);
            html.AppendClosingTag("p");
            html.AppendOpeningTag("p");
            html.AppendHtmlEncoded(example);
            html.AppendClosingTag("p");
            html.AppendClosingTag("div");
            return;
        }

    }

}