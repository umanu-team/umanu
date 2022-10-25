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

namespace Framework.BusinessApplications.Widgets {

    using Persistence;
    using Persistence.Fields;

    /// <summary>
    /// Represents a dashboard view.
    /// </summary>
    public class DashboardView : PersistentObject {

        /// <summary>
        /// Page title.
        /// </summary>
        public string Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PersistentFieldForString title =
            new PersistentFieldForString(nameof(Title));

        /// <summary>
        /// List of dashboard view widgets.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<ViewWidget> ViewWidgets { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public DashboardView()
            : base() {
            this.RegisterPersistentField(this.title);
            this.ViewWidgets = new PersistentFieldForPersistentObjectCollection<ViewWidget>(nameof(ViewWidgets), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.ViewWidgets);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">page title</param>
        public DashboardView(string title)
            : this() {
            this.Title = title;
        }

    }

}