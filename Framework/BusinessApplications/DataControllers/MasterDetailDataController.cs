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
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract base class for master/detail data controllers.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public abstract class MasterDetailDataController<T> : FormDataController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="dataProvider">provider of master/detail data</param>
        public MasterDetailDataController(DataProvider<T> dataProvider)
            : base(dataProvider) {
            // nothing to do
        }

        /// <summary>
        /// Gets key chains to be preloaded for list table view.
        /// </summary>
        /// <returns>key chains to be preloaded for list table view</returns>
        public abstract IList<string[]> GetKeyChainsToPreloadForListTableView();

        /// <summary>
        /// Gets the buttons to show on list table.
        /// </summary>
        /// <returns>buttons to show on list table</returns>
        public abstract IEnumerable<ActionButton> GetListTableButtons();

        /// <summary>
        /// Gets the display title of a specific hashed type.
        /// </summary>
        /// <param name="hashedType">hashed type to get display title
        /// for</param>
        /// <returns>display title of specific hashed type</returns>
        public sealed override string GetTitleOfNewElement(string hashedType) {
            string title = null;
            foreach (var button in this.GetListTableButtons()) {
                var newButton = button as NewButton<T>;
                if (null != newButton && newButton.HashedType == hashedType) {
                    title = newButton.Title;
                    break;
                }
            }
            return title;
        }

        /// <summary>
        /// Gets the actual type of a specific hashed type.
        /// </summary>
        /// <param name="hashedType">hashed type to get actual type
        /// for</param>
        /// <returns>actual type of specific hashed type</returns>
        public sealed override Type GetTypeOfNewElement(string hashedType) {
            Type type = null;
            foreach (var button in this.GetListTableButtons()) {
                var newButton = button as NewButton<T>;
                if (null != newButton && newButton.HashedType == hashedType) {
                    type = newButton.ElementType;
                    break;
                }
            }
            return type;
        }

    }

}