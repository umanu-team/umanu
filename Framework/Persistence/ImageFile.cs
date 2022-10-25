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

namespace Framework.Persistence {

    using Framework.Model;
    using Framework.Persistence.Fields;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net.Mime;

    /// <summary>
    /// Image file to be stored in persistence mechanism.
    /// </summary>
    public sealed class ImageFile : File {

        /// <summary>
        /// ID of Exif orientation property.
        /// </summary>
        private const int exifOrientationPropertyId = 0x0112;

        /// <summary>
        /// Indicates whether the remainder of division of width/
        /// height by 16 is zero.
        /// </summary>
        public bool HasOptimizedApectRatio {
            get { return 0 == this.Width % 16 && 0 == this.Height % 16; }
        }

        /// <summary>
        /// Height of image.
        /// </summary>
        public int Height {
            get { return this.height.Value; }
            private set { this.height.Value = value; }
        }
        private readonly PersistentFieldForInt height =
            new PersistentFieldForInt(nameof(Height), 0);

        /// <summary>
        /// Encoder for saving images in JPEG format.
        /// </summary>
        private static ImageCodecInfo JpegEncoder {
            get {
                if (null == ImageFile.jpegEncoder) {
                    foreach (var imageCodecInfo in ImageCodecInfo.GetImageEncoders()) {
                        if (ImageFormat.Jpeg.Guid == imageCodecInfo.FormatID) {
                            ImageFile.jpegEncoder = imageCodecInfo;
                            break;
                        }
                    }
                }
                return ImageFile.jpegEncoder;
            }
        }
        private static ImageCodecInfo jpegEncoder;

        /// <summary>
        /// Length of longer side of image.
        /// </summary>
        public int LongerSideLength {
            get {
                int longerSide;
                if (this.Width > this.Height) {
                    longerSide = this.Width;
                } else {
                    longerSide = this.Height;
                }
                return longerSide;
            }
        }

        /// <summary>
        /// Length of shorter side of image.
        /// </summary>
        public int ShorterSideLength {
            get {
                int shorterSide;
                if (this.Width < this.Height) {
                    shorterSide = this.Width;
                } else {
                    shorterSide = this.Height;
                }
                return shorterSide;
            }
        }

        /// <summary>
        /// Lock for method ToJpegImageFile(...).
        /// </summary>
        private static readonly object toJpegImageFileLock = new object();

        /// <summary>
        /// Number of pending retries of creation of JPEG image
        /// files.
        /// </summary>
        private static long toJpegImageFileRetries = 0L;

        /// <summary>
        /// Difference in rotation against source image.
        /// </summary>
        public RotateFlipType RotationAgainstSourceImage {
            get { return (RotateFlipType)this.rotationAgainstSourceImage.Value; }
            set { this.rotationAgainstSourceImage.Value = (int)value; }
        }
        private readonly PersistentFieldForInt rotationAgainstSourceImage =
            new PersistentFieldForInt(nameof(RotationAgainstSourceImage), (int)RotateFlipType.RotateNoneFlipNone);

        /// <summary>
        /// Width of image.
        /// </summary>
        public int Width {
            get { return this.width.Value; }
            private set { this.width.Value = value; }
        }
        private readonly PersistentFieldForInt width =
            new PersistentFieldForInt(nameof(Width), 0);

        /// <summary>
        /// Instatiates a new instance.
        /// </summary>
        public ImageFile()
            : base() {
            this.IsBlobFullTextIndexed = false;
            this.RegisterPersistentField(this.height);
            this.RegisterPersistentField(this.rotationAgainstSourceImage);
            this.RegisterPersistentField(this.width);
            this.PropertyChanged += delegate (object sender, PropertyChangedEventArgs eventArguments) {
                if (nameof(ImageFile.Blob) == eventArguments.PropertyName || KeyChain.ConcatToKey(nameof(ImageFile.Blob), nameof(Persistence.Blob.Bytes)) == eventArguments.PropertyName) {
                    this.Height = 0;
                    this.Width = 0;
                    if (null != this.Bytes) {
                        using (var imageStream = new MemoryStream(this.Bytes)) {
                            try {
                                using (var image = Image.FromStream(imageStream)) {
                                    foreach (var imageCodecInfo in ImageCodecInfo.GetImageEncoders()) {
                                        if (imageCodecInfo.FormatID == image.RawFormat.Guid) {
                                            this.MimeType = imageCodecInfo.MimeType;
                                            break;
                                        }
                                    }
                                    this.Width = image.Width;
                                    this.Height = image.Height;
                                    this.RotationAgainstSourceImage = ImageFile.GetRotateFlipTypeFromExifDataOf(image);
                                    this.TryApplyRotationAndFlipLosslessly();
                                }
                            } catch (ArgumentException exception) {
                                throw new ArgumentException("Value of ImageFile.Bytes does not represent a supported image file.", exception);
                            }
                        }
                    }
                }
                return;
            };
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <param name="bytes">file contents as binary data</param>
        public ImageFile(string name, byte[] bytes)
            : this() {
            this.Name = name;
            this.Bytes = bytes;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="filePath">local system path of file</param>
        /// <param name="fileName">name of file</param>
        public ImageFile(string filePath, string fileName)
            : this() {
            this.LoadFromLocalSystemPath(filePath, fileName);
        }

        /// <summary>
        /// Adds a further rotation to the rotation of this image.
        /// </summary>
        /// <param name="rotateFlipType">type of rotation to add</param>
        public void AddRotationOrFlip(RotateFlipType rotateFlipType) {
            int rotation = ImageFile.GetNumericRotationOf(this.RotationAgainstSourceImage) + ImageFile.GetNumericRotationOf(rotateFlipType);
            if (rotation > 270) {
                rotation -= 360;
            }
            bool isFlippedX = ImageFile.GetIsFlippedXOf(this.RotationAgainstSourceImage) ^ ImageFile.GetIsFlippedXOf(rotateFlipType);
            bool isFlippedY = ImageFile.GetIsFlippedYOf(this.RotationAgainstSourceImage) ^ ImageFile.GetIsFlippedYOf(rotateFlipType);
            this.RotationAgainstSourceImage = ImageFile.GetRotateFlipType(rotation, isFlippedX, isFlippedY);
            return;
        }

        /// <summary>
        /// Calculates new dimensions for this image file.
        /// </summary>
        /// <param name="resizeType">type of resize operation to
        /// apply</param>
        /// <param name="sideLength">new maximum side length</param>
        /// <param name="resizeWidth">width for resizing</param>
        /// <param name="resizeHeight">height for resizing</param>
        /// <param name="finalWidth">final width after cropping and
        /// resizing</param>
        /// <param name="finalHeight">final height after cropping and
        /// resizing</param>
        private void CalculateNewDimensions(ResizeType resizeType, int sideLength, out int resizeWidth, out int resizeHeight, out int finalWidth, out int finalHeight) {
            int oldWidth = this.Width;
            int oldHeight = this.Height;
            bool isAspectRatioToBeOptimized = ResizeType.OptimizeAspectRatioSetLongerSide == resizeType || ResizeType.OptimizeAspectRatioSetWidth == resizeType || ResizeType.OptimizeAspectRatioSetHeight == resizeType;
            if (ResizeType.None == resizeType) {
                finalWidth = resizeWidth = oldWidth;
                finalHeight = resizeHeight = oldHeight;
            } else if (ResizeType.CropToSquare == resizeType) {
                finalWidth = finalHeight = sideLength;
                if (oldWidth < oldHeight) {
                    resizeWidth = sideLength;
                    resizeHeight = (int)Math.Round(sideLength * ((double)this.Height / this.Width));
                } else {
                    resizeWidth = (int)Math.Round(sideLength * ((double)this.Width / this.Height));
                    resizeHeight = sideLength;
                }
            } else if (ResizeType.KeepAspectRatioSetHeight == resizeType || ResizeType.OptimizeAspectRatioSetHeight == resizeType || ((ResizeType.KeepAspectRatioSetLongerSide == resizeType || ResizeType.OptimizeAspectRatioSetLongerSide == resizeType) && oldWidth < oldHeight)) {
                finalHeight = resizeHeight = sideLength;
                resizeWidth = (int)Math.Round(finalHeight * ((double)oldWidth / oldHeight));
                if (isAspectRatioToBeOptimized) {
                    finalWidth = resizeWidth - resizeWidth % 16;
                } else { // keep aspect ratio
                    finalWidth = resizeWidth;
                }
            } else if (ResizeType.KeepAspectRatioSetWidth == resizeType || ResizeType.OptimizeAspectRatioSetWidth == resizeType || ((ResizeType.KeepAspectRatioSetLongerSide == resizeType || ResizeType.OptimizeAspectRatioSetLongerSide == resizeType) && oldWidth >= oldHeight)) {
                finalWidth = resizeWidth = sideLength;
                resizeHeight = (int)Math.Round(finalWidth * ((double)oldHeight / oldWidth));
                if (isAspectRatioToBeOptimized) {
                    finalHeight = resizeHeight - resizeHeight % 16;
                } else { // optimize aspect ratio
                    finalHeight = resizeHeight;
                }
            } else {
                throw new ArgumentException("Resize type \"" + resizeType.ToString() + "\" is unknown.", nameof(resizeType));
            }
            return;
        }

        /// <summary>
        /// Gets the new dimensions of this image file after applying
        /// a resize operation.
        /// </summary>
        /// <param name="resizeType">type of resize operation to
        /// apply</param>
        /// <param name="sideLength">new maximum side length</param>
        /// <param name="width">new width</param>
        /// <param name="height">new height</param>
        public void GetDimensionsAfterResizeOperation(ResizeType resizeType, int sideLength, out int width, out int height) {
            this.CalculateNewDimensions(resizeType, sideLength, out int resizeWidth, out int _, out width, out height);
            if (RotateFlipType.Rotate90FlipNone == this.RotationAgainstSourceImage
                || RotateFlipType.Rotate90FlipX == this.RotationAgainstSourceImage
                || RotateFlipType.Rotate90FlipXY == this.RotationAgainstSourceImage
                || RotateFlipType.Rotate90FlipY == this.RotationAgainstSourceImage
                || RotateFlipType.Rotate270FlipNone == this.RotationAgainstSourceImage
                || RotateFlipType.Rotate270FlipX == this.RotationAgainstSourceImage
                || RotateFlipType.Rotate270FlipXY == this.RotationAgainstSourceImage
                || RotateFlipType.Rotate270FlipY == this.RotationAgainstSourceImage) {
                int newResizeWidth = height;
                height = resizeWidth;
                width = newResizeWidth;
            }
            return;
        }

        /// <summary>
        /// Gets a value indicating whether a rotate flip type is
        /// flipped X.
        /// </summary>
        /// <param name="rotateFlipType">rotate flip type to get
        /// numeric value for</param>
        /// <returns>value indicating whether a rotate flip type is
        /// flipped X</returns>
        private static bool GetIsFlippedXOf(RotateFlipType rotateFlipType) {
            // all other enum keys do not need to be checked because they have the same integer values as the checked ones
            return RotateFlipType.Rotate180FlipX == rotateFlipType
                || RotateFlipType.Rotate180FlipXY == rotateFlipType
                || RotateFlipType.Rotate270FlipX == rotateFlipType
                || RotateFlipType.Rotate270FlipXY == rotateFlipType;
        }

        /// <summary>
        /// Gets a value indicating whether a rotate flip type is
        /// flipped Y.
        /// </summary>
        /// <param name="rotateFlipType">rotate flip type to get
        /// numeric value for</param>
        /// <returns>value indicating whether a rotate flip type is
        /// flipped Y</returns>
        private static bool GetIsFlippedYOf(RotateFlipType rotateFlipType) {
            // all other enum keys do not need to be checked because they have the same integer values as the checked ones
            return RotateFlipType.Rotate180FlipXY == rotateFlipType
                || RotateFlipType.Rotate180FlipY == rotateFlipType
                || RotateFlipType.Rotate270FlipXY == rotateFlipType
                || RotateFlipType.Rotate270FlipY == rotateFlipType;
        }

        /// <summary>
        /// Gets the numeric value in degrees of a rotate flip type.
        /// </summary>
        /// <param name="rotateFlipType">rotate flip type to get
        /// numeric value for</param>
        /// <returns>numeric value in degrees of rotate flip type</returns>
        private static int GetNumericRotationOf(RotateFlipType rotateFlipType) {
            // all other enum keys do not need to be checked because they have the same integer values as the checked ones
            int rotation;
            if (RotateFlipType.Rotate180FlipNone == rotateFlipType
                || RotateFlipType.Rotate180FlipX == rotateFlipType
                || RotateFlipType.Rotate180FlipXY == rotateFlipType
                || RotateFlipType.Rotate180FlipY == rotateFlipType) {
                rotation = 180;
            } else if (RotateFlipType.Rotate270FlipNone == rotateFlipType
                || RotateFlipType.Rotate270FlipX == rotateFlipType
                || RotateFlipType.Rotate270FlipXY == rotateFlipType
                || RotateFlipType.Rotate270FlipY == rotateFlipType) {
                rotation = 270;
            } else {
                throw new InvalidEnumArgumentException(nameof(rotateFlipType), (int)rotateFlipType, typeof(RotateFlipType));
            }
            return rotation;
        }

        /// <summary>
        /// Gets the rotate flip type for given paramters.
        /// </summary>
        /// <param name="rotation">rotation in degrees</param>
        /// <param name="isFlippedX">indicates whether image is
        /// flipped X</param>
        /// <param name="isFlippedY">indicates whether image is
        /// flipped Y</param>
        /// <returns>rotate flip type for given parameters</returns>
        private static RotateFlipType GetRotateFlipType(int rotation, bool isFlippedX, bool isFlippedY) {
            RotateFlipType rotateFlipType;
            if (0 == rotation && !isFlippedX && !isFlippedY) {
                rotateFlipType = RotateFlipType.RotateNoneFlipNone;
            } else if (0 == rotation && isFlippedX && !isFlippedY) {
                rotateFlipType = RotateFlipType.RotateNoneFlipX;
            } else if (0 == rotation && isFlippedX && isFlippedY) {
                rotateFlipType = RotateFlipType.RotateNoneFlipXY;
            } else if (0 == rotation && !isFlippedX && isFlippedY) {
                rotateFlipType = RotateFlipType.RotateNoneFlipY;
            } else if (90 == rotation && !isFlippedX && !isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate90FlipNone;
            } else if (90 == rotation && isFlippedX && !isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate90FlipX;
            } else if (90 == rotation && isFlippedX && isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate90FlipXY;
            } else if (90 == rotation && !isFlippedX && isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate90FlipY;
            } else if (180 == rotation && !isFlippedX && !isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate180FlipNone;
            } else if (180 == rotation && isFlippedX && !isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate180FlipX;
            } else if (180 == rotation && isFlippedX && isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate180FlipXY;
            } else if (180 == rotation && !isFlippedX && isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate180FlipY;
            } else if (270 == rotation && !isFlippedX && !isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate270FlipNone;
            } else if (270 == rotation && isFlippedX && !isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate270FlipX;
            } else if (270 == rotation && isFlippedX && isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate270FlipXY;
            } else if (270 == rotation && !isFlippedX && isFlippedY) {
                rotateFlipType = RotateFlipType.Rotate270FlipY;
            } else {
                throw new ArgumentException("Rotate flip type cannot be determined for given arguments.");
            }
            return rotateFlipType;
        }

        /// <summary>
        /// Gets the rotate flip type from Exif data of an image.
        /// </summary>
        /// <param name="image">image to get rotate flip type for</param>
        /// <returns>rotate flip type from Exif data of image</returns>
        private static RotateFlipType GetRotateFlipTypeFromExifDataOf(Image image) {
            var rotateFlipType = RotateFlipType.RotateNoneFlipNone;
            if (ImageFile.IsExifOrientationPropertySetIn(image)) {
                var orientationProperty = image.GetPropertyItem(ImageFile.exifOrientationPropertyId);
                byte orientationValue = orientationProperty.Value[0];
                if (1 == orientationValue) {
                    rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                } else if (2 == orientationValue) {
                    rotateFlipType = RotateFlipType.RotateNoneFlipX;
                } else if (3 == orientationValue) {
                    rotateFlipType = RotateFlipType.Rotate180FlipNone;
                } else if (4 == orientationValue) {
                    rotateFlipType = RotateFlipType.Rotate180FlipX;
                } else if (5 == orientationValue) {
                    rotateFlipType = RotateFlipType.Rotate90FlipX;
                } else if (6 == orientationValue) {
                    rotateFlipType = RotateFlipType.Rotate90FlipNone;
                } else if (7 == orientationValue) {
                    rotateFlipType = RotateFlipType.Rotate270FlipX;
                } else if (8 == orientationValue) {
                    rotateFlipType = RotateFlipType.Rotate270FlipNone;
                }
            }
            return rotateFlipType;
        }

        /// <summary>
        /// Indicates whether Exif orientation property is set in an
        /// image.
        /// </summary>
        /// <param name="image">image to search Exif orientation
        /// property in</param>
        /// <returns>true if Exif orientation property is set in
        /// image, false otherwise</returns>
        private static bool IsExifOrientationPropertySetIn(Image image) {
            bool isExifOrientationPropertyIdSet = false;
            foreach (int propertyId in image.PropertyIdList) {
                if (propertyId == ImageFile.exifOrientationPropertyId) {
                    isExifOrientationPropertyIdSet = true;
                    break;
                }
            }
            return isExifOrientationPropertyIdSet;
        }

        /// <summary>
        /// Rotates and flips an image represented as byte array.
        /// </summary>
        /// <param name="imageBytes">image to be rotated and flipped
        /// represented as byte array</param>
        /// <param name="transformValue">transformation to be applied</param>
        /// <returns>processed image represented as byte array</returns>
        private static byte[] RotateFlipImage(byte[] imageBytes, EncoderValue transformValue) {
            byte[] processedImageBytes;
            using (var imageStream = new MemoryStream(imageBytes)) {
                using (var image = Image.FromStream(imageStream, true)) {
                    if (ImageFile.IsExifOrientationPropertySetIn(image)) {
                        image.RemovePropertyItem(ImageFile.exifOrientationPropertyId);
                    }
                    using (var encoderParameters = new EncoderParameters(1)) {
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Transformation, (long)transformValue);
                        using (var jpegStream = new MemoryStream()) {
                            image.Save(jpegStream, ImageFile.JpegEncoder, encoderParameters);
                            processedImageBytes = jpegStream.ToArray();
                        }
                    }
                }
            }
            return processedImageBytes;
        }

        /// <summary>
        /// Gets this image as JPEG encoded image file with applied
        /// rotation.
        /// </summary>
        /// <returns>JPEG encoded image file with applied rotation -
        /// if this is a JPEG encoded image file without lossless
        /// rotation to be applied already this instance will be
        /// returned</returns>
        public ImageFile ToJpegImageFile() {
            return this.ToJpegImageFile(ResizeType.None, this.LongerSideLength);
        }

        /// <summary>
        /// Gets this image as resized JPEG encoded image file with
        /// applied rotation.
        /// </summary>
        /// <param name="resizeType">type of resize operation to
        /// apply</param>
        /// <param name="sideLength">new maximum side length</param>
        /// <returns>resized JPEG encoded image file with applied
        /// rotation - if this is a JPEG encoded image file with
        /// matching dimensions without lossless rotation to be
        /// applied already this instance will be returned</returns>
        public ImageFile ToJpegImageFile(ResizeType resizeType, int sideLength) {
            ImageFile imageFile;
            if (sideLength < 1) {
                throw new ArgumentException("Side length " + sideLength + " is not valid - it must be greater than 0.", nameof(sideLength));
            }
            bool isResizingOrCropingNecessary = (ResizeType.CropToSquare == resizeType && (this.Width != sideLength || this.Height != sideLength))
                || (ResizeType.KeepAspectRatioSetLongerSide == resizeType && this.LongerSideLength > sideLength)
                || (ResizeType.OptimizeAspectRatioSetLongerSide == resizeType && (this.LongerSideLength > sideLength || !this.HasOptimizedApectRatio))
                || (ResizeType.KeepAspectRatioSetWidth == resizeType && this.Width > sideLength)
                || (ResizeType.OptimizeAspectRatioSetWidth == resizeType && (this.Width > sideLength || !this.HasOptimizedApectRatio))
                || (ResizeType.KeepAspectRatioSetHeight == resizeType && this.Height > sideLength)
                || (ResizeType.OptimizeAspectRatioSetHeight == resizeType && (this.Height > sideLength || !this.HasOptimizedApectRatio));
            if (!isResizingOrCropingNecessary) {
                this.TryApplyRotationAndFlipLosslessly();
            }
            bool isRotationOrFlipNecessary = RotateFlipType.RotateNoneFlipNone != this.RotationAgainstSourceImage;
            if (!isResizingOrCropingNecessary && !isRotationOrFlipNecessary) {
                if (MediaTypeNames.Image.Jpeg == this.MimeType) {
                    imageFile = this;
                } else {
                    imageFile = this.ToJpegImageFile(this.Width, this.Height, this.Width, this.Height);
                }
            } else {
                if (sideLength % 16 != 0) {
                    throw new ArgumentException("Remainder of division of side length " + sideLength + " by 16 is not zero.", nameof(sideLength));
                }
                this.CalculateNewDimensions(resizeType, sideLength, out int resizeWidth, out int resizeHeight, out int newWidth, out int newHeight);
                imageFile = this.ToJpegImageFile(resizeWidth, resizeHeight, newWidth, newHeight);
            }
            return imageFile;
        }

        /// <summary>
        /// Gets this image as resized JPEG encoded image file with
        /// applied rotation.
        /// </summary>
        /// <param name="width">new width in pixels prior to rotation</param>
        /// <param name="height">new height in pixels prior to
        /// rotation</param>
        /// <returns>resized JPEG encoded image file with applied
        /// rotation - if this is a JPEG encoded image file with
        /// matching dimensions without lossless rotation to be
        /// applied already this instance will be returned</returns>
        public ImageFile ToJpegImageFile(int width, int height) {
            ImageFile imageFile;
            if (width < 1) {
                throw new ArgumentException("Width " + width + " is not valid - it must be greater than 0.", nameof(width));
            }
            if (height < 1) {
                throw new ArgumentException("Height " + height + " is not valid - it must be greater than 0.", nameof(height));
            }
            bool isResizingOrCropingNecessary = this.Width != width || this.Height != height;
            if (!isResizingOrCropingNecessary) {
                this.TryApplyRotationAndFlipLosslessly();
            }
            bool isRotationOrFlipNecessary = RotateFlipType.RotateNoneFlipNone != this.RotationAgainstSourceImage;
            if (!isResizingOrCropingNecessary && !isRotationOrFlipNecessary && MediaTypeNames.Image.Jpeg == this.MimeType) {
                imageFile = this;
            } else {
                double factor1 = (double)this.Width / width;
                double factor2 = (double)this.Height / height;
                double factor;
                if (factor1 > factor2) {
                    factor = factor2;
                } else {
                    factor = factor1;
                }
                int resizeWidth = (int)(Math.Round(this.Width / factor));
                int resizeHeight = (int)(Math.Round(this.Height / factor));
                imageFile = this.ToJpegImageFile(resizeWidth, resizeHeight, width, height);
            }
            return imageFile;
        }

        /// <summary>
        /// Gets this image as resized JPEG encoded image file with
        /// applied rotation. Out of memory exceptions are handeled
        /// automatically.
        /// </summary>
        /// <param name="resizeWidth">resized width in pixels prior
        /// to crop operation and to rotation</param>
        /// <param name="resizeHeight">resized width in pixels prior
        /// to crop operation and to rotation</param>
        /// <param name="newWidth">new width in pixels prior to
        /// rotation</param>
        /// <param name="newHeight">new height in pixels prior to
        /// rotation</param>
        /// <returns>resized JPEG encoded image file with applied
        /// rotation</returns>
        private ImageFile ToJpegImageFile(int resizeWidth, int resizeHeight, int newWidth, int newHeight) {
            ImageFile imageFile;
            if (ImageFile.toJpegImageFileRetries > 0) {
                lock (ImageFile.toJpegImageFileLock) {
                    imageFile = this.ToJpegImageFileUnsafe(resizeWidth, resizeHeight, newWidth, newHeight);
                }
            } else {
                try {
                    imageFile = this.ToJpegImageFileUnsafe(resizeWidth, resizeHeight, newWidth, newHeight);
                } catch (OutOfMemoryException) {
                    ImageFile.toJpegImageFileRetries++;
                    try {
                        imageFile = this.ToJpegImageFile(resizeWidth, resizeHeight, newWidth, newHeight);
                    } finally {
                        ImageFile.toJpegImageFileRetries--;
                    }
                }
            }
            return imageFile;
        }

        /// <summary>
        /// Gets this image as resized JPEG encoded image file with
        /// applied rotation. Out of memory exceptions are not
        /// handeled.
        /// </summary>
        /// <param name="resizeWidth">resized width in pixels prior
        /// to crop operation and to rotation</param>
        /// <param name="resizeHeight">resized width in pixels prior
        /// to crop operation and to rotation</param>
        /// <param name="newWidth">new width in pixels prior to
        /// rotation</param>
        /// <param name="newHeight">new height in pixels prior to
        /// rotation</param>
        /// <returns>resized JPEG encoded image file with applied
        /// rotation</returns>
        private ImageFile ToJpegImageFileUnsafe(int resizeWidth, int resizeHeight, int newWidth, int newHeight) {
            ImageFile imageFile;
            bool isRotationOrFlipNecessary = RotateFlipType.RotateNoneFlipNone != this.RotationAgainstSourceImage;
            using (var imageStream = new MemoryStream(this.Bytes)) {
                using (var image = Image.FromStream(imageStream, true)) {
                    using (var croppedResizedBitmap = new Bitmap(newWidth, newHeight)) {
                        using (var graphics = Graphics.FromImage(croppedResizedBitmap)) {
                            // resize and crop
                            int cropLeft = (resizeWidth - newWidth) / -2;
                            int cropTop = (resizeHeight - newHeight) / -2;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.DrawImage(image, cropLeft, cropTop, resizeWidth, resizeHeight);
                            // rotate and flip
                            if (isRotationOrFlipNecessary) {
                                croppedResizedBitmap.RotateFlip(this.RotationAgainstSourceImage);
                            }
                            // encode
                            using (var encoderParameters = new EncoderParameters(2)) {
                                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85L);
                                encoderParameters.Param[1] = new EncoderParameter(Encoder.RenderMethod, (int)EncoderValue.RenderProgressive);
                                using (var jpegStream = new MemoryStream()) {
                                    croppedResizedBitmap.Save(jpegStream, ImageFile.JpegEncoder, encoderParameters);
                                    imageFile = new ImageFile {
                                        Bytes = jpegStream.ToArray(),
                                        Name = this.Name
                                    };
                                }
                            }
                        }
                    }
                }
            }
            return imageFile;
        }

        /// <summary>
        /// Tries to apply the rotation and flip offset of this image
        /// losslessly. JPEG images are rotated losslessly if width %
        /// 16 = 0 and height % 16 = 0.
        /// </summary>
        /// <returns>true if rotation and flip could be applied
        /// losslessly, false otherwise</returns>
        public bool TryApplyRotationAndFlipLosslessly() {
            bool success;
            if (RotateFlipType.RotateNoneFlipNone == this.RotationAgainstSourceImage) {
                success = true;
            } else {
                if (MediaTypeNames.Image.Jpeg == this.MimeType && this.Width % 16 == 0 && this.Height % 16 == 0) {
                    byte[] imageBytes = this.Bytes;
                    int rotation = ImageFile.GetNumericRotationOf(this.RotationAgainstSourceImage);
                    if (rotation > 0) {
                        EncoderValue transformValue;
                        if (90 == rotation) {
                            transformValue = EncoderValue.TransformRotate90;
                        } else if (180 == rotation) {
                            transformValue = EncoderValue.TransformRotate180;
                        } else if (270 == rotation) {
                            transformValue = EncoderValue.TransformRotate270;
                        } else {
                            throw new ArgumentException("Rotation of \"" + rotation.ToString() + "\" degrees is not valid.", nameof(this.RotationAgainstSourceImage));
                        }
                        imageBytes = ImageFile.RotateFlipImage(imageBytes, transformValue);
                    }
                    if (ImageFile.GetIsFlippedXOf(this.RotationAgainstSourceImage)) {
                        imageBytes = ImageFile.RotateFlipImage(imageBytes, EncoderValue.TransformFlipHorizontal);
                    }
                    if (ImageFile.GetIsFlippedYOf(this.RotationAgainstSourceImage)) {
                        imageBytes = ImageFile.RotateFlipImage(imageBytes, EncoderValue.TransformFlipVertical);
                    }
                    this.Bytes = imageBytes;
                    success = true;
                } else {
                    success = false;
                }
            }
            return success;
        }

    }

}