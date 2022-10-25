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
    using System.Net.Mime;
    using System.Text;

    /// <summary>
    /// File to be stored in persistence mechanism.
    /// </summary>
    public class File : PersistentObject {

        /// <summary>
        /// Encapsulated blob.
        /// </summary>
        protected Blob Blob {
            get { return this.blob.Value; }
        }
        private readonly PersistentFieldForPersistentObject<Blob> blob =
            new PersistentFieldForPersistentObject<Blob>(nameof(Blob), CascadedRemovalBehavior.RemoveValuesForcibly);

        /// <summary>
        /// File contents as binary data.
        /// </summary>
        public byte[] Bytes {
            get { return this.Blob.Bytes; }
            set { this.Blob.Bytes = value; }
        }

        /// <summary>
        /// File extension of file name without leading dot.
        /// </summary>
        public string Extension {
            get {
                string extension;
                var fileExtensionPosition = this.Name.LastIndexOf('.');
                if (fileExtensionPosition > -1) {
                    extension = this.Name.Substring(fileExtensionPosition + 1);
                } else {
                    extension = string.Empty;
                }
                return extension;
            }
        }

        /// <summary>
        /// Indicates whether a blob is connected to this file. This
        /// is the usual case except for in versioning repositories.
        /// </summary>
        internal bool HasBlob {
            get { return null != this.Blob; }
        }

        /// <summary>
        /// Determines whether file is UTF-8 encoded text file with
        /// BOM.
        /// </summary>
        /// <returns>true if file is UTF-8 encoded text file with
        /// BOM, false otherwise</returns>
        public bool HasUtf8Bom {
            get { return this.Bytes.LongLength > 2 && 0xEF == this.Bytes[0] && 0xBB == this.Bytes[1] && 0xBF == this.Bytes[2]; }
        }

        /// <summary>
        /// Indicates whether encapsulated blob is supposed to be
        /// indexed for full-text.
        /// </summary>
        protected bool IsBlobFullTextIndexed {
            get { return this.blob.IsFullTextIndexed; }
            set { this.blob.IsFullTextIndexed = value; }
        }

        /// <summary>
        /// Indicates whether a file is an Office file.
        /// </summary>
        public bool IsOfficeFile {
            get {
                string fileMimeTypeToUpper = this.MimeType.ToUpperInvariant();
                return "APPLICATION/MSWORD" == fileMimeTypeToUpper
                    || "APPLICATION/VISIO" == fileMimeTypeToUpper
                    || "APPLICATION/X-VISIO" == fileMimeTypeToUpper
                    || "APPLICATION/VND.VISIO" == fileMimeTypeToUpper
                    || "APPLICATION/VISIO.DRAWING" == fileMimeTypeToUpper
                    || "APPLICATION/VSD" == fileMimeTypeToUpper
                    || "APPLICATION/X-VSD" == fileMimeTypeToUpper
                    || "IMAGE/X-VSD" == fileMimeTypeToUpper
                    || "ZZ-APPLICATION/ZZ-WINASSOC-VSD" == fileMimeTypeToUpper
                    || "APPLICATION/X-MSACCESS" == fileMimeTypeToUpper
                    || "APPLICATION/MSPROJ" == fileMimeTypeToUpper
                    || "APPLICATION/X-MSPROJECT" == fileMimeTypeToUpper
                    || "APPLICATION/X-MS-PROJECT" == fileMimeTypeToUpper
                    || "APPLICATION/X-DOS_MS_PROJECT" == fileMimeTypeToUpper
                    || "APPLICATION/MPP" == fileMimeTypeToUpper
                    || "ZZ-APPLICATION/ZZ-WINASSOC-MPP" == fileMimeTypeToUpper
                    || "APPLICATION/X-MSPUBLISHER" == fileMimeTypeToUpper
                    || fileMimeTypeToUpper.StartsWith("APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT")
                    || fileMimeTypeToUpper.StartsWith("APPLICATION/VND.MS-")
                    || fileMimeTypeToUpper.StartsWith("APPLICATION/MSACCESS")
                    || fileMimeTypeToUpper.StartsWith("APPLICATION/MSPROJECT")
                    || fileMimeTypeToUpper.StartsWith("APPLICATION/VND.OASIS.OPENDOCUMENT");
            }
        }

        /// <summary>
        /// Indicates whether a file is a PDF file.
        /// </summary>
        public bool IsPdfFile {
            get { return "APPLICATION/PDF" == this.MimeType.ToUpperInvariant(); }
        }

        /// <summary>
        /// MIME type of file.
        /// </summary>
        public string MimeType {
            get {
                return this.mimeType.Value;
            }
            set {
                if (null != value && "IMAGE/PJPEG" == value.ToUpperInvariant()) {
                    this.mimeType.Value = MediaTypeNames.Image.Jpeg;
                } else {
                    this.mimeType.Value = value;
                }
            }
        }
        private readonly PersistentFieldForString mimeType =
            new PersistentFieldForString(nameof(MimeType));

        /// <summary>
        /// Name of file.
        /// </summary>
        public string Name {
            get { return this.name.Value; }
            set { this.name.Value = value; }
        }
        private readonly PersistentFieldForString name =
            new PersistentFieldForString(nameof(Name));

        /// <summary>
        /// Size of file in bytes.
        /// </summary>
        public long Size {
            get { return this.size.Value; }
            private set { this.size.Value = value; }
        }
        private readonly PersistentFieldForLong size =
            new PersistentFieldForLong(nameof(Size), 0);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public File()
            : base() {
            this.blob.Value = new Blob();
            this.RegisterPersistentField(this.blob);
            this.RegisterPersistentField(this.mimeType);
            this.RegisterPersistentField(this.name);
            this.RegisterPersistentField(this.size);
            this.PropertyChanged += delegate (object sender, PropertyChangedEventArgs eventArguments) {
                if (nameof(File.Blob) == eventArguments.PropertyName || KeyChain.ConcatToKey(nameof(File.Blob), nameof(Persistence.Blob.Bytes)) == eventArguments.PropertyName) {
                    if (null == this.Bytes) {
                        this.Size = 0;
                    } else {
                        this.Size = this.Bytes.LongLength;
                    }
                }
                return;
            };
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <param name="mimeType">MIME type of file</param>
        /// <param name="bytes">file contents as binary data</param>
        public File(string name, string mimeType, byte[] bytes)
            : this() {
            this.Name = name;
            this.MimeType = mimeType;
            this.Bytes = bytes;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="filePath">local system path of file</param>
        /// <param name="fileName">name of file</param>
        public File(string filePath, string fileName)
            : this() {
            this.LoadFromLocalSystemPath(filePath, fileName);
        }

        /// <summary>
        /// Cleans a file name.
        /// </summary>
        /// <param name="fileName">file name to clean</param>
        /// <returns>cleaned file name</returns>
        public static string CleanName(string fileName) {
            string cleanedfileName;
            if (string.IsNullOrEmpty(fileName)) {
                cleanedfileName = "unnamed";
            } else {
                cleanedfileName = fileName;
                int lastSlashIndex = cleanedfileName.LastIndexOfAny(new char[] { '/', '\\' });
                if (lastSlashIndex > -1) {
                    cleanedfileName = cleanedfileName.Substring(lastSlashIndex + 1);
                }
                cleanedfileName = cleanedfileName.Replace('+', '-'); // plus character causes trouble when encoded in URLs
                cleanedfileName = cleanedfileName.Trim();
            }
            return cleanedfileName;
        }

        /// <summary>
        /// Gets the file contents as string using UTF-8 encoding. If
        /// a BOM is set, it will be removed.
        /// </summary>
        /// <returns>file contents as string with UTF-8 encoding</returns>
        public string GetBytesAsString() {
            if (!this.HasUtf8Bom) {
                throw new FormatException("Text file cannot be read because UTF-8 BOM (byte order mark) could not be detected.");
            }
            return this.GetBytesAsString(Encoding.UTF8);
        }

        /// <summary>
        /// Gets the file contents as string using specified
        /// encoding. If a BOM is set, it will be removed.
        /// </summary>
        /// <param name="encoding">encoding of string to get file
        /// contents as</param>
        /// <returns>file contents as string with specified encoding
        /// without BOM</returns>
        public string GetBytesAsString(Encoding encoding) {
            return encoding.GetString(this.Bytes).TrimStart(new char[] { '\xFEFF' });
        }

        /// <summary>
        /// Gets the MIME type for a file name.
        ///
        /// The MIT License (MIT)
        /// Copyright (c) 2014 Samuel Neff
        /// https://github.com/samuelneff/MimeTypeMap
        /// 
        /// Permission is hereby granted, free of charge, to any person obtaining a copy
        /// of this software and associated documentation files (the "Software"), to deal
        /// in the Software without restriction, including without limitation the rights
        /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        /// copies of the Software, and to permit persons to whom the Software is
        /// furnished to do so, subject to the following conditions:
        /// The above copyright notice and this permission notice shall be included in all
        /// copies or substantial portions of the Software.
        /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        /// SOFTWARE.        
        /// </summary>
        /// <param name="fileName">name of file</param>
        /// <returns>matching MIME type for file name</returns>
        internal static string GetMimeTypeFor(string fileName) {
            string mimeType;
            if (fileName.EndsWith(".323", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/h323";
            } else if (fileName.EndsWith(".3g2", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/3gpp2";
            } else if (fileName.EndsWith(".3gp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/3gpp";
            } else if (fileName.EndsWith(".3gp2", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/3gpp2";
            } else if (fileName.EndsWith(".3gpp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/3gpp";
            } else if (fileName.EndsWith(".7z", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-7z-compressed";
            } else if (fileName.EndsWith(".aa", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/audible";
            } else if (fileName.EndsWith(".AAC", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/aac";
            } else if (fileName.EndsWith(".aax", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/vnd.audible.aax";
            } else if (fileName.EndsWith(".ac3", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/ac3";
            } else if (fileName.EndsWith(".accda", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess.addin";
            } else if (fileName.EndsWith(".accdb", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess";
            } else if (fileName.EndsWith(".accdc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess.cab";
            } else if (fileName.EndsWith(".accde", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess";
            } else if (fileName.EndsWith(".accdr", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess.runtime";
            } else if (fileName.EndsWith(".accdt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess";
            } else if (fileName.EndsWith(".accdw", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess.webapplication";
            } else if (fileName.EndsWith(".accft", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess.ftemplate";
            } else if (fileName.EndsWith(".acx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/internet-property-stream";
            } else if (fileName.EndsWith(".AddIn", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".ade", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess";
            } else if (fileName.EndsWith(".adobebridge", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-bridge-url";
            } else if (fileName.EndsWith(".adp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess";
            } else if (fileName.EndsWith(".ADT", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/vnd.dlna.adts";
            } else if (fileName.EndsWith(".ADTS", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/aac";
            } else if (fileName.EndsWith(".ai", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/postscript";
            } else if (fileName.EndsWith(".aif", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-aiff";
            } else if (fileName.EndsWith(".aifc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/aiff";
            } else if (fileName.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/aiff";
            } else if (fileName.EndsWith(".air", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.adobe.air-application-installer-package+zip";
            } else if (fileName.EndsWith(".amc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-mpeg";
            } else if (fileName.EndsWith(".application", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-ms-application";
            } else if (fileName.EndsWith(".art", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-jg";
            } else if (fileName.EndsWith(".asa", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".asax", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".asf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-ms-asf";
            } else if (fileName.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".asm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".asmx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".asr", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-ms-asf";
            } else if (fileName.EndsWith(".asx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-ms-asf";
            } else if (fileName.EndsWith(".atom", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/atom+xml";
            } else if (fileName.EndsWith(".au", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/basic";
            } else if (fileName.EndsWith(".avi", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-msvideo";
            } else if (fileName.EndsWith(".axs", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/olescript";
            } else if (fileName.EndsWith(".bas", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".bcpio", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-bcpio";
            } else if (fileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/bmp";
            } else if (fileName.EndsWith(".c", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".caf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-caf";
            } else if (fileName.EndsWith(".calx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-office.calx";
            } else if (fileName.EndsWith(".cat", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-pki.seccat";
            } else if (fileName.EndsWith(".cc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".cd", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".cdda", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/aiff";
            } else if (fileName.EndsWith(".cdf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-cdf";
            } else if (fileName.EndsWith(".cer", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-x509-ca-cert";
            } else if (fileName.EndsWith(".class", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-java-applet";
            } else if (fileName.EndsWith(".clp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-msclip";
            } else if (fileName.EndsWith(".cmx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-cmx";
            } else if (fileName.EndsWith(".cnf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".cod", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/cis-cod";
            } else if (fileName.EndsWith(".config", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".contact", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-ms-contact";
            } else if (fileName.EndsWith(".coverage", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".cpio", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-cpio";
            } else if (fileName.EndsWith(".cpp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".crd", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-mscardfile";
            } else if (fileName.EndsWith(".crl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/pkix-crl";
            } else if (fileName.EndsWith(".crt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-x509-ca-cert";
            } else if (fileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".csdproj", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".csh", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-csh";
            } else if (fileName.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".css", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/css";
            } else if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/csv";
            } else if (fileName.EndsWith(".cxx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".datasource", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".dbproj", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".dcr", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-director";
            } else if (fileName.EndsWith(".def", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".der", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-x509-ca-cert";
            } else if (fileName.EndsWith(".dgml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".dib", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/bmp";
            } else if (fileName.EndsWith(".dif", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-dv";
            } else if (fileName.EndsWith(".dir", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-director";
            } else if (fileName.EndsWith(".disco", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-msdownload";
            } else if (fileName.EndsWith(".dll.config", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".dlm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/dlm";
            } else if (fileName.EndsWith(".doc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msword";
            } else if (fileName.EndsWith(".docm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-word.document.macroEnabled.12";
            } else if (fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            } else if (fileName.EndsWith(".dot", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msword";
            } else if (fileName.EndsWith(".dotm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-word.template.macroEnabled.12";
            } else if (fileName.EndsWith(".dotx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
            } else if (fileName.EndsWith(".dsw", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".dtd", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".dtsConfig", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".dv", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-dv";
            } else if (fileName.EndsWith(".dvi", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-dvi";
            } else if (fileName.EndsWith(".dwf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "drawing/x-dwf";
            } else if (fileName.EndsWith(".dxr", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-director";
            } else if (fileName.EndsWith(".eml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "message/rfc822";
            } else if (fileName.EndsWith(".eps", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/postscript";
            } else if (fileName.EndsWith(".etl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/etl";
            } else if (fileName.EndsWith(".etx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-setext";
            } else if (fileName.EndsWith(".evy", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/envoy";
            } else if (fileName.EndsWith(".exe.config", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".fdf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.fdf";
            } else if (fileName.EndsWith(".fif", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/fractals";
            } else if (fileName.EndsWith(".filters", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "Application/xml";
            } else if (fileName.EndsWith(".flr", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "x-world/x-vrml";
            } else if (fileName.EndsWith(".flv", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-flv";
            } else if (fileName.EndsWith(".fsscript", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/fsharp-script";
            } else if (fileName.EndsWith(".fsx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/fsharp-script";
            } else if (fileName.EndsWith(".generictest", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/gif";
            } else if (fileName.EndsWith(".group", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-ms-group";
            } else if (fileName.EndsWith(".gsm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-gsm";
            } else if (fileName.EndsWith(".gtar", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-gtar";
            } else if (fileName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-gzip";
            } else if (fileName.EndsWith(".h", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".hdf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-hdf";
            } else if (fileName.EndsWith(".hdml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-hdml";
            } else if (fileName.EndsWith(".hhc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-oleobject";
            } else if (fileName.EndsWith(".hlp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/winhlp";
            } else if (fileName.EndsWith(".hpp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".hqx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/mac-binhex40";
            } else if (fileName.EndsWith(".hta", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/hta";
            } else if (fileName.EndsWith(".htc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-component";
            } else if (fileName.EndsWith(".htm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/html";
            } else if (fileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/html";
            } else if (fileName.EndsWith(".htt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/webviewhtml";
            } else if (fileName.EndsWith(".hxa", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".hxc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".hxe", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".hxf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".hxk", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".hxt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/html";
            } else if (fileName.EndsWith(".hxv", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".hxx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".i", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-icon";
            } else if (fileName.EndsWith(".idl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".ief", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/ief";
            } else if (fileName.EndsWith(".iii", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-iphone";
            } else if (fileName.EndsWith(".inc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".inl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".ins", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-internet-signup";
            } else if (fileName.EndsWith(".ipa", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-itunes-ipa";
            } else if (fileName.EndsWith(".ipg", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-itunes-ipg";
            } else if (fileName.EndsWith(".ipproj", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".ipsw", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-itunes-ipsw";
            } else if (fileName.EndsWith(".iqy", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-ms-iqy";
            } else if (fileName.EndsWith(".isp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-internet-signup";
            } else if (fileName.EndsWith(".ite", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-itunes-ite";
            } else if (fileName.EndsWith(".itlp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-itunes-itlp";
            } else if (fileName.EndsWith(".itms", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-itunes-itms";
            } else if (fileName.EndsWith(".itpc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-itunes-itpc";
            } else if (fileName.EndsWith(".IVF", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-ivf";
            } else if (fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/java-archive";
            } else if (fileName.EndsWith(".jck", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/liquidmotion";
            } else if (fileName.EndsWith(".jcz", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/liquidmotion";
            } else if (fileName.EndsWith(".jfif", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/pjpeg";
            } else if (fileName.EndsWith(".jnlp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-java-jnlp-file";
            } else if (fileName.EndsWith(".jpe", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/jpeg";
            } else if (fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/jpeg";
            } else if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/jpeg";
            } else if (fileName.EndsWith(".js", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/javascript";
            } else if (fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/json";
            } else if (fileName.EndsWith(".jsx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/jscript";
            } else if (fileName.EndsWith(".jsxbin", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".latex", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-latex";
            } else if (fileName.EndsWith(".library-ms", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/windows-library+xml";
            } else if (fileName.EndsWith(".lit", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-ms-reader";
            } else if (fileName.EndsWith(".loadtest", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".lsf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-la-asf";
            } else if (fileName.EndsWith(".lst", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".lsx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-la-asf";
            } else if (fileName.EndsWith(".m13", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-msmediaview";
            } else if (fileName.EndsWith(".m14", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-msmediaview";
            } else if (fileName.EndsWith(".m1v", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".m2t", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/vnd.dlna.mpeg-tts";
            } else if (fileName.EndsWith(".m2ts", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/vnd.dlna.mpeg-tts";
            } else if (fileName.EndsWith(".m2v", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".m3u", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-mpegurl";
            } else if (fileName.EndsWith(".m3u8", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-mpegurl";
            } else if (fileName.EndsWith(".m4a", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/m4a";
            } else if (fileName.EndsWith(".m4b", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/m4b";
            } else if (fileName.EndsWith(".m4p", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/m4p";
            } else if (fileName.EndsWith(".m4r", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-m4r";
            } else if (fileName.EndsWith(".m4v", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-m4v";
            } else if (fileName.EndsWith(".mac", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-macpaint";
            } else if (fileName.EndsWith(".mak", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".man", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-troff-man";
            } else if (fileName.EndsWith(".manifest", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-ms-manifest";
            } else if (fileName.EndsWith(".map", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".master", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".mda", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess";
            } else if (fileName.EndsWith(".mdb", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-msaccess";
            } else if (fileName.EndsWith(".mde", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msaccess";
            } else if (fileName.EndsWith(".me", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-troff-me";
            } else if (fileName.EndsWith(".mfp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-shockwave-flash";
            } else if (fileName.EndsWith(".mht", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "message/rfc822";
            } else if (fileName.EndsWith(".mhtml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "message/rfc822";
            } else if (fileName.EndsWith(".mid", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/mid";
            } else if (fileName.EndsWith(".midi", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/mid";
            } else if (fileName.EndsWith(".mk", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".mmf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-smaf";
            } else if (fileName.EndsWith(".mno", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".mny", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-msmoney";
            } else if (fileName.EndsWith(".mod", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".mov", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/quicktime";
            } else if (fileName.EndsWith(".movie", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-sgi-movie";
            } else if (fileName.EndsWith(".mp2", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".mp2v", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/mpeg";
            } else if (fileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mp4";
            } else if (fileName.EndsWith(".mp4v", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mp4";
            } else if (fileName.EndsWith(".mpa", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".mpe", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".mpeg", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".mpf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-mediapackage";
            } else if (fileName.EndsWith(".mpg", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".mpp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-project";
            } else if (fileName.EndsWith(".mpv2", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".mqv", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/quicktime";
            } else if (fileName.EndsWith(".ms", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-troff-ms";
            } else if (fileName.EndsWith(".mts", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/vnd.dlna.mpeg-tts";
            } else if (fileName.EndsWith(".mtx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".mvb", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-msmediaview";
            } else if (fileName.EndsWith(".mvc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-miva-compiled";
            } else if (fileName.EndsWith(".mxp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-mmxp";
            } else if (fileName.EndsWith(".nc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-netcdf";
            } else if (fileName.EndsWith(".nsc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-ms-asf";
            } else if (fileName.EndsWith(".nws", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "message/rfc822";
            } else if (fileName.EndsWith(".oda", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/oda";
            } else if (fileName.EndsWith(".odc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-ms-odc";
            } else if (fileName.EndsWith(".odh", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".odl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".odp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.oasis.opendocument.presentation";
            } else if (fileName.EndsWith(".ods", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/oleobject";
            } else if (fileName.EndsWith(".odt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.oasis.opendocument.text";
            } else if (fileName.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/ogg";
            } else if (fileName.EndsWith(".one", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/onenote";
            } else if (fileName.EndsWith(".onea", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/onenote";
            } else if (fileName.EndsWith(".onepkg", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/onenote";
            } else if (fileName.EndsWith(".onetmp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/onenote";
            } else if (fileName.EndsWith(".onetoc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/onenote";
            } else if (fileName.EndsWith(".onetoc2", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/onenote";
            } else if (fileName.EndsWith(".orderedtest", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".osdx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/opensearchdescription+xml";
            } else if (fileName.EndsWith(".p10", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/pkcs10";
            } else if (fileName.EndsWith(".p12", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-pkcs12";
            } else if (fileName.EndsWith(".p7b", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-pkcs7-certificates";
            } else if (fileName.EndsWith(".p7c", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/pkcs7-mime";
            } else if (fileName.EndsWith(".p7m", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/pkcs7-mime";
            } else if (fileName.EndsWith(".p7r", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-pkcs7-certreqresp";
            } else if (fileName.EndsWith(".p7s", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/pkcs7-signature";
            } else if (fileName.EndsWith(".pbm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-portable-bitmap";
            } else if (fileName.EndsWith(".pcast", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-podcast";
            } else if (fileName.EndsWith(".pct", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/pict";
            } else if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/pdf";
            } else if (fileName.EndsWith(".pfx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-pkcs12";
            } else if (fileName.EndsWith(".pgm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-portable-graymap";
            } else if (fileName.EndsWith(".pic", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/pict";
            } else if (fileName.EndsWith(".pict", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/pict";
            } else if (fileName.EndsWith(".pkgdef", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".pkgundef", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".pko", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-pki.pko";
            } else if (fileName.EndsWith(".pls", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/scpls";
            } else if (fileName.EndsWith(".pma", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-perfmon";
            } else if (fileName.EndsWith(".pmc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-perfmon";
            } else if (fileName.EndsWith(".pml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-perfmon";
            } else if (fileName.EndsWith(".pmr", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-perfmon";
            } else if (fileName.EndsWith(".pmw", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-perfmon";
            } else if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/png";
            } else if (fileName.EndsWith(".pnm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-portable-anymap";
            } else if (fileName.EndsWith(".pnt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-macpaint";
            } else if (fileName.EndsWith(".pntg", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-macpaint";
            } else if (fileName.EndsWith(".pnz", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/png";
            } else if (fileName.EndsWith(".pot", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-powerpoint";
            } else if (fileName.EndsWith(".potm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-powerpoint.template.macroEnabled.12";
            } else if (fileName.EndsWith(".potx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.openxmlformats-officedocument.presentationml.template";
            } else if (fileName.EndsWith(".ppa", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-powerpoint";
            } else if (fileName.EndsWith(".ppam", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-powerpoint.addin.macroEnabled.12";
            } else if (fileName.EndsWith(".ppm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-portable-pixmap";
            } else if (fileName.EndsWith(".pps", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-powerpoint";
            } else if (fileName.EndsWith(".ppsm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
            } else if (fileName.EndsWith(".ppsx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
            } else if (fileName.EndsWith(".ppt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-powerpoint";
            } else if (fileName.EndsWith(".pptm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
            } else if (fileName.EndsWith(".pptx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            } else if (fileName.EndsWith(".prf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/pics-rules";
            } else if (fileName.EndsWith(".ps", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/postscript";
            } else if (fileName.EndsWith(".psc1", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/PowerShell";
            } else if (fileName.EndsWith(".psess", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".pub", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-mspublisher";
            } else if (fileName.EndsWith(".pwz", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-powerpoint";
            } else if (fileName.EndsWith(".qht", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-html-insertion";
            } else if (fileName.EndsWith(".qhtm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-html-insertion";
            } else if (fileName.EndsWith(".qt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/quicktime";
            } else if (fileName.EndsWith(".qti", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-quicktime";
            } else if (fileName.EndsWith(".qtif", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-quicktime";
            } else if (fileName.EndsWith(".qtl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-quicktimeplayer";
            } else if (fileName.EndsWith(".ra", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-pn-realaudio";
            } else if (fileName.EndsWith(".ram", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-pn-realaudio";
            } else if (fileName.EndsWith(".ras", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-cmu-raster";
            } else if (fileName.EndsWith(".rat", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/rat-file";
            } else if (fileName.EndsWith(".rc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".rc2", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".rct", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".rdlc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".resx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".rf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/vnd.rn-realflash";
            } else if (fileName.EndsWith(".rgb", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-rgb";
            } else if (fileName.EndsWith(".rgs", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".rm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.rn-realmedia";
            } else if (fileName.EndsWith(".rmi", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/mid";
            } else if (fileName.EndsWith(".rmp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.rn-rn_music_package";
            } else if (fileName.EndsWith(".roff", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-troff";
            } else if (fileName.EndsWith(".rpm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-pn-realaudio-plugin";
            } else if (fileName.EndsWith(".rqy", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-ms-rqy";
            } else if (fileName.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/rtf";
            } else if (fileName.EndsWith(".rtx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/richtext";
            } else if (fileName.EndsWith(".ruleset", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".s", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".safariextz", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-safari-safariextz";
            } else if (fileName.EndsWith(".scd", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-msschedule";
            } else if (fileName.EndsWith(".sct", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/scriptlet";
            } else if (fileName.EndsWith(".sd2", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-sd2";
            } else if (fileName.EndsWith(".sdp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/sdp";
            } else if (fileName.EndsWith(".searchConnector-ms", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/windows-search-connector+xml";
            } else if (fileName.EndsWith(".setpay", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/set-payment-initiation";
            } else if (fileName.EndsWith(".setreg", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/set-registration-initiation";
            } else if (fileName.EndsWith(".settings", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".sgimb", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-sgimb";
            } else if (fileName.EndsWith(".sgml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/sgml";
            } else if (fileName.EndsWith(".sh", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-sh";
            } else if (fileName.EndsWith(".shar", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-shar";
            } else if (fileName.EndsWith(".shtml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/html";
            } else if (fileName.EndsWith(".sit", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-stuffit";
            } else if (fileName.EndsWith(".sitemap", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".skin", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".sldm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-powerpoint.slide.macroEnabled.12";
            } else if (fileName.EndsWith(".sldx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.openxmlformats-officedocument.presentationml.slide";
            } else if (fileName.EndsWith(".slk", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel";
            } else if (fileName.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".slupkg-ms", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-ms-license";
            } else if (fileName.EndsWith(".smd", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-smd";
            } else if (fileName.EndsWith(".smx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-smd";
            } else if (fileName.EndsWith(".smz", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-smd";
            } else if (fileName.EndsWith(".snd", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/basic";
            } else if (fileName.EndsWith(".snippet", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".sol", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".sor", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".spc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-pkcs7-certificates";
            } else if (fileName.EndsWith(".spl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/futuresplash";
            } else if (fileName.EndsWith(".src", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-wais-source";
            } else if (fileName.EndsWith(".srf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".SSISDeploymentManifest", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".ssm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/streamingmedia";
            } else if (fileName.EndsWith(".sst", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-pki.certstore";
            } else if (fileName.EndsWith(".stl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-pki.stl";
            } else if (fileName.EndsWith(".sv4cpio", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-sv4cpio";
            } else if (fileName.EndsWith(".sv4crc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-sv4crc";
            } else if (fileName.EndsWith(".svc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/svg+xml";
            } else if (fileName.EndsWith(".swf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-shockwave-flash";
            } else if (fileName.EndsWith(".t", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-troff";
            } else if (fileName.EndsWith(".tar", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-tar";
            } else if (fileName.EndsWith(".tcl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-tcl";
            } else if (fileName.EndsWith(".testrunconfig", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".testsettings", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".tex", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-tex";
            } else if (fileName.EndsWith(".texi", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-texinfo";
            } else if (fileName.EndsWith(".texinfo", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-texinfo";
            } else if (fileName.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-compressed";
            } else if (fileName.EndsWith(".thmx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-officetheme";
            } else if (fileName.EndsWith(".tif", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/tiff";
            } else if (fileName.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/tiff";
            } else if (fileName.EndsWith(".tlh", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".tli", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".tr", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-troff";
            } else if (fileName.EndsWith(".trm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-msterminal";
            } else if (fileName.EndsWith(".trx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".ts", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/vnd.dlna.mpeg-tts";
            } else if (fileName.EndsWith(".tsv", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/tab-separated-values";
            } else if (fileName.EndsWith(".tts", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/vnd.dlna.mpeg-tts";
            } else if (fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".uls", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/iuls";
            } else if (fileName.EndsWith(".user", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".ustar", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-ustar";
            } else if (fileName.EndsWith(".vb", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".vbdproj", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".vbk", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/mpeg";
            } else if (fileName.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".vbs", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/vbscript";
            } else if (fileName.EndsWith(".vcf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/x-vcard";
            } else if (fileName.EndsWith(".vcproj", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "Application/xml";
            } else if (fileName.EndsWith(".vcs", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".vcxproj", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "Application/xml";
            } else if (fileName.EndsWith(".vddproj", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".vdp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".vdproj", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".vdx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-visio.viewer";
            } else if (fileName.EndsWith(".vml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".vscontent", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".vsct", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".vsd", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.visio";
            } else if (fileName.EndsWith(".vsi", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/ms-vsi";
            } else if (fileName.EndsWith(".vsix", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vsix";
            } else if (fileName.EndsWith(".vsixlangpack", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".vsixmanifest", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".vsmdi", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".vspscc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".vss", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.visio";
            } else if (fileName.EndsWith(".vsscc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".vssettings", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".vssscc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".vst", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.visio";
            } else if (fileName.EndsWith(".vstemplate", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".vsto", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-ms-vsto";
            } else if (fileName.EndsWith(".vsw", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.visio";
            } else if (fileName.EndsWith(".vsx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.visio";
            } else if (fileName.EndsWith(".vtx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.visio";
            } else if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/wav";
            } else if (fileName.EndsWith(".wave", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/wav";
            } else if (fileName.EndsWith(".wax", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-ms-wax";
            } else if (fileName.EndsWith(".wbk", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msword";
            } else if (fileName.EndsWith(".wbmp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/vnd.wap.wbmp";
            } else if (fileName.EndsWith(".wcm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-works";
            } else if (fileName.EndsWith(".wdb", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-works";
            } else if (fileName.EndsWith(".wdp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/vnd.ms-photo";
            } else if (fileName.EndsWith(".webarchive", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-safari-webarchive";
            } else if (fileName.EndsWith(".webmanifest", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/manifest+json";
            } else if (fileName.EndsWith(".webtest", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".wiq", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".wiz", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/msword";
            } else if (fileName.EndsWith(".wks", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-works";
            } else if (fileName.EndsWith(".WLMP", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/wlmoviemaker";
            } else if (fileName.EndsWith(".wlpginstall", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-wlpg-detect";
            } else if (fileName.EndsWith(".wlpginstall3", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-wlpg3-detect";
            } else if (fileName.EndsWith(".wm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-ms-wm";
            } else if (fileName.EndsWith(".wma", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "audio/x-ms-wma";
            } else if (fileName.EndsWith(".wmd", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-ms-wmd";
            } else if (fileName.EndsWith(".wmf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-msmetafile";
            } else if (fileName.EndsWith(".wml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/vnd.wap.wml";
            } else if (fileName.EndsWith(".wmlc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.wap.wmlc";
            } else if (fileName.EndsWith(".wmls", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/vnd.wap.wmlscript";
            } else if (fileName.EndsWith(".wmlsc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.wap.wmlscriptc";
            } else if (fileName.EndsWith(".wmp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-ms-wmp";
            } else if (fileName.EndsWith(".wmv", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-ms-wmv";
            } else if (fileName.EndsWith(".wmx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-ms-wmx";
            } else if (fileName.EndsWith(".wmz", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-ms-wmz";
            } else if (fileName.EndsWith(".woff", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/font-woff";
            } else if (fileName.EndsWith(".wpl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-wpl";
            } else if (fileName.EndsWith(".wps", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-works";
            } else if (fileName.EndsWith(".wri", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-mswrite";
            } else if (fileName.EndsWith(".wrl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "x-world/x-vrml";
            } else if (fileName.EndsWith(".wrz", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "x-world/x-vrml";
            } else if (fileName.EndsWith(".wsc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/scriptlet";
            } else if (fileName.EndsWith(".wsdl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".wvx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "video/x-ms-wvx";
            } else if (fileName.EndsWith(".x", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/directx";
            } else if (fileName.EndsWith(".xaf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "x-world/x-vrml";
            } else if (fileName.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xaml+xml";
            } else if (fileName.EndsWith(".xap", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-silverlight-app";
            } else if (fileName.EndsWith(".xbap", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-ms-xbap";
            } else if (fileName.EndsWith(".xbm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-xbitmap";
            } else if (fileName.EndsWith(".xdr", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".xht", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xhtml+xml";
            } else if (fileName.EndsWith(".xhtml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xhtml+xml";
            } else if (fileName.EndsWith(".xla", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel";
            } else if (fileName.EndsWith(".xlam", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel.addin.macroEnabled.12";
            } else if (fileName.EndsWith(".xlc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel";
            } else if (fileName.EndsWith(".xld", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel";
            } else if (fileName.EndsWith(".xlk", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel";
            } else if (fileName.EndsWith(".xll", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel";
            } else if (fileName.EndsWith(".xlm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel";
            } else if (fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel";
            } else if (fileName.EndsWith(".xlsb", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
            } else if (fileName.EndsWith(".xlsm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel.sheet.macroEnabled.12";
            } else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            } else if (fileName.EndsWith(".xlt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel";
            } else if (fileName.EndsWith(".xltm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel.template.macroEnabled.12";
            } else if (fileName.EndsWith(".xltx", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
            } else if (fileName.EndsWith(".xlw", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-excel";
            } else if (fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".xmta", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".xof", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "x-world/x-vrml";
            } else if (fileName.EndsWith(".XOML", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/plain";
            } else if (fileName.EndsWith(".xpm", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-xpixmap";
            } else if (fileName.EndsWith(".xps", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/vnd.ms-xpsdocument";
            } else if (fileName.EndsWith(".xrm-ms", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".xsc", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".xsf", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".xsl", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "text/xml";
            } else if (fileName.EndsWith(".xss", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/xml";
            } else if (fileName.EndsWith(".xwd", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "image/x-xwindowdump";
            } else if (fileName.EndsWith(".z", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/x-compress";
            } else if (fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/zip";
            } else if (fileName.EndsWith(".aaf", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".aca", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".afm", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".asd", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".asi", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".bin", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".cab", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".chm", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".cur", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".dat", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".deploy", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".dsp", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".dwp", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".emz", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".eot", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".fla", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".hhk", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".hhp", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".hxd", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".hxh", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".hxi", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".hxq", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".hxr", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".hxs", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".hxw", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".ics", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".inf", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".java", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".jpb", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".lpk", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".lzh", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".mdp", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".mix", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".msi", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".mso", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".ocx", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".pcx", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".pcz", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".pfb", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".pfm", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".prm", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".prx", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".psd", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".psm", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".psp", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".qxd", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".sea", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".smi", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".snp", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".thn", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".toc", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".u32", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".xtp", StringComparison.OrdinalIgnoreCase)) {
                mimeType = "application/octet-stream";
            } else {
                throw new ApplicationException("File extension of file \"" + fileName + "\" is unknown. Cannot determine MIME type.");
            }
            return mimeType;
        }

        /// <summary>
        /// Loads a file from a local system path.
        /// </summary>
        /// <param name="filePath">local system path of file</param>
        /// <param name="fileName">name of file</param>
        protected void LoadFromLocalSystemPath(string filePath, string fileName) {
            this.Name = fileName;
            this.MimeType = File.GetMimeTypeFor(fileName);
            using (var fileStream = System.IO.File.Open(filePath + fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read)) {
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                this.Bytes = bytes;
            }
            this.CreatedAt = System.IO.File.GetCreationTimeUtc(filePath + fileName);
            this.ModifiedAt = System.IO.File.GetLastWriteTimeUtc(filePath + fileName);
            return;
        }

        /// <summary>
        /// Saves a file to a local system path.
        /// </summary>
        /// <param name="filePath">local system path of file</param>
        /// <param name="fileName">name of file</param>
        public void SaveToLocalSystemPath(string filePath, string fileName) {
            using (var fileStream = System.IO.File.Create(filePath + fileName)) {
                fileStream.Write(this.Bytes, 0, this.Bytes.Length);
            }
            return;
        }

    }

}