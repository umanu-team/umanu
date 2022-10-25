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

namespace Framework.Presentation {

    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using Persistence.Directories;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Interface for data providers of option providers.
    /// </summary>
    public interface IOptionDataProvider {

        /// <summary>
        /// Group of all users.
        /// </summary>
        Group AllUsers { get; }

        /// <summary>
        /// Adds a file to the underlying persistence mechanism of
        /// this option data provider.
        /// </summary>
        /// <param name="file">file to add</param>
        void AddTemporaryFile(TemporaryFile file);

        /// <summary>
        /// Copys this option data provider and sets the security
        /// model of the copied instance to ignore permissions.
        /// </summary>
        /// <returns>copy of this option data provider that ignores
        /// all permissions</returns>
        IOptionDataProvider CopyWithElevatedPrivileges();

        /// <summary>
        /// Checks whether a container for storing persistent
        /// objects of type T exists.
        /// </summary>
        /// <typeparam name="T">type of persistent objects to be
        /// stored in container</typeparam>
        /// <returns>true if container exists, false otherwise</returns>
        bool ContainsContainer<T>() where T : PersistentObject, new();

        /// <summary>
        /// Directory to be used for user resolval.
        /// </summary>
        UserDirectory UserDirectory { get; }

        /// <summary>
        /// Finds all objects matching the filter criteria.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>all matching objects</returns>
        /// <typeparam name="T">type of objects to find</typeparam>
        ReadOnlyCollection<T> Find<T>(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria) where T : PersistentObject, new();

        /// <summary>
        /// Finds all objects matching the filter criteria.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <returns>all matching objects</returns>
        /// <typeparam name="T">type of objects to find</typeparam>
        ReadOnlyCollection<T> Find<T>(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition) where T : PersistentObject, new();

        /// <summary>
        /// Finds all objects matching the filter criteria.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to return</param>
        /// <returns>all matching objects</returns>
        /// <typeparam name="T">type of objects to find</typeparam>
        ReadOnlyCollection<T> Find<T>(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) where T : PersistentObject, new();

        /// <summary>
        /// Finds the average values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="propertyNames">keys of properties to find
        /// average values for</param>
        /// <returns>average values of specific properties</returns>
        /// <typeparam name="T">type of objects to find average
        /// values for</typeparam>
        ReadOnlyCollection<object> FindAverageValues<T>(FilterCriteria filterCriteria, string[] propertyNames) where T : PersistentObject, new();

        /// <summary>
        /// Finds the average values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="propertyKeyChains">key chains of properties
        /// to find average values for</param>
        /// <returns>average values of specific properties</returns>
        /// <typeparam name="T">type of objects to find average
        /// values for</typeparam>
        ReadOnlyCollection<object> FindAverageValues<T>(FilterCriteria filterCriteria, string[][] propertyKeyChains) where T : PersistentObject, new();

        /// <summary>
        /// Finds all distinct combinations of a specific set of
        /// properties.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort the distinct
        /// result combinations by</param>
        /// <param name="propertyNames">keys of properties to find
        /// all distinct combinations for</param>
        /// <returns>all distinct combinations of a specific set of
        /// properties</returns>
        /// <typeparam name="T">type of objects to find distinct
        /// values for</typeparam>
        ReadOnlyCollection<object[]> FindDistinctValues<T>(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, params string[] propertyNames) where T : PersistentObject, new();

        /// <summary>
        /// Finds all distinct combinations of a specific set of
        /// properties.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort the distinct
        /// result combinations by</param>
        /// <param name="propertyKeyChains">keys chains of properties
        /// to find all distinct combinations for</param>
        /// <returns>all distinct combinations of a specific set of
        /// properties</returns>
        /// <typeparam name="T">type of objects to find distinct
        /// values for</typeparam>
        ReadOnlyCollection<object[]> FindDistinctValues<T>(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, params string[][] propertyKeyChains) where T : PersistentObject, new();

        /// <summary>
        /// Finds one object matching the filter criteria.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// object for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <returns>first matching object</returns>
        /// <typeparam name="T">type of object to find</typeparam>
        T FindOne<T>(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria) where T : PersistentObject, new();

        /// <summary>
        /// Finds the sum of values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="propertyNames">keys of properties to find sums
        /// of values for</param>
        /// <returns>sums of values of specific properties</returns>
        /// <typeparam name="T">type of objects to find sums of
        /// values for</typeparam>
        ReadOnlyCollection<object> FindSumsOfValues<T>(FilterCriteria filterCriteria, string[] propertyNames) where T : PersistentObject, new();

        /// <summary>
        /// Finds the sum of values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="propertyKeyChains">key chains of properties
        /// to find sums of values for</param>
        /// <returns>sums of values of specific properties</returns>
        /// <typeparam name="T">type of objects to find sums of
        /// values for</typeparam>
        ReadOnlyCollection<object> FindSumsOfValues<T>(FilterCriteria filterCriteria, string[][] propertyKeyChains) where T : PersistentObject, new();

        /// <summary>
        /// Preloads the state of multiple persistent objects from
        /// persistence mechanism.
        /// </summary>
        /// <param name="items">persistent objects to preload</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        void Preload<T>(IEnumerable<T> items, IEnumerable<string[]> keyChains) where T : PersistentObject, new();

    }

}