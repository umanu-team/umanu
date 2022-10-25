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

    using Framework.BusinessApplications.Web;
    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web.Controllers;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Control for search field with optional facets.
    /// </summary>
    /// <typeparam name="T">type of providable objects to be found</typeparam>
    public class SearchFieldPane<T> : CascadedControl where T : class, IProvidableObject {

        /// <summary>
        /// Absolute path of http request.
        /// </summary>
        private string absolutePath;

        /// <summary>
        /// Base directory for files.
        /// </summary>
        private readonly string fileBaseDirectory;

        /// <summary>
        /// Optional lookup provider for autocompletion suggestions.
        /// </summary>
        public SearchSuggestionProvider LookupProvider { get; set; }

        /// <summary>
        /// Wrapper of all relational operators to be applied.
        /// </summary>
        private readonly PresentableObject queryOperators;

        /// <summary>
        /// Wrapper of all values to be filtered by.
        /// </summary>
        private readonly PresentableObject queryValues;

        /// <summary>
        /// List of search facets.
        /// </summary>
        private readonly SearchFacetCollection searchFacets;

        /// <summary>
        /// Current query to be processed.
        /// </summary>
        private SearchQueryResolver<T> searchQueryResolver;

        /// <summary>
        /// URL of endpoint for seach suggestions.
        /// </summary>
        public const string SuggestionsEndpointName = "suggestions";

        /// <summary>
        /// Factory for building web controls.
        /// </summary>
        private readonly WebFactory webFactory;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public SearchFieldPane()
            : this(null, null, null, null) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="searchQueryResolver">resolver for providable
        /// objects based on query string</param>
        /// <param name="facetedSearchView">view for faceted search</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="webFactory">factory for building web
        /// controls</param>
        public SearchFieldPane(SearchQueryResolver<T> searchQueryResolver, IFacetedSearchView facetedSearchView, string fileBaseDirectory, WebFactory webFactory)
            : base("form") {
            this.fileBaseDirectory = fileBaseDirectory;
            this.queryOperators = new PresentableObject();
            this.queryValues = new PresentableObject();
            if (null == facetedSearchView) {
                this.searchFacets = new SearchFacetCollection();
            } else {
                this.searchFacets = facetedSearchView.SearchFacets;
            }
            this.searchQueryResolver = searchQueryResolver;
            this.webFactory = webFactory;
        }

        /// <summary>
        /// Creates all child controls. This is called prior to
        /// HandleEvents().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(HttpRequest httpRequest) {
            if (null == this.searchQueryResolver) {
                this.searchQueryResolver = new SearchQueryResolver<T>(httpRequest, null, null, ulong.MaxValue);
            }
            this.absolutePath = httpRequest.Url.AbsolutePath;
            WebControl fieldset = null;
            foreach (var searchFacetWithReplacedKey in this.searchFacets.WithReplacedKeys) {
                if (null == fieldset) {
                    fieldset = new WebControl("div");
                    fieldset.CssClasses.Add("fieldset");
                    this.Controls.Add(fieldset);
                }
                var presentableField = searchFacetWithReplacedKey.CreatePresentableField(this.queryValues);
                this.queryValues.AddPresentableField(presentableField);
                fieldset.AddChildControl(this.CreateFieldForQueryOperator("o-" + searchFacetWithReplacedKey.Key.Substring(2), presentableField.ContentBaseType));
                var webField = this.webFactory.BuildFieldFor(presentableField, searchFacetWithReplacedKey, this.queryValues, null, string.Empty, string.Empty, PostBackState.ValidPostBack, this.fileBaseDirectory);
                webField.CssClasses.Add("value");
                fieldset.AddChildControl(webField);
            }
            base.CreateChildControls(httpRequest);
            return;
        }

        /// <summary>
        /// Creates a new web field for a relational operator to be
        /// applied on a search facet.
        /// </summary>
        /// <param name="key">key to set for new web field</param>
        /// <param name="contentBaseType">base type of value to
        /// compare to</param>
        /// <returns>new web field for relational operator to be
        /// applied on search facet</returns>
        private WebFieldForChoice CreateFieldForQueryOperator(string key, Type contentBaseType) {
            var presentableField = new PresentableFieldForNullableInt(this.queryOperators, key);
            this.queryOperators.AddPresentableField(presentableField);
            var viewField = new ViewFieldForStringChoice(null, key, Mandatoriness.Optional);
            var optionProvider = new StringOptionDictionary();
            if (TypeOf.DateTime == contentBaseType || TypeOf.NullableDateTime == contentBaseType) {
                optionProvider.DisplayValueForNull = "<";
                optionProvider.Add(((int)RelationalOperator.IsGreaterThanOrEqualTo).ToString(CultureInfo.InvariantCulture), "≥");
            } else {
                if (TypeOf.String == contentBaseType) {
                    optionProvider.DisplayValueForNull = "≈";
                    optionProvider.Add(((int)RelationalOperator.IsEqualTo).ToString(CultureInfo.InvariantCulture), "=");
                } else {
                    optionProvider.DisplayValueForNull = "=";
                }
                optionProvider.Add(((int)RelationalOperator.IsNotEqualTo).ToString(CultureInfo.InvariantCulture), "≠");
                if (TypeOf.Byte == contentBaseType
                    || TypeOf.Char == contentBaseType
                    || TypeOf.Decimal == contentBaseType
                    || TypeOf.Double == contentBaseType
                    || TypeOf.Float == contentBaseType
                    || TypeOf.Int == contentBaseType
                    || TypeOf.Long == contentBaseType
                    || TypeOf.NullableByte == contentBaseType
                    || TypeOf.NullableChar == contentBaseType
                    || TypeOf.NullableDecimal == contentBaseType
                    || TypeOf.NullableDouble == contentBaseType
                    || TypeOf.NullableFloat == contentBaseType
                    || TypeOf.NullableInt == contentBaseType
                    || TypeOf.NullableLong == contentBaseType
                    || TypeOf.NullableSByte == contentBaseType
                    || TypeOf.NullableShort == contentBaseType
                    || TypeOf.NullableUInt == contentBaseType
                    || TypeOf.NullableULong == contentBaseType
                    || TypeOf.NullableUShort == contentBaseType
                    || TypeOf.SByte == contentBaseType
                    || TypeOf.Short == contentBaseType
                    || TypeOf.UInt == contentBaseType
                    || TypeOf.ULong == contentBaseType
                    || TypeOf.UShort == contentBaseType) {
                    optionProvider.Add(((int)RelationalOperator.IsLessThan).ToString(CultureInfo.InvariantCulture), "<");
                    optionProvider.Add(((int)RelationalOperator.IsLessThanOrEqualTo).ToString(CultureInfo.InvariantCulture), "≤");
                    optionProvider.Add(((int)RelationalOperator.IsGreaterThanOrEqualTo).ToString(CultureInfo.InvariantCulture), "≥");
                    optionProvider.Add(((int)RelationalOperator.IsGreaterThan).ToString(CultureInfo.InvariantCulture), ">");
                }
            }
            viewField.OptionProvider = optionProvider;
            var webField = new WebFieldForChoice(presentableField, viewField, FieldRenderMode.Form, this.queryOperators, this.webFactory.OptionDataProvider, null, string.Empty, string.Empty, PostBackState.ValidPostBack);
            webField.CssClasses.Add("operator");
            return webField;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            yield return new KeyValuePair<string, string>("action", this.absolutePath);
            yield return new KeyValuePair<string, string>("enctype", "application/x-www-form-urlencoded");
            yield return new KeyValuePair<string, string>("method", "post");
        }

        /// <summary>
        /// Handles all server-side postback events. This is called
        /// after CreateChildControls(), but prior to Render().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        /// <param name="httpResponse">HTTP response for client</param>
        public override void HandleEvents(HttpRequest httpRequest, HttpResponse httpResponse) {
            base.HandleEvents(httpRequest, httpResponse);
            if ("POST" == httpRequest.HttpMethod.ToUpperInvariant()) {
                this.searchQueryResolver.ReplaceKeysInFilterCriteria(this.queryOperators, this.queryValues);
                var query = this.searchQueryResolver.BuildQueryString();
                RedirectionController.RedirectRequest(httpResponse, this.absolutePath + query);
            }
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.AppendOpeningTag("div", "search");
            var inputAttributes = new Dictionary<string, string>(7) {
                { "autofocus", "autofocus" },
                { "name", "search" },
                { "placeholder", System.Web.HttpUtility.HtmlEncode(Resources.Search) + "..." }
            };
            if (null != this.LookupProvider) {
                inputAttributes.Add("data-ajaxlist", SearchFieldPane<T>.SuggestionsEndpointName + ".json");
                inputAttributes.Add("data-allowfillin", "1");
            }
            var combinedQuery = this.searchQueryResolver.GetSearchParameter();
            if (!string.IsNullOrEmpty(combinedQuery)) {
                inputAttributes.Add("value", System.Web.HttpUtility.HtmlEncode(combinedQuery));
            }
            inputAttributes.Add("type", "text"); // type "search" would be better semantically but looks ugly on iOS: https://css-tricks.com/webkit-html5-search-inputs/
            html.AppendSelfClosingTag("input", inputAttributes); // self-closing tags are not possible via child controls
            var buttonAttributes = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("type", "submit")
            };
            html.AppendOpeningTag("button", buttonAttributes);
            html.Append("🔍");
            html.AppendClosingTag("button");
            base.RenderChildControls(html);
            html.AppendClosingTag("div");
            return;
        }

    }

}