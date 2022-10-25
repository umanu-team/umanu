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

    /// <summary>
    /// Interface for list view for a collection of elements.
    /// </summary>
    public interface IListTableView {

        /// <summary>
        /// Type of chart to generate.
        /// </summary>
        ChartType ChartType { get; set; }

        /// <summary>
        /// Description to show on top.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Depth of automatic expansion.
        /// </summary>
        byte ExpansionDepth { get; set; }

        /// <summary>
        /// Number of groupings to apply.
        /// </summary>
        byte Groupings { get; set; }

        /// <summary>
        /// True if list table should calculate counts for groupings,
        /// false otherwise.
        /// </summary>
        bool HasCounts { get; set; }

        /// <summary>
        /// True if list table should calculate totals, false
        /// otherwise.
        /// </summary>
        bool HasTotals { get; set; }

        /// <summary>
        /// Indicates whether list should be visible.
        /// </summary>
        bool IsListVisible { get; set; }

        /// <summary>
        /// Title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// List of fields for elements contained in this view.
        /// </summary>        
        IConvenientList<ViewField> ViewFields { get; }

    }

}