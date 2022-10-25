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

namespace Framework.Presentation.Forms {

    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Fields;

    /// <summary>
    /// Represents a list view for a collection of elements.
    /// </summary>
    public class ListTableView : PersistentObject, IListTableView {

        /// <summary>
        /// Type of chart to generate.
        /// </summary>
        public ChartType ChartType {
            get { return (ChartType)this.chartType.Value; }
            set { this.chartType.Value = (int)value; }
        }
        private readonly PersistentFieldForInt chartType =
            new PersistentFieldForInt(nameof(ChartType), (int)ChartType.None);

        /// <summary>
        /// Description.
        /// </summary>
        public string Description {
            get { return this.description.Value; }
            set { this.description.Value = value; }
        }
        private readonly PersistentFieldForString description =
            new PersistentFieldForString(nameof(Description));

        /// <summary>
        /// Depth of automatic expansion.
        /// </summary>
        public byte ExpansionDepth {
            get { return this.expansionDepth.Value; }
            set { this.expansionDepth.Value = value; }
        }
        private readonly PersistentFieldForByte expansionDepth =
            new PersistentFieldForByte(nameof(ExpansionDepth), 0);

        /// <summary>
        /// Number of groupings to apply.
        /// </summary>
        public byte Groupings {
            get { return this.groupings.Value; }
            set { this.groupings.Value = value; }
        }
        private readonly PersistentFieldForByte groupings =
            new PersistentFieldForByte(nameof(Groupings), 0);

        /// <summary>
        /// True if list table should calculate counts for groupings,
        /// false otherwise.
        /// </summary>
        public bool HasCounts {
            get { return this.hasCounts.Value; }
            set { this.hasCounts.Value = value; }
        }
        private readonly PersistentFieldForBool hasCounts =
            new PersistentFieldForBool(nameof(HasCounts), true);

        /// <summary>
        /// True if list table should calculate totals, false
        /// otherwise.
        /// </summary>
        public bool HasTotals {
            get { return this.hasTotals.Value; }
            set { this.hasTotals.Value = value; }
        }
        private readonly PersistentFieldForBool hasTotals =
            new PersistentFieldForBool(nameof(HasTotals), false);

        /// <summary>
        /// Indicates whether list should be visible.
        /// </summary>
        public bool IsListVisible {
            get { return this.isListVisible.Value; }
            set { this.isListVisible.Value = value; }
        }
        private readonly PersistentFieldForBool isListVisible =
            new PersistentFieldForBool(nameof(IsListVisible), true);

        /// <summary>
        /// Display title of view.
        /// </summary>
        public string Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PersistentFieldForString title =
            new PersistentFieldForString(nameof(Title));

        /// <summary>
        /// List of fields for elements contained in this view.
        /// </summary>
        public IConvenientList<ViewField> ViewFields {
            get {
                return this.viewFields;
            }
        }
        private readonly PersistentFieldForPersistentObjectCollection<ViewField> viewFields;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ListTableView()
            : base() {
            this.RegisterPersistentField(this.chartType);
            this.RegisterPersistentField(this.description);
            this.RegisterPersistentField(this.expansionDepth);
            this.RegisterPersistentField(this.groupings);
            this.RegisterPersistentField(this.hasCounts);
            this.RegisterPersistentField(this.hasTotals);
            this.RegisterPersistentField(this.isListVisible);
            this.RegisterPersistentField(this.title);
            this.viewFields = new PersistentFieldForPersistentObjectCollection<ViewField>(nameof(this.ViewFields), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.viewFields);
        }

    }

}