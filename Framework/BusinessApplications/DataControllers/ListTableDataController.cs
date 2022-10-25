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

namespace Framework.BusinessApplications.DataControllers {

    using Framework.BusinessApplications.Buttons;
    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract base class for list table data controllers.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public abstract class ListTableDataController<T> : MasterDetailDataController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="dataProvider">provider of master/detail data</param>
        public ListTableDataController(DataProvider<T> dataProvider)
            : base(dataProvider) {
            // nothing to do
        }

        /// <summary>
        /// Gets a delete button or a references button for an
        /// element dependent on element to be referenced by other
        /// objects.
        /// </summary>
        /// <param name="element">element to get delete button or
        /// references button for</param>
        /// <param name="confirmationMessage">confirmation message to
        /// display on click on delete button</param>
        /// <returns>delete button or references button for element</returns>
        protected ServerSideButton GetDeleteButtonOrReferencesButtonFor(PersistentObject element, string confirmationMessage) {
            ServerSideButton button;
            if (element.HasReferencingPersistentObjects()) { // this method is more efficient than the one with type arrays
                button = new ReferencesButton(Resources.References);
                button.AllowedGroupsForReading.AddRange(element.AllowedGroups.ForReading);
            } else {
                button = new DeleteButton<T>(Resources.Delete, confirmationMessage, this.DataProvider);
                button.AllowedGroupsForReading.AddRange(element.AllowedGroups.ForWriting);
            }
            return button;
        }

        /// <summary>
        /// Gets a delete button or a references button for an
        /// element dependent on element to be referenced by other
        /// objects.
        /// </summary>
        /// <param name="element">element to get delete button or
        /// references button for</param>
        /// <param name="confirmationMessage">confirmation message to
        /// display on click on delete button</param>
        /// <param name="typesOfTopmostPresentableObjects">types of
        /// topmost presentable objects to list as references</param>
        /// <returns>delete button or references button for element</returns>
        protected ServerSideButton GetDeleteButtonOrReferencesButtonFor(PersistentObject element, string confirmationMessage, IEnumerable<Type> typesOfTopmostPresentableObjects) {
            return this.GetDeleteButtonOrReferencesButtonFor(element, confirmationMessage, typesOfTopmostPresentableObjects, new Type[0]);
        }

        /// <summary>
        /// Gets a delete button or a references button for an
        /// element dependent on element to be referenced by other
        /// objects.
        /// </summary>
        /// <param name="element">element to get delete button or
        /// references button for</param>
        /// <param name="confirmationMessage">confirmation message to
        /// display on click on delete button</param>
        /// <param name="typesOfTopmostPresentableObjects">types of
        /// topmost presentable objects to list as references</param>
        /// <param name="typesOfBarrierPresenableObjects">types of
        /// presentable objects to act as barrier when resolving
        /// topmost presentable objects - usually barrier types are
        /// needed to adjust the traversal behaviour on object tree
        /// if child instances can be accessed via multiple paths</param>
        /// <returns>delete button or references button for element</returns>
        protected ServerSideButton GetDeleteButtonOrReferencesButtonFor(PersistentObject element, string confirmationMessage, IEnumerable<Type> typesOfTopmostPresentableObjects, IEnumerable<Type> typesOfBarrierPresenableObjects) {
            ServerSideButton button;
            if (element.HasReferencingPersistentObjects(typesOfTopmostPresentableObjects, typesOfBarrierPresenableObjects)) {
                button = new ReferencesButton(Resources.References, typesOfTopmostPresentableObjects, typesOfBarrierPresenableObjects);
                button.AllowedGroupsForReading.AddRange(element.AllowedGroups.ForReading);
            } else {
                button = new DeleteButton<T>(Resources.Delete, confirmationMessage, this.DataProvider);
                button.AllowedGroupsForReading.AddRange(element.AllowedGroups.ForWriting);
            }
            return button;
        }

        /// <summary>
        /// Gets key chains to be preloaded for a list table view.
        /// </summary>
        /// <param name="sampleElement">sample element to get key
        /// chains to be preloaded for</param>
        /// <param name="listTableView">list table view to get key
        /// chains to be preloaded for</param>
        /// <returns>key chains to be preloaded for list table view</returns>
        public static IList<string[]> GetKeyChainsToPreloadFor(T sampleElement, IListTableView listTableView) {
            var keyChainsToPreload = new List<string[]>();
            if (null != sampleElement) {
                foreach (var viewField in listTableView.ViewFields) {
                    var viewFieldForEditableValue = viewField as ViewFieldForEditableValue;
                    try {
                        if (null != viewFieldForEditableValue && !viewFieldForEditableValue.Key.Contains("[") && sampleElement.FindPresentableField(viewFieldForEditableValue.KeyChain) is PersistentField) {
                            keyChainsToPreload.Add(viewFieldForEditableValue.KeyChain);
                        }
                    } catch (NotImplementedException) {
                        // ignore not implemented exceptions
                    }
                }
            }
            return keyChainsToPreload;
        }

        /// <summary>
        /// Gets key chains to be preloaded for list table view.
        /// </summary>
        /// <returns>key chains to be preloaded for list table view</returns>
        public override IList<string[]> GetKeyChainsToPreloadForListTableView() {
            IList<string[]> keyChainsToPreload;
            var listTableView = this.GetListTableView();
            if (null == listTableView) {
                keyChainsToPreload = new List<string[]>(0);
            } else {
                var type = typeof(T);
                T sampleElement = this.DataProvider.Create(type);
                if (null == sampleElement) {
                    try {
                        sampleElement = Activator.CreateInstance(type) as T;
                    } catch (Exception) {
                        // ignore exceptions
                    }
                }
                keyChainsToPreload = ListTableDataController<T>.GetKeyChainsToPreloadFor(sampleElement, listTableView);
            }
            return keyChainsToPreload;
        }

        /// <summary>
        /// Gets the view to use for list table.
        /// </summary>
        /// <returns>view to use for list table</returns>
        public virtual IListTableView GetListTableView() {
            var listTableView = new ListTableView();
            listTableView.ViewFields.Add(new ViewFieldForTitle(Resources.Title));
            return listTableView;
        }

    }

}