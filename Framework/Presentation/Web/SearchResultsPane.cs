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

    using Framework.Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Control for search results.
    /// </summary>
    /// <typeparam name="T">type of search results</typeparam>
    public sealed class SearchResultsPane<T> : Control where T : IPresentableObject {

        /// <summary>
        /// Description text to be displayed.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// End of the URL to load on click on a search result.
        /// </summary>
        public string OnClickUrlEnd { get; private set; }

        /// <summary>
        /// Begin of the URL to load on click on a search result.
        /// </summary>
        public string OnClickUrlStart { get; private set; }

        /// <summary>
        /// Query search results are for.
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// List of search results.
        /// </summary>
        public ICollection<T> SearchResults { get; private set; }

        /// <summary>
        /// View to apply on search results.
        /// </summary>
        public SearchResultView SearchResultsView { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="query">query search results are for</param>
        /// <param name="searchResults">list of search results</param>
        /// <param name="description">description text to be
        /// displayed</param>
        /// <param name="searchResultsView">view to apply on search</param>
        /// <param name="onClickUrlStart">begin of the URL to load on
        /// click on a search result</param>
        /// <param name="onClickUrlEnd">end of the URL to load on
        /// click on a search result</param>
        public SearchResultsPane(string query, ICollection<T> searchResults, string description, SearchResultView searchResultsView, string onClickUrlStart, string onClickUrlEnd)
            : base("article") {
            this.CssClasses.Add("results");
            this.Description = description;
            this.OnClickUrlEnd = onClickUrlEnd;
            this.OnClickUrlStart = onClickUrlStart;
            this.Query = query;
            this.SearchResults = searchResults;
            this.SearchResultsView = searchResultsView;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            if (!string.IsNullOrEmpty(this.Description)) {
                html.AppendOpeningTag("p");
                html.AppendHtmlEncoded(this.Description);
                html.AppendClosingTag("p");
            }
            foreach (var searchResult in this.SearchResults) {
                html.AppendOpeningTag("div");
                html.AppendOpeningTag("h1");
                var aAttributes = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("href", this.OnClickUrlStart + searchResult.Id.ToString("N") + this.OnClickUrlEnd)
                };
                html.AppendOpeningTag("a", aAttributes);
                string title;
                if (string.IsNullOrEmpty(this.SearchResultsView.TitleKey)) {
                    var providableSearchResult = searchResult as IProvidableObject;
                    title = providableSearchResult?.GetTitle();
                } else {
                    var titleField = searchResult.FindPresentableField(this.SearchResultsView.TitleKey) as IPresentableFieldForElement;
                    title = titleField?.GetValueAsPlainText();
                }
                if (!string.IsNullOrEmpty(title)) {
                    title = TextUtility.Truncate(title, 160);
                    html.AppendHtmlEncoded(title);
                }
                html.AppendClosingTag("a");
                html.AppendClosingTag("h1");
                string description;
                var descriptionField = searchResult.FindPresentableField(this.SearchResultsView.DescriptionKey) as IPresentableFieldForElement;
                if (null == descriptionField) {
                    description = null;
                } else {
                    description = descriptionField.GetValueAsPlainText();
                }
                if (!string.IsNullOrEmpty(description)) {
                    html.AppendOpeningTag("p");
                    description = TextUtility.Truncate(description, 256);
                    html.AppendHtmlEncoded(description);
                    html.AppendClosingTag("p");
                }
                html.AppendClosingTag("div");
            }
            return;
        }

    }

}