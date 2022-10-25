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

    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using Model;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Builder of relational joins for sort criteria and additional
    /// field names.
    /// </summary>
    public sealed class RelationalJoinBuilder {

        /// <summary>
        /// Additional field names to build relational joins for.
        /// </summary>
        public IList<string> AdditionalFieldNames { get; private set; }

        /// <summary>
        /// Filter criteria to build relational joins for.
        /// </summary>
        public FilterCriteria FilterCriteria { get; private set; }

        /// <summary>
        /// Delegate to be called for resolving the internal name of
        /// the persistent container for a specific type.
        /// </summary>
        private GetInternalNameOfContainerForType GetInternalNameOfContainerForTypeMethod { get; set; }

        /// <summary>
        /// Internal name of container to apply join to.
        /// </summary>
        private string InternalNameOfContainer {
            get {
                if (null == this.internalNameOfContainer) {
                    this.internalNameOfContainer = this.GetInternalNameOfContainerForTypeMethod(this.sampleObject.Type);
                }
                return this.internalNameOfContainer;
            }
        }
        private string internalNameOfContainer = null;

        /// <summary>
        /// Sample object of type of objects to apply join to.
        /// </summary>
        private readonly PersistentObject sampleObject;

        /// <summary>
        /// Sort criteria to build relational joins for.
        /// </summary>
        public SortCriterionCollection SortCriteria { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to build
        /// joins for</param>
        /// <param name="sortCriteria">sort criteria to build joins
        /// for</param>
        /// <param name="getInternalNameOfContainerForTypeMethod">delegate
        /// to be called for resolving the internal name of the
        /// persistent container for a specific type</param>
        /// <param name="sampleObject">sample object of type of
        /// objects to apply join to</param>
        public RelationalJoinBuilder(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, GetInternalNameOfContainerForType getInternalNameOfContainerForTypeMethod, PersistentObject sampleObject) {
            this.AdditionalFieldNames = new List<string>();
            this.FilterCriteria = filterCriteria;
            this.GetInternalNameOfContainerForTypeMethod = getInternalNameOfContainerForTypeMethod;
            this.sampleObject = sampleObject;
            this.SortCriteria = sortCriteria;
        }

        /// <summary>
        /// Adds element collection joins for a chain of field names.
        /// </summary>
        /// <param name="fieldNameChain">chain of field names</param>
        /// <param name="relationalJoins">list to add relational
        /// joins to</param>
        /// <param name="lastObjectOfChain">last object of chain</param>
        private void AddElementCollectionJoins(string[] fieldNameChain, PersistentObject lastObjectOfChain, List<RelationalJoin> relationalJoins) {
            string fieldName = fieldNameChain[fieldNameChain.LongLength - 1];
            if (null != lastObjectOfChain.GetPersistentFieldForCollectionOfElements(fieldName)) {
                string parentAlias;
                string childView;
                if (fieldNameChain.Length > 1) {
                    string internalNameOfParentContainer = this.GetInternalNameOfContainerForTypeMethod(lastObjectOfChain.Type);
                    parentAlias = RelationalJoinBuilder.GetAliasForViewOfContainer(internalNameOfParentContainer, relationalJoins);
                    childView = RelationalDatabase.GetViewNameForSubTable(internalNameOfParentContainer, fieldName);
                } else {
                    parentAlias = RelationalJoin.RootTableAlias;
                    childView = RelationalDatabase.GetViewNameForSubTable(this.InternalNameOfContainer, fieldName);
                }
                string relationsAlias = 'r' + (relationalJoins.Count + 1).ToString(CultureInfo.InvariantCulture);
                var relationalJoin = new RelationalJoin(childView, relationsAlias) {
                    FullFieldName = KeyChain.ToKey(fieldNameChain)
                };
                relationalJoin.FieldPredicates.Add(parentAlias + '.' + nameof(PersistentObject.Id), relationsAlias + ".ParentID");
                if (!relationalJoins.Contains(relationalJoin)) {
                    relationalJoins.Add(relationalJoin);
                }
            }
            return;
        }

        /// <summary>
        /// Adds object joins for a chain of field names.
        /// </summary>
        /// <param name="fieldNameChain">chain of field names</param>
        /// <param name="relationalJoins">list to add relational
        /// joins to</param>
        /// <returns>last object of chain</returns>
        private PersistentObject AddObjectJoins(string[] fieldNameChain, List<RelationalJoin> relationalJoins) {
            var lastObjectOfChain = this.sampleObject;
            for (int i = 0; i < fieldNameChain.Length - 1; i++) {
                string fieldName = fieldNameChain[i];
                string parentAlias;
                if (i > 0) {
                    string internalNameOfParentContainer = this.GetInternalNameOfContainerForTypeMethod(lastObjectOfChain.Type);
                    parentAlias = RelationalJoinBuilder.GetAliasForViewOfContainer(internalNameOfParentContainer, relationalJoins);
                } else {
                    parentAlias = RelationalJoin.RootTableAlias;
                }
                bool isForInlineRelation;
                PersistentField field = lastObjectOfChain.GetPersistentFieldForPersistentObject(fieldName);
                if (null == field) {
                    isForInlineRelation = false;
                    field = lastObjectOfChain.GetPersistentFieldForCollectionOfPersistentObjects(fieldName);
                } else {
                    isForInlineRelation = true;
                }
                if (null == field) {
                    throw new FilterException("Sub object in field \"" + fieldName + "\" of type \"" + lastObjectOfChain.GetType() + "\" cannot be resolved.");
                } else {
                    lastObjectOfChain = field.NewItemAsObject() as PersistentObject;
                    string internalNameOfChildContainer = this.GetInternalNameOfContainerForTypeMethod(lastObjectOfChain.Type);
                    internalNameOfChildContainer = RelationalDatabase.GetViewNameForContainer(internalNameOfChildContainer);
                    if (isForInlineRelation) {
                        string relationsAlias = 'r' + (relationalJoins.Count + 1).ToString(CultureInfo.InvariantCulture);
                        var relationalJoin = new RelationalJoin(internalNameOfChildContainer, relationsAlias) {
                            FullFieldName = string.Join(".", fieldNameChain, 0, i + 1)
                        };
                        relationalJoin.FieldPredicates.Add(parentAlias + '.' + fieldName, relationsAlias + '.' + nameof(PersistentObject.Id));
                        if (!relationalJoins.Contains(relationalJoin)) {
                            relationalJoins.Add(relationalJoin);
                        }
                    } else {
                        string relationsAlias1 = 'r' + (relationalJoins.Count + 1).ToString(CultureInfo.InvariantCulture);
                        var relationalJoin1 = new RelationalJoin(RelationalDatabase.CollectionRelationsTableName, relationsAlias1);
                        relationalJoin1.FieldPredicates.Add(parentAlias + '.' + nameof(PersistentObject.Id), relationsAlias1 + ".ParentID");
                        relationalJoin1.StringPredicates.Add(relationsAlias1 + ".ParentField", fieldName);
                        string relationsAlias2 = 'r' + (relationalJoins.Count + 2).ToString(CultureInfo.InvariantCulture);
                        var relationalJoin2 = new RelationalJoin(internalNameOfChildContainer, relationsAlias2) {
                            FullFieldName = string.Join(".", fieldNameChain, 0, i + 1)
                        };
                        relationalJoin2.FieldPredicates.Add(relationsAlias1 + ".ChildID", relationsAlias2 + '.' + nameof(PersistentObject.Id));
                        if (!relationalJoins.Contains(relationalJoin2)) {
                            relationalJoins.Add(relationalJoin1);
                            relationalJoins.Add(relationalJoin2);
                        }
                    }
                }
            }
            return lastObjectOfChain;
        }

        /// <summary>
        /// Gets the alias for the view of a container.
        /// </summary>
        /// <param name="internalNameOfContainer">internal name of
        /// container</param>
        /// <param name="relationalJoins">list of known relational
        /// joins so far</param>
        /// <returns>alias for the view of a container</returns>
        private static string GetAliasForViewOfContainer(string internalNameOfContainer, List<RelationalJoin> relationalJoins) {
            string parentAlias = null;
            string parentView = RelationalDatabase.GetViewNameForContainer(internalNameOfContainer);
            for (int i = relationalJoins.Count - 1; i >= 0; i--) {
                RelationalJoin relationalJoin = relationalJoins[i];
                if (relationalJoin.TableName == parentView) {
                    parentAlias = relationalJoin.TableAlias;
                    break;
                }
            }
            return parentAlias;
        }

        /// <summary>
        /// Builds a collection of relational joins for this
        /// instance.
        /// </summary>
        /// <returns>collection of relational joins</returns>
        public ICollection<RelationalJoin> ToRelationalJoins() {
            var relationalJoins = new List<RelationalJoin>();
            this.ToRelationalJoins(this.FilterCriteria, relationalJoins);
            foreach (var sortCriterion in this.SortCriteria) {
                if (!string.IsNullOrEmpty(sortCriterion.FieldName)) {
                    string[] fieldNameChain = sortCriterion.FieldNameChain;
                    PersistentObject lastObjectOfFilterChain = this.AddObjectJoins(fieldNameChain, relationalJoins);
                    this.AddElementCollectionJoins(fieldNameChain, lastObjectOfFilterChain, relationalJoins);
                }
            }
            foreach (var additionalFieldName in this.AdditionalFieldNames) {
                string[] fieldNameChain = KeyChain.FromKey(additionalFieldName);
                PersistentObject lastObjectOfFilterChain = this.AddObjectJoins(fieldNameChain, relationalJoins);
                this.AddElementCollectionJoins(fieldNameChain, lastObjectOfFilterChain, relationalJoins);
            }
            return relationalJoins;
        }

        /// <summary>
        /// Builds relational joins for the supplied filter criteria.
        /// </summary>
        /// <param name="relationalJoins">list to add relational
        /// joins to</param>
        /// <param name="filterCriteria">filter criteria to build
        /// relational joins for</param>
        private void ToRelationalJoins(FilterCriteria filterCriteria, List<RelationalJoin> relationalJoins) {
            if (!filterCriteria.IsEmpty) {
                if (filterCriteria.HasSubFilterCriteria) {
                    this.ToRelationalJoins(filterCriteria.SubFilterCriteria, relationalJoins);
                } else {
                    string[] fieldNameChain = filterCriteria.FieldNameChain;
                    PersistentObject lastObjectOfFilterChain = this.AddObjectJoins(fieldNameChain, relationalJoins);
                    this.AddElementCollectionJoins(fieldNameChain, lastObjectOfFilterChain, relationalJoins);
                }
                if (!filterCriteria.IsLastFilterOfChain) {
                    this.ToRelationalJoins(filterCriteria.Next, relationalJoins);
                }
            }
            return;
        }

    }

}