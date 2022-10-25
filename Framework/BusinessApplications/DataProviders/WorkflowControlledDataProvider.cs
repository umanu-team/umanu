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

    using Framework.BusinessApplications.Workflows;
    using Framework.Persistence;
    using Framework.Persistence.Filters;

    /// <summary>
    /// Base class of data providers for workflow controlled types.
    /// </summary>
    /// <typeparam name="T">type of objects to provide</typeparam>
    public abstract class WorkflowControlledDataProvider<T> : PersistentDataProvider<T> where T : WorkflowControlledObject, new() {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        /// <param name="baseFilterCriteria">base filter criteria to
        /// apply</param>
        public WorkflowControlledDataProvider(PersistenceMechanism persistenceMechanism, FilterCriteria baseFilterCriteria)
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
        public WorkflowControlledDataProvider(PersistenceMechanism persistenceMechanism, FilterCriteria baseFilterCriteria, SortCriterion baseSortCriterion)
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
        public WorkflowControlledDataProvider(PersistenceMechanism persistenceMechanism, FilterCriteria baseFilterCriteria, SortCriterionCollection baseSortCriteria)
            : base(persistenceMechanism, baseFilterCriteria, baseSortCriteria) {
            // nothing to do
        }

        /// <summary>
        /// Adds or updates an object in persistence mechanism.
        /// </summary>
        /// <param name="element">object to add or update</param>
        public override void AddOrUpdate(T element) {
            if (null != element?.Workflow && element.Workflow.IsAssociatedObjectNull) {
                throw new WorkflowException("Workflow cannot be saved because associated object is null. Please use the constuctor with a workflow controlled object as parameter for creation of new workflow instances.");
            }
            this.PersistentContainer.AddOrUpdateCascadedly(element);
            var workflowWithElevatedPrivileges = element.WorkflowWithElevatedPrivileges;
            if (null != workflowWithElevatedPrivileges) {
                workflowWithElevatedPrivileges.RetrieveCascadedly();
                workflowWithElevatedPrivileges.Execute();
            }
            this.PersistenceMechanism.RemoveExpiredTemporaryFiles();
            return;
        }

        /// <summary>
        /// Finds the object with a specific ID.
        /// </summary>
        /// <param name="id">ID to get object for</param>
        /// <returns>object with specific ID or null if no
        /// match was found</returns>
        public override T FindOne(string id) {
            return base.FindOne(id)?.Workflow.GetAssociatedObjectForForms() as T;
        }

    }

}