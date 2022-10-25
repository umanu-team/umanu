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

    using Framework.Presentation.Forms;

    /// <summary>
    /// Represents an image to be used in web manifests.
    /// </summary>
    public sealed class WebAppManifestImage : PresentableObject {

        /// <summary>
        /// Purpose image may be used for.
        /// </summary>
        public string Purpose {
            get { return this.purpose.Value; }
            set { this.purpose.Value = value; }
        }
        private readonly PresentableFieldForString purpose;

        /// <summary>
        /// Sizes to use image file for in pixels, e.g. &quot;72x72
        /// 96x96&quot;.
        /// </summary>
        public string Sizes {
            get { return this.sizes.Value; }
            set { this.sizes.Value = value; }
        }
        private readonly PresentableFieldForString sizes;

        /// <summary>
        /// URL of image file.
        /// </summary>
        public string Src {
            get { return this.src.Value; }
            set { this.src.Value = value; }
        }
        private readonly PresentableFieldForString src;

        /// <summary>
        /// Mime type of image file, e.g. &quot;image/png&quot;.
        /// </summary>
        public string Type {
            get { return this.type.Value; }
            set { this.type.Value = value; }
        }
        private readonly PresentableFieldForString type;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="src">URL of image file</param>
        /// <param name="sizes">sizes to use image file for in
        /// pixels, e.g. &quot;72x72 96x96&quot;</param>
        public WebAppManifestImage(string src, string sizes)
            : base() {
            this.purpose = new PresentableFieldForString(this, nameof(this.Purpose));
            this.AddPresentableField(this.purpose);
            this.sizes = new PresentableFieldForString(this, nameof(this.Sizes), sizes);
            this.AddPresentableField(this.sizes);
            this.src = new PresentableFieldForString(this, nameof(this.Src), src);
            this.AddPresentableField(this.src);
            this.type = new PresentableFieldForString(this, nameof(this.Type));
            this.AddPresentableField(this.type);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="src">URL of image file</param>
        /// <param name="sizes">sizes to use image file for in
        /// pixels, e.g. &quot;72x72 96x96&quot;</param>
        /// <param name="purpose">purpose image may be used for</param>
        public WebAppManifestImage(string src, string sizes, string purpose)
            : this(src, sizes) {
            this.Purpose = purpose;
        }

    }

}