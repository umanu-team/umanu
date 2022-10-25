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

namespace Framework.Presentation.Web.Controllers {

    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// Lightweight controller for processing requests of files.
    /// </summary>
    public abstract class FileController : HttpCacheController {

        /// <summary>
        /// Indicated whether output is supposed to be buffered.
        /// </summary>
        public static bool BufferOutput = false;

        /// <summary>
        /// Indicates how local caching of files on client-side
        /// is supposed to be handled.
        /// </summary>
        protected CacheControl CacheControl { get; private set; }

        /// <summary>
        /// List of file locks.
        /// </summary>
        private static readonly FileLockList FileLockList = new FileLockList();

        /// <summary>
        /// Lock object.
        /// </summary>
        private static readonly object fileLockListLock = new object();

        /// <summary>
        /// Indicates whether current user is anonymous.
        /// </summary>
        public bool IsCurrentUserAnonymous { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="cacheControl">indicates how local caching of
        /// files on client-side is supposed to be handled</param>
        /// <param name="isCurrentUserAnonymous">indicates whether
        /// current user is anonymous - is not relevant if
        /// cache-control is set to no-cache or no-store</param>
        public FileController(CacheControl cacheControl, bool isCurrentUserAnonymous)
            : base() {
            this.CacheControl = cacheControl;
            this.IsCurrentUserAnonymous = isCurrentUserAnonymous;
        }

        /// <summary>
        /// Gets the decoded resize type and side length.
        /// </summary>
        /// <param name="encodedString">encoded string</param>
        /// <param name="width">new width of image file</param>
        /// <param name="height">new height of image file</param>
        /// <returns>true on success, false otherwise</returns>
        public static bool DecodeImageParameters(string encodedString, out int width, out int height) {
            bool success = false;
            width = 0;
            height = 0;
            if (encodedString.Length > 2) {
                var xPosition = encodedString.IndexOf('x');
                success = xPosition > 0
                    && (0 == width % 16 || 0 == height % 16)
                    && int.TryParse(encodedString.Substring(0, xPosition), out width)
                    && int.TryParse(encodedString.Substring(xPosition + 1), out height);
            }
            return success;
        }

        /// <summary>
        /// Gets the decoded resize width and height.
        /// </summary>
        /// <param name="encodedString">encoded string</param>
        /// <param name="resizeType">type of resize operation to
        /// apply</param>
        /// <param name="sideLength">new maximum side length</param>
        /// <returns>true on success, false otherwise</returns>
        public static bool DecodeImageParameters(string encodedString, out ResizeType resizeType, out int sideLength) {
            resizeType = ResizeType.None;
            sideLength = 0;
            return encodedString.Length > 1
                && Enum.TryParse<ResizeType>(encodedString.Substring(0, 1), out resizeType)
                && Enum.IsDefined(typeof(ResizeType), resizeType)
                && int.TryParse(encodedString.Substring(1), out sideLength)
                && 0 == sideLength % 16;
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="file">file to be deleted</param>
        private void DeleteFile(HttpRequest httpRequest, HttpResponse httpResponse, File file) {
            httpResponse.Clear();
            if (file.IsWriteProtected || !this.UpdateParentPersistentObjectOfFile(httpRequest.Url)) {
                httpResponse.StatusCode = 403; // Forbidden
            } else {
                httpResponse.StatusCode = 204; // No Content
                file.RemoveCascadedly();
            }
            return;
        }

        /// <summary>
        /// Finds the file for a specific URL.
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <returns>file for URL or null</returns>
        protected abstract File FindFile(Uri url);

        /// <summary>
        /// Finds file properties.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="file">file to find properties for</param>
        private void FindFileProperties(HttpRequest httpRequest, HttpResponse httpResponse, File file) {
            httpResponse.Clear();
            string xmlString;
            using (var streamReader = new System.IO.StreamReader(httpRequest.InputStream)) {
                xmlString = streamReader.ReadToEnd();
            }
            var supportedProperties = FileController.GetSupportedPropertiesOf(file);
            bool isNamesRequestedOnly = false;
            Dictionary<string, string> requestedProperties;
            if (string.IsNullOrWhiteSpace(xmlString)) {
                requestedProperties = supportedProperties;
            } else {
                var xmlRequest = new XmlDocument();
                try {
                    xmlRequest.LoadXml(xmlString);
                } catch (XmlException) {
                    // ignore XMLException
                }
                var namespaceManager = new XmlNamespaceManager(xmlRequest.NameTable);
                namespaceManager.AddNamespace("D", "DAV:");
                namespaceManager.AddNamespace("Office", "urn:schemas-microsoft-com:office:office");
                var propfind = xmlRequest.DocumentElement;
                if (null != propfind && propfind.Name.Equals("d:propfind", StringComparison.OrdinalIgnoreCase)) {
                    if (propfind.FirstChild.Name.Equals("d:allprop", StringComparison.OrdinalIgnoreCase)) {
                        requestedProperties = supportedProperties;
                    } else if (propfind.FirstChild.Name.Equals("d:propname", StringComparison.OrdinalIgnoreCase)) {
                        isNamesRequestedOnly = true;
                        requestedProperties = null;
                    } else {
                        requestedProperties = new Dictionary<string, string>();
                        foreach (var supportedProperty in supportedProperties) {
                            if (null != propfind.SelectSingleNode("/D:propfind/D:prop/" + supportedProperty.Key, namespaceManager)) {
                                requestedProperties.Add(supportedProperty.Key, supportedProperty.Value);
                            }
                        }
                    }
                } else {
                    requestedProperties = null;
                }
            }
            if (isNamesRequestedOnly) {
                httpResponse.ContentType = "text/xml";
                httpResponse.StatusCode = 200; // OK
                WebDavXmlWriter.WritePropertyNames(httpResponse, httpRequest.Url.AbsoluteUri, supportedProperties);
            } else if (null == requestedProperties) {
                httpResponse.StatusCode = 422; // Unprocessable Entry
            } else {
                httpResponse.ContentType = "text/xml";
                httpResponse.StatusCode = 207; // Multi-Status
                var fileLock = FileLockList.TryGetItemFor(file.Id);
                WebDavXmlWriter.WriteFindPropertyInfo(httpResponse, httpRequest.Url.AbsoluteUri, requestedProperties, fileLock);
            }
            return;
        }

        /// <summary>
        /// Gets the file name of a JPEG file and makes sure a JPEG
        /// file extension is at the end.
        /// </summary>
        /// <param name="imageFile">JPEG image file</param>
        /// <returns>file name of JPEG file with JPEG file extension
        /// at the end</returns>
        private static string GetFileNameWithJpegExtensionFor(ImageFile imageFile) {
            string fileName = imageFile.Name;
            if (!fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) && !fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)) {
                fileName += ".jpg"; // necessary because some browsers do not respect MIME types
            }
            return fileName;
        }

        /// <summary>
        /// Resolves the URL of a specific file.
        /// </summary>
        /// <param name="fileBaseDirectory">URL of base directory for
        /// files</param>
        /// <param name="file">name of file to resolve URL for</param>
        /// <returns>URL of file</returns>
        public static string GetUrlOf(string fileBaseDirectory, File file) {
            return FileController.GetUrlOf(fileBaseDirectory, file.Id, file.Name);
        }

        /// <summary>
        /// Resolves the URL of a specific file.
        /// </summary>
        /// <param name="fileBaseDirectory">URL of base directory for
        /// files</param>
        /// <param name="fileId">ID of file to resolve URL for</param>
        /// <param name="fileName">name of file to resolve URL for</param>
        /// <returns>URL of file</returns>
        internal static string GetUrlOf(string fileBaseDirectory, Guid fileId, string fileName) {
            return fileBaseDirectory + fileId.ToString("N") + '/' + Uri.EscapeDataString(fileName);
        }

        /// <summary>
        /// Resolves the URL of a processed image.
        /// </summary>
        /// <param name="fileBaseDirectory">URL of base directory for
        /// files</param>
        /// <param name="imageFile">image file to be processed</param>
        /// <param name="width">display width of image file</param>
        /// <param name="height">display height of image file</param>
        /// <returns>URL of processed image or null</returns>
        public static string GetUrlOf(string fileBaseDirectory, ImageFile imageFile, int width, int height) {
            return FileController.GetUrlOf(fileBaseDirectory, imageFile.Id, imageFile, width, height);
        }

        /// <summary>
        /// Resolves the URL of a processed image.
        /// </summary>
        /// <param name="fileBaseDirectory">URL of base directory for
        /// files</param>
        /// <param name="fileId">ID of file to resolve URL for</param>
        /// <param name="imageFile">image file to be processed</param>
        /// <param name="width">display width of image file</param>
        /// <param name="height">display height of image file</param>
        /// <returns>URL of processed image or null</returns>
        internal static string GetUrlOf(string fileBaseDirectory, Guid fileId, ImageFile imageFile, int width, int height) {
            if (width % 16 != 0 && height % 16 != 0) {
                throw new ArgumentException("At least one of the remainders of division of width or height by 16 must be zero.");
            }
            string url;
            if (null == imageFile) {
                url = null;
            } else {
                string escapedFileName = Uri.EscapeDataString(FileController.GetFileNameWithJpegExtensionFor(imageFile));
                url = fileBaseDirectory + fileId.ToString("N") + '/' + (width).ToString(CultureInfo.InvariantCulture) + 'x' + height.ToString(CultureInfo.InvariantCulture) + '/' + escapedFileName;
            }
            return url;
        }

        /// <summary>
        /// Resolves the URL of a processed image.
        /// </summary>
        /// <param name="fileBaseDirectory">URL of base directory for
        /// files</param>
        /// <param name="imageFile">image file to be processed</param>
        /// <param name="resizeType">type of resize operation to
        /// apply</param>
        /// <param name="sideLength">new maximum side length</param>
        /// <returns>URL of processed image or null</returns>
        public static string GetUrlOf(string fileBaseDirectory, ImageFile imageFile, ResizeType resizeType, int sideLength) {
            return FileController.GetUrlOf(fileBaseDirectory, imageFile.Id, imageFile, resizeType, sideLength);
        }

        /// <summary>
        /// Resolves the URL of a processed image.
        /// </summary>
        /// <param name="fileBaseDirectory">URL of base directory for
        /// files</param>
        /// <param name="fileId">ID of file to resolve URL for</param>
        /// <param name="imageFile">image file to be processed</param>
        /// <param name="resizeType">type of resize operation to
        /// apply</param>
        /// <param name="sideLength">new maximum side length</param>
        /// <returns>URL of processed image or null</returns>
        internal static string GetUrlOf(string fileBaseDirectory, Guid fileId, ImageFile imageFile, ResizeType resizeType, int sideLength) {
            if (sideLength % 16 != 0) {
                throw new ArgumentException("Remainder of division of side length " + sideLength + " by 16 is not zero.", nameof(sideLength));
            }
            string url;
            if (null == imageFile) {
                url = null;
            } else {
                string escapedFileName = Uri.EscapeDataString(FileController.GetFileNameWithJpegExtensionFor(imageFile));
                url = fileBaseDirectory + fileId.ToString("N") + '/' + ((int)resizeType).ToString(CultureInfo.InvariantCulture) + sideLength.ToString(CultureInfo.InvariantCulture) + '/' + escapedFileName;
            }
            return url;
        }

        /// <summary>
        /// Gets the supported properties of a file.
        /// </summary>
        /// <param name="file">file to get supported properties for</param>
        /// <returns>supported properties of file</returns>
        private static Dictionary<string, string> GetSupportedPropertiesOf(File file) {
            string creationDate;
            if (DateTimeKind.Local == file.CreatedAt.Kind) {
                creationDate = file.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
            } else {
                creationDate = file.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture) + 'Z';
            }
            return new Dictionary<string, string>(12) {
                { "D:creationdate", creationDate },
                { "D:displayname", file.Name },
                { "D:getcontentlanguage", "iv" },
                { "D:getcontentlength", file.Size.ToString("0", CultureInfo.InvariantCulture) },
                { "D:getcontenttype", file.MimeType },
                { "D:getetag", null },
                { "D:getlastmodified", file.ModifiedAt.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture) + " GMT" },
                { "D:lockdiscovery", null },
                { "D:resourcetype", null },
                { "D:source", null },
                { "D:supportedlock", null },
                { "Office:modifiedby", file.ModifiedBy?.UserName }
            };
        }

        /// <summary>
        /// Locks a file.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="file">file to be locked</param>
        private void LockFile(HttpRequest httpRequest, HttpResponse httpResponse, File file) {
            httpResponse.Clear();
            if (file.IsWriteProtected) {
                httpResponse.StatusCode = 403; // Forbidden
            } else {
                string xmlString;
                using (var streamReader = new System.IO.StreamReader(httpRequest.InputStream)) {
                    xmlString = streamReader.ReadToEnd();
                }
                if (string.IsNullOrWhiteSpace(xmlString)) {
                    lock (FileController.fileLockListLock) {
                        var fileLock = FileController.FileLockList.TryGetItemFor(file.Id);
                        var ifHeader = httpRequest.Headers.Get("If");
                        if (null != fileLock && null != ifHeader && ifHeader.Contains(fileLock.LockToken)) {
                            httpResponse.ContentType = "text/xml";
                            httpResponse.StatusCode = 200; // OK
                            fileLock.RefreshTimeout();
                            WebDavXmlWriter.WriteFileLockInfo(httpResponse, fileLock);
                        } else {
                            httpResponse.StatusCode = 412; // Precondition Failed
                        }
                    }
                } else {
                    var xmlRequest = new XmlDocument();
                    try {
                        xmlRequest.LoadXml(xmlString);
                    } catch (XmlException) {
                        // ignore XMLException
                    }
                    var namespaceManager = new XmlNamespaceManager(xmlRequest.NameTable);
                    namespaceManager.AddNamespace("D", "DAV:");
                    var lockinfo = xmlRequest.DocumentElement;
                    if (null != lockinfo && lockinfo.Name.Equals("d:lockinfo", StringComparison.OrdinalIgnoreCase)) {
                        var lockscope = lockinfo.SelectSingleNode("/D:lockinfo/D:lockscope", namespaceManager);
                        var locktype = lockinfo.SelectSingleNode("/D:lockinfo/D:locktype", namespaceManager);
                        var owner = lockinfo.SelectSingleNode("/D:lockinfo/D:owner", namespaceManager);
                        if (null != lockscope && 1 == lockscope.ChildNodes.Count && lockscope.FirstChild.Name.Equals("d:exclusive", StringComparison.OrdinalIgnoreCase)
                            && null != locktype && 1 == locktype.ChildNodes.Count && locktype.FirstChild.Name.Equals("d:write", StringComparison.OrdinalIgnoreCase)
                            && null != owner && !string.IsNullOrEmpty(owner.InnerXml)) {
                            lock (FileController.fileLockListLock) {
                                if (FileController.FileLockList.AddFileLock(file.Id, owner.InnerXml)) {
                                    httpResponse.ContentType = "text/xml";
                                    httpResponse.StatusCode = 200; // OK
                                    WebDavXmlWriter.WriteFileLockInfo(httpResponse, FileController.FileLockList[file.Id]);
                                } else {
                                    httpResponse.StatusCode = 423; // Locked
                                }
                            }
                        } else {
                            httpResponse.StatusCode = 412; // Precondition Failed
                        }
                    } else {
                        httpResponse.StatusCode = 422; // Unprocessable Entry
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Sets file properties.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="file">file to set properties for</param>
        private void PatchFileProperties(HttpRequest httpRequest, HttpResponse httpResponse, File file) {
            httpResponse.Clear();
            string xmlString;
            using (var streamReader = new System.IO.StreamReader(httpRequest.InputStream)) {
                xmlString = streamReader.ReadToEnd();
            }
            var supportedProperties = FileController.GetSupportedPropertiesOf(file);
            Dictionary<string, string> requestedProperties;
            if (string.IsNullOrWhiteSpace(xmlString)) {
                httpResponse.StatusCode = 400; // Bad Request
            } else {
                var xmlRequest = new XmlDocument();
                try {
                    xmlRequest.LoadXml(xmlString);
                } catch (XmlException) {
                    // ignore XMLException
                }
                var namespaceManager = new XmlNamespaceManager(xmlRequest.NameTable);
                namespaceManager.AddNamespace("D", "DAV:");
                namespaceManager.AddNamespace("Office", "urn:schemas-microsoft-com:office:office");
                var propertyupdate = xmlRequest.DocumentElement;
                if (null != propertyupdate && propertyupdate.Name.Equals("d:propertyupdate", StringComparison.OrdinalIgnoreCase)) {
                    requestedProperties = new Dictionary<string, string>();
                    foreach (var supportedProperty in supportedProperties) {
                        if (null != propertyupdate.SelectSingleNode("/D:propertyupdate/D:set/D:prop/" + supportedProperty.Key, namespaceManager)
                            || null != propertyupdate.SelectSingleNode("/D:propertyupdate/D:remove/D:prop/" + supportedProperty.Key, namespaceManager)) {
                            requestedProperties.Add(supportedProperty.Key, supportedProperty.Value);
                        }
                    }
                    httpResponse.ContentType = "text/xml";
                    httpResponse.StatusCode = 207; // Multi-Status
                    WebDavXmlWriter.WritePatchPropertyInfo(httpResponse, httpRequest.Url.AbsoluteUri, requestedProperties);
                } else {
                    httpResponse.StatusCode = 400; // Bad Request
                }
            }
            return;
        }

        /// <summary>
        /// Processes image file based on parameters in encoded
        /// string.
        /// </summary>
        /// <param name="file">image file to be processed</param>
        /// <param name="width">new width of image file</param>
        /// <param name="height">new height of image file</param>
        /// <returns>processed image file based on parameters</returns>
        private static ImageFile ProcessImageFile(File file, int width, int height) {
            ImageFile processedImageFile = null;
            var imageFile = file as ImageFile;
            if (null == imageFile) {
                imageFile = new ImageFile(file.Name, file.Bytes);
            }
            if (null != imageFile) {
                var longerSideLength = imageFile.LongerSideLength;
                if (width > longerSideLength && height > longerSideLength) { // images won't be enlarged because this could lead into buffer overflows
                    if (width > height) {
                        height = height * longerSideLength / width;
                        width = longerSideLength;
                    } else {
                        width = width * longerSideLength / height;
                        height = longerSideLength;
                    }
                }
                processedImageFile = imageFile.ToJpegImageFile(width, height);
            }
            return processedImageFile;
        }

        /// <summary>
        /// Processes image file based on parameters in encoded
        /// string.
        /// </summary>
        /// <param name="file">image file to be processed</param>
        /// <param name="resizeType">type of resize operation to
        /// apply</param>
        /// <param name="sideLength">new maximum side length</param>
        /// <returns>processed image file based on parameters</returns>
        private static ImageFile ProcessImageFile(File file, ResizeType resizeType, int sideLength) {
            ImageFile processedImageFile = null;
            var imageFile = file as ImageFile;
            if (null == imageFile) {
                imageFile = new ImageFile(file.Name, file.Bytes);
            }
            if (null != imageFile) {
                if (sideLength > imageFile.LongerSideLength) { // images won't be enlarged because this could lead into buffer overflows
                    sideLength = imageFile.LongerSideLength / 16 * 16;
                    if (sideLength < 1) {
                        sideLength = 16;
                    }
                }
                processedImageFile = imageFile.ToJpegImageFile(resizeType, sideLength);
            }
            return processedImageFile;
        }

        /// <summary>
        /// Processes a file prior to resonding it.
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <param name="file">file to process</param>
        /// <returns>processed file</returns>
        protected virtual File ProcessFile(Uri url, File file) {
            File processedFile = file;
            var urlSegments = url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.None);
            long urlSegmentsLength = urlSegments.LongLength;
            if (urlSegments.LongLength > 1) {
                var encodedString = urlSegments[urlSegmentsLength - 2L];
                if (FileController.DecodeImageParameters(encodedString, out ResizeType resizeType, out int sideLength)) {
                    processedFile = FileController.ProcessImageFile(file, resizeType, sideLength);
                } else if (FileController.DecodeImageParameters(encodedString, out int width, out int height)) {
                    processedFile = FileController.ProcessImageFile(file, width, height);
                }
            }
            return processedFile;
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_PostAuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public override bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed;
            string httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
            var file = this.FindFile(httpRequest.Url);
            if (null == file) {
                isProcessed = false;
            } else {
                if ("GET" == httpMethod) {
                    this.RespondFile(httpRequest, httpResponse, file);
                } else if ("DELETE" == httpMethod) {
                    this.DeleteFile(httpRequest, httpResponse, file);
                } else if ("LOCK" == httpMethod) {
                    this.LockFile(httpRequest, httpResponse, file);
                } else if ("PUT" == httpMethod) {
                    this.PutFile(httpRequest, httpResponse, file);
                } else if ("PROPFIND" == httpMethod) {
                    this.FindFileProperties(httpRequest, httpResponse, file);
                } else if ("PROPPATCH" == httpMethod) {
                    this.PatchFileProperties(httpRequest, httpResponse, file);
                } else if ("UNLOCK" == httpMethod) {
                    this.UnlockFile(httpRequest, httpResponse, file);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            }
            return isProcessed;
        }

        /// <summary>
        /// Updates a file.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="file">file to be updated</param>
        private void PutFile(HttpRequest httpRequest, HttpResponse httpResponse, File file) {
            httpResponse.Clear();
            if (file.IsWriteProtected || !this.UpdateParentPersistentObjectOfFile(httpRequest.Url)) {
                httpResponse.StatusCode = 403; // Forbidden
            } else {
                httpResponse.StatusCode = 204; // No Content
                file.Bytes = new byte[httpRequest.InputStream.Length];
                httpRequest.InputStream.Read(file.Bytes, 0, file.Bytes.Length);
                file.UpdateCascadedly();
            }
            return;
        }

        /// <summary>
        /// Responds to a file request.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="file">requested file</param>
        protected void RespondFile(HttpRequest httpRequest, HttpResponse httpResponse, File file) {
            try {
                httpResponse.Clear();
                httpResponse.BufferOutput = FileController.BufferOutput;
                bool isOfficeFile = file.IsOfficeFile;
                if (isOfficeFile || CacheControl.NoStore == this.CacheControl || file.IsPdfFile) {
                    httpResponse.AppendHeader("Cache-Control", "no-store");
                } else if (CacheControl.NoCache == this.CacheControl) {
                    httpResponse.AppendHeader("Cache-Control", "no-cache");
                } else {
                    bool isFilePublic = this.IsCurrentUserAnonymous || true == file.AllowedGroups?.ForReading.ContainsPermissionsFor(UserDirectory.AnonymousUser);
                    if (CacheControl.NoRevalidation == this.CacheControl) {
                        if (isFilePublic) {
                            httpResponse.AppendHeader("Cache-Control", "public, max-age=31536000"); // 1 year
                        } else {
                            httpResponse.AppendHeader("Cache-Control", "private, max-age=31536000"); // 1 year
                        }
                    } else if (CacheControl.MustRevalidate == this.CacheControl) {
                        if (isFilePublic) {
                            httpResponse.AppendHeader("Cache-Control", "public, must-revalidate, max-age=43200"); // 12 hours
                        } else {
                            httpResponse.AppendHeader("Cache-Control", "private, must-revalidate, max-age=43200"); // 12 hours
                        }
                    } else {
                        throw new InvalidOperationException("Cache-Control type \"" + this.CacheControl.ToString() + "\" is not known.");
                    }
                }
                if (!file.IsWriteProtected && isOfficeFile) {
                    httpResponse.AppendHeader("X-MS-InvokeApp", "1");
                }
                if (UtcDateTime.MinValue == file.ModifiedAt) {
                    httpResponse.ContentType = file.MimeType;
                    httpResponse.BinaryWrite(file.Bytes);
                } else {
                    var modificationDate = new DateTime(file.ModifiedAt.Year, file.ModifiedAt.Month, file.ModifiedAt.Day, file.ModifiedAt.Hour, file.ModifiedAt.Minute, file.ModifiedAt.Second, file.ModifiedAt.Kind);
                    if (modificationDate >= FileController.GetIfUnmodifiedSince(httpRequest)) {
                        httpResponse.StatusCode = 412; // Precondition Failed
                    } else if (modificationDate <= FileController.GetIfModifiedSince(httpRequest)) {
                        httpResponse.StatusCode = 304; // Not Modified
                    } else {
                        file = this.ProcessFile(httpRequest.Url, file);
                        if (null != file) {
                            httpResponse.ContentType = file.MimeType;
                            httpResponse.AppendHeader("Last-Modified", modificationDate.ToString("R", CultureInfo.InvariantCulture));
                            httpResponse.BinaryWrite(file.Bytes);
                        }
                    }
                }
            } catch (HttpException exception) {
                if (exception.ErrorCode != -2147023667 && exception.ErrorCode != -2147024809 && exception.ErrorCode != -2147024832 && exception.ErrorCode != -2147023901) { // if not connection aborted
                    throw;
                }
            }
            return;
        }

        /// <summary>
        /// Unlocks a file.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="file">file to be unlocked</param>
        private void UnlockFile(HttpRequest httpRequest, HttpResponse httpResponse, File file) {
            httpResponse.Clear();
            if (file.IsWriteProtected) {
                httpResponse.StatusCode = 403; // Forbidden
            } else {
                lock (FileController.fileLockListLock) {
                    var fileLock = FileController.FileLockList.TryGetItemFor(file.Id);
                    var lockTokenHeader = httpRequest.Headers.Get("Lock-Token");
                    if (null != fileLock && null != lockTokenHeader && lockTokenHeader.Contains(fileLock.LockToken)) {
                        httpResponse.StatusCode = 204; // No Content
                        FileController.FileLockList.Remove(file.Id);
                    } else {
                        httpResponse.StatusCode = 412; // Precondition Failed
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Updates the parent persistent object of a file for a
        /// specific URL.
        /// </summary>
        /// <param name="url">URL of file to update parent persistent
        /// object for</param>
        /// <returns>true on success, false otherwise</returns>
        protected abstract bool UpdateParentPersistentObjectOfFile(Uri url);

    }

}