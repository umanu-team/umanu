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

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Properties;

    /// <summary>
    /// Field for single image file to be presented in a view.
    /// </summary>
    public class ViewFieldForImageFile : ViewFieldForFile {

        /// <summary>
        /// Indicates whether automatic rotation of images is
        /// enabled.
        /// </summary>
        public bool HasAutomaticRotationEnabled {
            get { return this.hasAutomaticRotationEnabled.Value; }
            set { this.hasAutomaticRotationEnabled.Value = value; }
        }
        private readonly PersistentFieldForBool hasAutomaticRotationEnabled =
            new PersistentFieldForBool(nameof(HasAutomaticRotationEnabled), true);

        /// <summary>
        /// Minimum height of image file in pixels to be valid.
        /// </summary>
        public ushort MinHeight {
            get { return this.minHeight.Value; }
            set { this.minHeight.Value = value; }
        }
        private readonly PersistentFieldForUShort minHeight =
            new PersistentFieldForUShort(nameof(MinHeight), ushort.MinValue);

        /// <summary>
        /// Minimum side length of longer side of image file in
        /// pixels to be valid.
        /// </summary>
        public ushort MinSideLength {
            get { return this.minSideLength.Value; }
            set { this.minSideLength.Value = value; }
        }
        private readonly PersistentFieldForUShort minSideLength =
            new PersistentFieldForUShort(nameof(MinSideLength), ushort.MinValue);

        /// <summary>
        /// Minimum width of image file in pixels to be valid.
        /// </summary>
        public ushort MinWidth {
            get { return this.minWidth.Value; }
            set { this.minWidth.Value = value; }
        }
        private readonly PersistentFieldForUShort minWidth =
            new PersistentFieldForUShort(nameof(MinWidth), ushort.MinValue);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForImageFile()
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
        /// <param name="minSideLength">minimum side length of longer side
        /// of image file to be valid, in pixels</param>
        public ViewFieldForImageFile(string title, string key, Mandatoriness mandatoriness, ushort minSideLength)
            : base(title, key, mandatoriness) {
            this.Initialize();
            this.MinSideLength = minSideLength;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="minSideLength">minimum side length of longer side
        /// of image file to be valid, in pixels</param>
        public ViewFieldForImageFile(string title, string[] keyChain, Mandatoriness mandatoriness, ushort minSideLength)
            : base(title, keyChain, mandatoriness) {
            this.Initialize();
            this.MinSideLength = minSideLength;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="minWidth">minimum width of image file to be
        /// valid, in pixels</param>
        /// <param name="minHeight">minimum height of image file to
        /// be valid, in pixels</param>
        public ViewFieldForImageFile(string title, string key, Mandatoriness mandatoriness, ushort minWidth, ushort minHeight)
            : base(title, key, mandatoriness) {
            this.Initialize();
            this.MinHeight = minHeight;
            this.MinWidth = minWidth;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="minWidth">minimum width of image file to be
        /// valid, in pixels</param>
        /// <param name="minHeight">minimum height of image file to
        /// be valid, in pixels</param>
        public ViewFieldForImageFile(string title, string[] keyChain, Mandatoriness mandatoriness, ushort minWidth, ushort minHeight)
            : base(title, keyChain, mandatoriness) {
            this.Initialize();
            this.MinHeight = minHeight;
            this.MinWidth = minWidth;
        }

        /// <summary>
        /// Initializes the view field.
        /// </summary>
        private void Initialize() {
            this.RegisterPersistentField(this.hasAutomaticRotationEnabled);
            this.RegisterPersistentField(this.minHeight);
            this.RegisterPersistentField(this.minSideLength);
            this.RegisterPersistentField(this.minWidth);
            this.AcceptedMimeTypes.Add("image/*");
            this.MaxFileSize = 50 * 1048576; // 50 MiB
            return;
        }

        /// <summary>
        /// Returns null if the specified value is valid, an error
        /// message otherwise.
        /// </summary>
        /// <param name="presentableField">presentable field to be
        /// validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>null if the specified value is valid, error
        /// message otherwise</returns>
        public override string Validate(IPresentableFieldForElement presentableField, ValidityCheck validityCheck, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string errorMessage = base.Validate(presentableField, validityCheck, presentableObject, optionDataProvider);
            if (string.IsNullOrEmpty(errorMessage)) {
                var imageFile = presentableField.ValueAsObject as ImageFile;
                if (null != imageFile) {
                    if (imageFile.Height < this.MinSideLength && imageFile.Width < this.MinSideLength) {
                        errorMessage = string.Format(Resources.TheMinimumImageSideLengthOfLongerSideMustBe0, this.MinSideLength);
                    } else if (imageFile.Height < this.MinHeight || imageFile.Width < this.MinWidth) {
                        errorMessage = string.Format(Resources.TheMinimumImageResolutionMustBe0x1, this.MinWidth, this.MinHeight);
                    }
                }
            }
            return errorMessage;
        }

    }

}