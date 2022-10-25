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

    using System.Runtime.Serialization;

    /// <summary>
    /// Type of chart.
    /// </summary>
    public enum ChartType {

        /// <summary>
        /// No chart.
        /// </summary>
        [EnumMemberAttribute]
        None = 0,

        /// <summary>
        /// Bar chart.
        /// </summary>
        [EnumMemberAttribute]
        Bar = 1,

        /// <summary>
        /// Doughnut chart.
        /// </summary>
        [EnumMemberAttribute]
        Doughnut = 2,

        /// <summary>
        /// Line chart.
        /// </summary>
        [EnumMemberAttribute]
        Line = 3,

        /// <summary>
        /// Pie chart.
        /// </summary>
        [EnumMemberAttribute]
        Pie = 4,

        /// <summary>
        /// Polar area chart.
        /// </summary>
        [EnumMemberAttribute]
        PolarArea = 5,

        /// <summary>
        /// Radar chart.
        /// </summary>
        [EnumMemberAttribute]
        Radar = 6

    }

}