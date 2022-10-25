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

    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Filters;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a facetted search query.
    /// </summary>
    public class FacetedSearchQuery {

        /// <summary>
        /// Combined query to be processed.
        /// </summary>
        public string CombinedQuery { get; private set; }

        /// <summary>
        /// Filter part of query string.
        /// </summary>
        public FilterCriteria FilterQuery { get; private set; }

        /// <summary>
        /// Full-text query part of query string.
        /// </summary>
        public string FullTextQuery { get; private set; }

        /// <summary>
        /// Data provider for option providers.
        /// </summary>
        private readonly IOptionDataProvider optionDataProvider;

        /// <summary>
        /// Separator to be used for separation of full-text part and
        /// filter part of query.
        /// </summary>
        private const string querySeparator = "FILTER ";

        /// <summary>
        /// List of search facets.
        /// </summary>
        private SearchFacetCollection searchFacets { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="search">value of search parameter</param>
        /// <param name="searchFacets">list of search facets</param>
        /// <param name="optionDataProvider">data provider for option
        /// providers</param>
        public FacetedSearchQuery(string search, SearchFacetCollection searchFacets, IOptionDataProvider optionDataProvider)
            : base() {
            this.optionDataProvider = optionDataProvider;
            this.searchFacets = searchFacets;
            this.SetCombinedQuery(search);
        }

        /// <summary>
        /// Adds missing quotation marks to query automatically.
        /// </summary>
        /// <param name="query">query to add missing quotation marks
        /// to</param>
        /// <returns>query with added missing quotation marks</returns>
        private static string AddMissingQuotationMarksTo(string query) {
            query = ' ' + query + ' ';
            uint quotationMarksCount = 0;
            int lastSpacePosition = 0;
            char previousCharakter = ' ';
            for (int i = 1; i < query.Length; i++) {
                char character = query[i];
                if ('"' == character) {
                    quotationMarksCount++;
                } else if (0 == quotationMarksCount % 2) {
                    if (' ' == previousCharakter && '*' == character) {
                        query = query.Insert(i, "\"");
                        for (int j = i; j < query.Length; j++) {
                            if (' ' == query[j]) {
                                query = query.Insert(j, "\"");
                                break;
                            }
                        }
                        quotationMarksCount++;
                    } else if ('*' == previousCharakter && ' ' == character) {
                        query = query.Insert(i, "\"");
                        query = query.Insert(lastSpacePosition + 1, "\"");
                        quotationMarksCount++;
                    }
                }
                if (' ' == character) {
                    lastSpacePosition = i;
                }
                previousCharakter = character;
            }
            return query.Trim(' ');
        }

        /// <summary>
        /// Gets the computable filter criteria for query string.
        /// </summary>
        /// <returns>computable filter criteria for query string</returns>
        public FilterCriteria GetFilterCriteria() {
            var dictionary = new Dictionary<string, string>();
            foreach (var searchFacetWithReplacedKey in this.searchFacets.WithReplacedKeys) {
                var keyChain = this.searchFacets.ReplacedKeysDictionary[searchFacetWithReplacedKey.Key];
                if (searchFacetWithReplacedKey.CreatePresentableField(new PresentableObject()) is PresentableFieldForObject) {
                    keyChain = KeyChain.Concat(keyChain, nameof(IPresentableObject.Id));
                }
                dictionary.Add(searchFacetWithReplacedKey.Title, KeyChain.ToKey(keyChain));
            }
            return this.FilterQuery.TranslateFieldNames(dictionary).TranslateValues(delegate (string fieldName, object value) {
                var translatedValue = value;
                foreach (var searchFacetWithReplacedKey in this.searchFacets.WithReplacedKeys) {
                    var keyChain = this.searchFacets.ReplacedKeysDictionary[searchFacetWithReplacedKey.Key];
                    var presentableField = searchFacetWithReplacedKey.CreatePresentableField(new PresentableObject());
                    bool isForPresentableObject = presentableField is PresentableFieldForObject;
                    if (isForPresentableObject) {
                        keyChain = KeyChain.Concat(keyChain, nameof(IPresentableObject.Id));
                    }
                    if (KeyChain.ToKey(keyChain) == fieldName) {
                        translatedValue = searchFacetWithReplacedKey.ParseReadOnlyValue(translatedValue as string, this.optionDataProvider);
                        if (isForPresentableObject) {
                            if (translatedValue is IPresentableObject presentableObject) {
                                translatedValue = presentableObject.Id;
                            } else if (Guid.TryParse(translatedValue as string, out Guid parsedId)) {
                                translatedValue = parsedId;
                            }
                        } else if (presentableField is PresentableFieldForDateTime || presentableField is PresentableFieldForNullableDateTime) {
                            translatedValue = (translatedValue as DateTime?)?.Ticks;
                        }
                        break;
                    }
                }
                return translatedValue;
            });
        }

        /// <summary>
        /// Replaces the keys in the filter criteria.
        /// </summary>
        /// <param name="queryOperators">wrapper of all relational
        /// operators to be applied</param>
        /// <param name="queryValues">wrapper of all values to be
        /// filtered by</param>
        public void ReplaceKeysInFilterCriteria(PresentableObject queryOperators, PresentableObject queryValues) {
            var filterCriteria = this.FilterQuery;
            foreach (var searchFacetWithReplacedKey in this.searchFacets.WithReplacedKeys) {
                var presentableFieldForValue = queryValues.FindPresentableField(searchFacetWithReplacedKey.Key) as IPresentableFieldForElement;
                if (null != presentableFieldForValue.ValueAsObject) {
                    var presentableFieldForOperator = queryOperators.FindPresentableField("o-" + searchFacetWithReplacedKey.Key.Substring(2)) as PresentableFieldForNullableInt;
                    RelationalOperator relationalOperator;
                    if (presentableFieldForOperator.Value.HasValue) {
                        relationalOperator = (RelationalOperator)presentableFieldForOperator.Value.Value;
                    } else if (TypeOf.DateTime == presentableFieldForValue.ContentBaseType || TypeOf.NullableDateTime == presentableFieldForValue.ContentBaseType) {
                        relationalOperator = RelationalOperator.IsLessThan;
                    } else if (TypeOf.String == presentableFieldForValue.ContentBaseType) {
                        relationalOperator = RelationalOperator.Contains;
                    } else {
                        relationalOperator = RelationalOperator.IsEqualTo;
                    }
                    var displayKey = searchFacetWithReplacedKey.Title;
                    var displayValue = searchFacetWithReplacedKey.GetReadOnlyValueFor(presentableFieldForValue, queryValues, this.optionDataProvider);
                    filterCriteria = filterCriteria.And(displayKey, relationalOperator, displayValue, FilterTarget.IsOtherTextValue);
                }
            }
            this.FilterQuery = filterCriteria;
            this.UpdateCombinedQuery();
            return;
        }

        /// <summary>
        /// Sets the combined query.
        /// </summary>
        /// <param name="combinedQuery">combined query to be set</param>
        private void SetCombinedQuery(string combinedQuery) {
            this.CombinedQuery = combinedQuery;
            this.FilterQuery = FilterCriteria.Empty;
            this.FullTextQuery = this.CombinedQuery;
            if (!string.IsNullOrEmpty(this.CombinedQuery)) {
                this.CombinedQuery = WebFieldForEditableValue.RemoveUnnecessaryWhiteSpace(this.CombinedQuery);
                var filterStartPosition = this.CombinedQuery?.IndexOf(FacetedSearchQuery.querySeparator) ?? -1;
                if (filterStartPosition > -1) {
                    try {
                        var stringFilter = this.CombinedQuery.Substring(filterStartPosition + FacetedSearchQuery.querySeparator.Length);
                        this.FilterQuery = FilterCriteria.FromStringFilter(stringFilter);
                        this.FullTextQuery = this.CombinedQuery.Substring(0, filterStartPosition);
                    } catch (FilterException) {
                        // ignore filter exceptions
                    }
                }
                this.FullTextQuery = FacetedSearchQuery.AddMissingQuotationMarksTo(this.FullTextQuery);
                this.UpdateCombinedQuery();
            }
            return;
        }

        /// <summary>
        /// Updates the combined query based on full-text query and
        /// filter query.
        /// </summary>
        private void UpdateCombinedQuery() {
            this.CombinedQuery = this.FullTextQuery;
            if (!this.FilterQuery.IsEmpty) {
                this.CombinedQuery += ' ' + FacetedSearchQuery.querySeparator + this.FilterQuery;
            }
            return;
        }

    }

}