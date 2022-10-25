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

namespace Framework.Presentation.Web {

    /// <summary>
    /// Represents a link attribute for head attribute of an HTML
    /// page.
    /// </summary>
    public sealed class HeadLink {

        /// <summary>
        /// Cross-origin behaviour to apply for resolval of linked
        /// resource.
        /// </summary>
        public string CrossOrigin { get; set; }

        /// <summary>
        /// URL of the linked resource.
        /// </summary>
        public string Href { get; private set; }

        /// <summary>
        /// Relationship of linked resource to page.
        /// </summary>
        public string Rel { get; private set; }

        /// <summary>
        /// Title of linked resource.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// MIME type of linked resource.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="rel">relationship of linked resource to page</param>
        /// <param name="href">URL of the linked resource</param>
        public HeadLink(string rel, string href)
            : base() {
            this.Href = href;
            this.Rel = rel;
        }

    }

}