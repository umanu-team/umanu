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

namespace Framework.Persistence.Rdbms {

    using Framework.Model;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;

    /// <summary>
    /// Builder of relational subqueries for filter criteria.
    /// </summary>
    public sealed class RelationalSubqueryBuilder {

        /// <summary>
        /// Filter criteria to build relational subqueries for.
        /// </summary>
        public FilterCriteria FilterCriteria { get; private set; }

        /// <summary>
        /// Delegate to be called for resolving the internal name of
        /// the persistent container for a specific type.
        /// </summary>
        private GetInternalNameOfContainerForType GetInternalNameOfContainerForTypeMethod { get; set; }

        /// <summary>
        /// Sample object of type of objects to apply filter to.
        /// </summary>
        private readonly PersistentObject sampleObject;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="filterCriteria">filter criteria this
        /// subquery map is for</param>
        /// <param name="getInternalNameOfContainerForTypeMethod">
        /// delegate to be called for resolving the internal name of
        /// the persistent container for a specific type</param>
        /// <param name="sampleObject">sample object of type of
        /// objects to apply filter to</param>
        public RelationalSubqueryBuilder(FilterCriteria filterCriteria, GetInternalNameOfContainerForType getInternalNameOfContainerForTypeMethod, PersistentObject sampleObject)
            : base() {
            this.FilterCriteria = filterCriteria;
            this.GetInternalNameOfContainerForTypeMethod = getInternalNameOfContainerForTypeMethod;
            this.sampleObject = sampleObject;
        }

        /// <summary>
        /// Gets the relational subqueries for filter criteria.
        /// </summary>
        /// <param name="relationalSubqueries">collection to add
        /// relational subqueries to</param>
        /// <param name="filterCriteria">filter criteria to get
        /// relational subqueries for</param>
        private void AddRelationalSubqueries(RelationalSubqueryCollection relationalSubqueries, FilterCriteria filterCriteria) {
            while (null != filterCriteria && !filterCriteria.IsEmpty) {
                if (filterCriteria.HasSubFilterCriteria) {
                    this.AddRelationalSubqueries(relationalSubqueries, filterCriteria.SubFilterCriteria);
                } else {
                    this.AddRelationalSubquery(relationalSubqueries, filterCriteria.FieldName);
                }
                filterCriteria = filterCriteria.Next;
            }
            return;
        }

        /// <summary>
        /// Adds the relational subquery for a field.
        /// </summary>
        /// <param name="relationalSubqueries">collection to add
        /// relational subquery to</param>
        /// <param name="fieldName">name of field</param>
        private void AddRelationalSubquery(RelationalSubqueryCollection relationalSubqueries, string fieldName) {
            if (!relationalSubqueries.Contains(fieldName)) {
                var relationalSubquery = new RelationalSubquery(fieldName);
                var fieldNameChain = KeyChain.FromKey(fieldName);
                var currentObjectOfChain = this.sampleObject;
                for (long i = 0; i < fieldNameChain.LongLength - 1L; i++) {
                    var currentFieldName = fieldNameChain[i];
                    bool isForInlineRelation;
                    PersistentField currentField = currentObjectOfChain.GetPersistentFieldForPersistentObject(currentFieldName);
                    if (null == currentField) {
                        isForInlineRelation = false;
                        currentField = currentObjectOfChain.GetPersistentFieldForCollectionOfPersistentObjects(currentFieldName);
                    } else {
                        isForInlineRelation = true;
                    }
                    if (null == currentField) {
                        throw new FilterException("Sub object in field \"" + currentFieldName + "\" of type \"" + currentObjectOfChain.GetType() + "\" cannot be resolved.");
                    } else {
                        currentObjectOfChain = currentField.NewItemAsObject() as PersistentObject;
                        string internalNameOfChildContainer = this.GetInternalNameOfContainerForTypeMethod(currentObjectOfChain.Type);
                        if (nameof(PersistentObject.AllowedGroups) != currentField.Key) {
                            internalNameOfChildContainer = RelationalDatabase.GetViewNameForContainer(internalNameOfChildContainer);
                        }
                        relationalSubquery.ChildQueries.Add(new RelationalSubqueryChildQuery(currentFieldName, internalNameOfChildContainer, isForInlineRelation));
                    }
                }
                string lastFieldNameOfChain = fieldNameChain[fieldNameChain.LongLength - 1L];
                if (null != currentObjectOfChain.GetPersistentFieldForCollectionOfElements(lastFieldNameOfChain)) {
                    string internalNameOfSubTable = this.GetInternalNameOfContainerForTypeMethod(currentObjectOfChain.Type);
                    relationalSubquery.SubTableView = RelationalDatabase.GetViewNameForSubTable(internalNameOfSubTable, lastFieldNameOfChain);
                }
                relationalSubqueries.Add(relationalSubquery);
            }
            return;
        }

        /// <summary>
        /// Builds a collection of relational joins for the filter
        /// criteria of this instance.
        /// </summary>
        /// <returns>collection of relational joins for filter
        /// criteria</returns>
        public RelationalSubqueryCollection ToRelationalSubqueries() {
            var relationalSubqueries = new RelationalSubqueryCollection();
            this.AddRelationalSubqueries(relationalSubqueries, this.FilterCriteria);
            return relationalSubqueries;
        }

    }

}