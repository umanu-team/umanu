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

namespace Framework.BusinessApplications.DataProviders {

    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using System;

    /// <summary>
    /// Read-only data provider for a persistent type.
    /// </summary>
    /// <typeparam name="T">type of data to provide</typeparam>
    public class PersistentReadOnlyDataProvider<T> : PersistentDataProvider<T> where T : PersistentObject, IProvidableObject, new() {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        /// <param name="baseFilterCriteria">base filter criteria to
        /// apply</param>
        public PersistentReadOnlyDataProvider(PersistenceMechanism persistenceMechanism, FilterCriteria baseFilterCriteria)
            : base(persistenceMechanism, baseFilterCriteria) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        /// <param name="baseFilterCriteria">base filter criteria to
        /// apply</param>
        /// <param name="baseSortCriterion">base sort criterion to
        /// apply</param>
        public PersistentReadOnlyDataProvider(PersistenceMechanism persistenceMechanism, FilterCriteria baseFilterCriteria, SortCriterion baseSortCriterion)
            : base(persistenceMechanism, baseFilterCriteria, baseSortCriterion) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        /// <param name="baseFilterCriteria">base filter criteria to
        /// apply</param>
        /// <param name="baseSortCriteria">base sort criteria to
        /// apply</param>
        public PersistentReadOnlyDataProvider(PersistenceMechanism persistenceMechanism, FilterCriteria baseFilterCriteria, SortCriterionCollection baseSortCriteria)
            : base(persistenceMechanism, baseFilterCriteria, baseSortCriteria) {
            // nothing to do
        }

        /// <summary>
        /// Adds or updates an object in persistence mechanism.
        /// </summary>
        /// <param name="element">object to add or update</param>
        public sealed override void AddOrUpdate(T element) {
            // nothing to do
            return;
        }

        /// <summary>
        /// Creates a new object of a specific type.
        /// </summary>
        /// <param name="type">type of new object</param>
        /// <returns>new object of specified type or null if current
        /// user is not allowed to create objects</returns>
        public sealed override T Create(Type type) {
            // nothing to do
            return null;
        }

        /// <summary>
        /// Deletes an object in persistence mechanism.
        /// </summary>
        /// <param name="element">object to delete</param>
        public sealed override void Delete(T element) {
            // nothing to do
            return;
        }

    }

}