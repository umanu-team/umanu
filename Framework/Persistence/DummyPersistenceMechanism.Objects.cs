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

namespace Framework.Persistence {

    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    // Partial class for object operations of persistence mechanism.
    public partial class DummyPersistenceMechanism {

        /// <summary>
        /// Adds an object to the persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to add</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        /// <returns>true if object was added to persistence
        /// mechanism successfully, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        protected internal override bool AddObject(PersistentObject persistentObject, string internalContainerName, IList<PersistentObject> potentialBrokenReferences) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether a container contains a specific
        /// object - including containers of sub types. Permissions
        /// are ignored.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="id">specific ID to look for</param>
        /// <returns>true if specific ID is contained, false
        /// otherwise</returns>
        internal override bool ContainsID(string internalContainerName, Guid id) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks the existence of references to a persistent object
        /// regardless of permissions.
        /// </summary>
        /// <param name="persistentObject">persistent object to check
        /// existence of references to for</param>
        /// <returns>true if references to persistent object exist,
        /// false otherwise</returns>
        internal override bool ContainsReferencingPersistentObjectsTo(PersistentObject persistentObject) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Counts objects of a certain type in persistence mechanism
        /// - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <returns>pairs of group and number of objects of group</returns>
        /// <typeparam name="T">type of persistent objects to count</typeparam>
        internal override int CountObjects<T>(string internalContainerName, FilterCriteria filterCriteria) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Counts objects of a certain type in persistence mechanism
        /// - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="groupBy">field names to group table rows by</param>
        /// <returns>pairs of group and number of objects of group</returns>
        /// <typeparam name="T">type of persistent objects to count</typeparam>
        internal override IDictionary<string[], int> CountObjects<T>(string internalContainerName, FilterCriteria filterCriteria, string[] groupBy) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all matching persistent objects from persistence
        /// mechanism - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="fullTextQuery">full-text query to select
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" ist the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <typeparam name="T">type of persistent object to find</typeparam>
        internal override ReadOnlyCollection<T> Find<T>(string internalContainerName, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the average values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="fieldNames">specific set of properties to
        /// find average values for</param>
        /// <returns>average values of specific properties</returns>
        /// <typeparam name="T">type of persistent object to find
        /// average values for</typeparam>
        internal override ReadOnlyCollection<object> FindAverageValues<T>(string internalContainerName, FilterCriteria filterCriteria, string[] fieldNames) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the complement set of all matching persistent
        /// objects from this container - including containers of sub
        /// types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="fullTextQuery">full-text query to select
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" ist the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <returns>matching persistent objects from persistence
        /// mechanism</returns>
        /// <typeparam name="T">type of persistent object to find</typeparam>
        internal override ReadOnlyCollection<T> FindComplement<T>(string internalContainerName, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds all distinct combinations of a specific set of
        /// properties - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort the distinct
        /// result combinations by</param>
        /// <param name="fieldNames">specific set of properties to
        /// find all distinct combinations for</param>
        /// <returns>all distinct combinations of a specific set of
        /// properties</returns>
        /// <typeparam name="T">type of persistent object to find
        /// distinct values for</typeparam>
        internal override ReadOnlyCollection<object[]> FindDistinctValues<T>(string internalContainerName, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string[] fieldNames) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the sum of values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="fieldNames">specific set of properties to
        /// find sums of values for</param>
        /// <returns>sums of values of specific properties</returns>
        /// <typeparam name="T">type of persistent object to find
        /// sums of values for</typeparam>
        internal override ReadOnlyCollection<object> FindSumsOfValues<T>(string internalContainerName, FilterCriteria filterCriteria, string[] fieldNames) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all references to a persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to get
        /// references to for</param>
        /// <returns>all references to persistent object</returns>
        internal override ReadOnlyCollection<PersistentObject> GetReferencingPersistentObjectsTo(PersistentObject persistentObject) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates whether an object with a specific ID has been
        /// removed before.
        /// </summary>
        /// <param name="id">ID of object to check removal state for</param>
        /// <returns>true if object with ID has been removed before,
        /// false otherwise</returns>
        public override bool IsIdDeleted(Guid id) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Preloads the state of multiple persistent objects from
        /// persistence mechanism.
        /// </summary>
        /// <param name="sampleObject">sample object of common base
        /// class of objects to be preloaded</param>
        /// <param name="persistentObjects">persistent objects to
        /// preload</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        internal override void PreloadObjects(PersistentObject sampleObject, IEnumerable<PersistentObject> persistentObjects, string internalContainerName, IEnumerable<string[]> keyChains) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a specific object from persistence mechanism and
        /// stores its id for replication scenarios.
        /// </summary>
        /// <param name="persistentObject">object to remove</param>
        /// <param name="isToBeRemovedIfNotReferencedOnly">true if
        /// object is to be removed if not referenced by other
        /// objects only, false otherwise</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, null if it was not removed because
        /// it is referenced by other objects, false otherwise or if
        /// object was not contained in persistence mechanism</returns>
        protected internal override bool? RemoveObject(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly, string internalContainerName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a specific object and all child objects to be
        /// removed cascadedly from persistence mechanism
        /// immediately.
        /// </summary>
        /// <param name="persistentObject">object to remove
        /// cascadedly</param>
        /// <param name="isToBeRemovedIfNotReferencedOnly">true if
        /// object is to be removed if not referenced by other
        /// objects only, false otherwise</param>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, null if it was not removed because
        /// it is referenced by other objects, false otherwise or if
        /// object was not contained in persistence mechanism</returns>
        internal override bool? RemoveObjectCascadedly(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// persistent objects from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal override void RetrieveFieldForCollectionOfElements(PersistentFieldForCollection persistentField, string internalContainerName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// elements from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal override void RetrieveFieldForCollectionOfPersistentObjects(PersistentFieldForPersistentObjectCollection persistentField, string internalContainerName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the state of all fields for elements of
        /// persistent object from persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// retrieve fields for elements for</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal override void RetrieveFieldsForElements(PersistentObject persistentObject, string internalContainerName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the state of a field for a persistent object
        /// from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal override void RetrieveFieldForPersistentObject(PersistentFieldForPersistentObject persistentField, string internalContainerName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the state of a persistent object from
        /// persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal override void RetrieveObject(PersistentObject persistentObject, string internalContainerName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a persistent object in persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to update</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        /// <returns>true if object was updated in persistence
        /// mechanism successfully, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        protected internal override bool UpdateObject(PersistentObject persistentObject, string internalContainerName, IList<PersistentObject> potentialBrokenReferences) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the value for created at of a persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// update value for created at for</param>
        /// <param name="createdAt">new date/time value to be set for
        /// created at</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        internal override bool UpdateObjectCreatedAt(PersistentObject persistentObject, DateTime createdAt) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the value for created by of a persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// update value for created by for</param>
        /// <param name="createdBy">new user value to be set for
        /// created by</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        internal override bool UpdateObjectCreatedBy(PersistentObject persistentObject, IUser createdBy) {
            throw new NotImplementedException();
        }

    }

}