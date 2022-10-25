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

    using Model;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// XML writer for WebDav responses.
    /// http://www.webdav.org/specs/rfc2518.html
    /// </summary>
    internal static class WebDavXmlWriter {

        /// <summary>
        /// Writes file lock info to http response as XML.
        /// </summary>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="fileLock">file lock info to respond</param>
        public static void WriteFileLockInfo(HttpResponse httpResponse, FileLock fileLock) {
            var xmlResponse = new Framework.Presentation.Web.XmlWriter(httpResponse);
            xmlResponse.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            var propAttributes = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("xmlns:D", "DAV:")
            };
            xmlResponse.AppendOpeningTag("D:prop", propAttributes);
            WebDavXmlWriter.WriteLockDiscovery(xmlResponse, fileLock);
            xmlResponse.AppendClosingTag("D:prop");
            return;
        }

        /// <summary>
        /// Writes file property info to http response as XML.
        /// </summary>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="href">full URL of file to write properties
        /// for</param>
        /// <param name="properties">properties to respond</param>
        /// <param name="fileLock">file lock info to be used in
        /// response</param>
        public static void WriteFindPropertyInfo(HttpResponse httpResponse, string href, Dictionary<string, string> properties, FileLock fileLock) {
            var xmlResponse = new Framework.Presentation.Web.XmlWriter(httpResponse);
            var responseAttributes = new Dictionary<string, string>(1);
            if (properties.ContainsKey("Office:modifiedby")) {
                responseAttributes.Add("xmlns:Office", "urn:schemas-microsoft-com:office:office");
            }
            WebDavXmlWriter.WriteMultistatusHeader(xmlResponse, href, responseAttributes);
            xmlResponse.AppendOpeningTag("D:propstat");
            xmlResponse.AppendOpeningTag("D:status");
            xmlResponse.Append("HTTP/1.1 200 OK");
            xmlResponse.AppendClosingTag("D:status");
            xmlResponse.AppendOpeningTag("D:prop");
            foreach (var property in properties) {
                if (null == property.Value) {
                    if ("D:lockdiscovery" == property.Key && null != fileLock) {
                        WebDavXmlWriter.WriteLockDiscovery(xmlResponse, fileLock);
                    } else if ("D:supportedlock" == property.Key) {
                        WebDavXmlWriter.WriteSupportedLock(xmlResponse);
                    } else {
                        xmlResponse.AppendSelfClosingTag(property.Key);
                    }
                } else {
                    xmlResponse.AppendOpeningTag(property.Key);
                    xmlResponse.Append(property.Value);
                    xmlResponse.AppendClosingTag(property.Key);
                }
            }
            xmlResponse.AppendClosingTag("D:prop");
            xmlResponse.AppendClosingTag("D:propstat");
            WebDavXmlWriter.WriteMultistatusFooter(xmlResponse);
            return;
        }

        /// <summary>
        /// Writed lock discovery info to XML response.
        /// </summary>
        /// <param name="xmlResponse">XML response to write file lock
        /// info to</param>
        /// <param name="fileLock">file lock info to respond</param>
        private static void WriteLockDiscovery(XmlWriter xmlResponse, FileLock fileLock) {
            xmlResponse.AppendOpeningTag("D:lockdiscovery");
            xmlResponse.AppendOpeningTag("D:activelock");
            xmlResponse.AppendOpeningTag("D:locktype");
            xmlResponse.AppendSelfClosingTag("D:write");
            xmlResponse.AppendClosingTag("D:locktype");
            xmlResponse.AppendOpeningTag("D:lockscope");
            xmlResponse.AppendSelfClosingTag("D:exclusive");
            xmlResponse.AppendClosingTag("D:lockscope");
            xmlResponse.AppendOpeningTag("D:depth");
            xmlResponse.Append('0');
            xmlResponse.AppendClosingTag("D:depth");
            xmlResponse.AppendOpeningTag("D:owner");
            xmlResponse.Append(fileLock.Owner);
            xmlResponse.AppendClosingTag("D:owner");
            xmlResponse.AppendOpeningTag("D:timeout");
            xmlResponse.Append("Second-");
            xmlResponse.Append(fileLock.Timeout.Subtract(UtcDateTime.Now).TotalSeconds.ToString("0", CultureInfo.InvariantCulture));
            xmlResponse.AppendClosingTag("D:timeout");
            xmlResponse.AppendOpeningTag("D:locktoken");
            xmlResponse.AppendOpeningTag("D:href");
            xmlResponse.Append(fileLock.LockToken);
            xmlResponse.AppendClosingTag("D:href");
            xmlResponse.AppendClosingTag("D:locktoken");
            xmlResponse.AppendClosingTag("D:activelock");
            xmlResponse.AppendClosingTag("D:lockdiscovery");
            return;
        }

        /// <summary>
        /// Writes a multistatus footer to http response as XML.
        /// </summary>
        /// <param name="xmlResponse">XML response to write
        /// multistatus footer to</param>
        private static void WriteMultistatusFooter(XmlWriter xmlResponse) {
            xmlResponse.AppendClosingTag("D:response");
            xmlResponse.AppendClosingTag("D:multistatus");
            return;
        }

        /// <summary>
        /// Writes a multistatus header to http response as XML.
        /// </summary>
        /// <param name="xmlResponse">XML response to write
        /// multistatus header to</param>
        /// <param name="href">full URL of file to write multistatus
        /// header for</param>
        /// <param name="responseAttributes">attributes for response
        /// tag</param>
        private static void WriteMultistatusHeader(XmlWriter xmlResponse, string href, Dictionary<string, string> responseAttributes) {
            xmlResponse.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            var multistatusAttributes = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("xmlns:D", "DAV:")
            };
            xmlResponse.AppendOpeningTag("D:multistatus", multistatusAttributes);
            xmlResponse.AppendOpeningTag("D:response", responseAttributes);
            xmlResponse.AppendOpeningTag("D:href");
            xmlResponse.Append(href);
            xmlResponse.AppendClosingTag("D:href");
            return;
        }

        /// <summary>
        /// Writes file patch info to http response as XML.
        /// </summary>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="href">full URL of file to write properties
        /// for</param>
        /// <param name="properties">properties to respond</param>
        public static void WritePatchPropertyInfo(HttpResponse httpResponse, string href, Dictionary<string, string> properties) {
            var xmlResponse = new Framework.Presentation.Web.XmlWriter(httpResponse);
            var responseAttributes = new Dictionary<string, string>(1);
            if (properties.ContainsKey("Office:modifiedby")) {
                responseAttributes.Add("xmlns:Office", "urn:schemas-microsoft-com:office:office");
            }
            WebDavXmlWriter.WriteMultistatusHeader(xmlResponse, href, responseAttributes);
            foreach (var property in properties) {
                xmlResponse.AppendOpeningTag("D:propstat");
                xmlResponse.AppendOpeningTag("D:prop");
                if (null == property.Value) {
                    xmlResponse.AppendSelfClosingTag(property.Key);
                } else {
                    xmlResponse.AppendOpeningTag(property.Key);
                    xmlResponse.Append(property.Value);
                    xmlResponse.AppendClosingTag(property.Key);
                }
                xmlResponse.AppendClosingTag("D:prop");
                xmlResponse.AppendOpeningTag("D:status");
                xmlResponse.Append("HTTP/1.1 409 Conflict");
                xmlResponse.AppendClosingTag("D:status");
                xmlResponse.AppendClosingTag("D:propstat");
            }
            WebDavXmlWriter.WriteMultistatusFooter(xmlResponse);
            return;
        }

        /// <summary>
        /// Writes file property names to http response as XML.
        /// </summary>
        /// <param name="httpResponse">http response for web request</param>
        /// <param name="href">full URL of file to write property
        /// names for</param>
        /// <param name="properties">properties to respond</param>
        public static void WritePropertyNames(HttpResponse httpResponse, string href, Dictionary<string, string> properties) {
            var xmlResponse = new Framework.Presentation.Web.XmlWriter(httpResponse);
            xmlResponse.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            var propAttributes = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("xmlns:D", "DAV:"),
                new KeyValuePair<string, string>("xmlns:Office", "urn:schemas-microsoft-com:office:office")
            };
            xmlResponse.AppendOpeningTag("D:prop", propAttributes);
            foreach (var property in properties) {
                xmlResponse.AppendSelfClosingTag(property.Key);
            }
            xmlResponse.AppendClosingTag("D:prop");
            return;
        }

        /// <summary>
        /// Writed supported lock info to XML response.
        /// </summary>
        /// <param name="xmlResponse">XML response to write supported
        /// lock info to</param>
        private static void WriteSupportedLock(XmlWriter xmlResponse) {
            xmlResponse.AppendOpeningTag("D:supportedlock");
            xmlResponse.AppendOpeningTag("D:lockentry");
            xmlResponse.AppendOpeningTag("D:locktype");
            xmlResponse.AppendSelfClosingTag("D:write");
            xmlResponse.AppendClosingTag("D:locktype");
            xmlResponse.AppendOpeningTag("D:lockscope");
            xmlResponse.AppendSelfClosingTag("D:exclusive");
            xmlResponse.AppendClosingTag("D:lockscope");
            xmlResponse.AppendClosingTag("D:lockentry");
            xmlResponse.AppendClosingTag("D:supportedlock");
            return;
        }

    }

}