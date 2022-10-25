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
    /// Field for single video file to be presented in a view.
    /// </summary>
    public class ViewFieldForVideoFile : ViewFieldForFile {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForVideoFile()
            : base() {
            this.Initialize();
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForVideoFile(string title, string key, Mandatoriness mandatoriness)
            : base(title, key, mandatoriness) {
            this.Initialize();
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForVideoFile(string title, string[] keyChain, Mandatoriness mandatoriness)
            : base(title, keyChain, mandatoriness) {
            this.Initialize();
        }

        /// <summary>
        /// Initializes the view field.
        /// </summary>
        private void Initialize() {
            this.AcceptedMimeTypes.Add("video/*");
            return;
        }

    }

}