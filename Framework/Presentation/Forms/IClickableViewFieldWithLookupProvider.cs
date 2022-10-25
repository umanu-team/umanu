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

    /// <summary>
    /// Interface for view fields for lookup with URL links for
    /// clickable values.
    /// </summary>
    public interface IClickableViewFieldWithLookupProvider : IViewFieldWithLookupProvider {

        /// <summary>
        /// Internal key chain of this field.
        /// </summary>
        string[] KeyChain { get; }

        /// <summary>
        /// Delegate to get URL link for clickable value.
        /// </summary>
        OnClickUrlDelegate OnClickUrlDelegate { get; set; }

    }

}