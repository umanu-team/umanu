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

    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Lightweight controller for providing OpenSearch descriptions.
    /// </summary>
    public class OpenSearchDescriptionController : HttpCacheController {

        /// <summary>
        /// Absolute URL of OpenSearch description file - it may not
        /// be empty, not contain any special charaters except for
        /// dashes and has to start with a slash.
        /// </summary>
        public string AbsoluteUrl {
            get {
                return this.absoluteUrl;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL \"" + value + "\" of OpenSearch description file does not start with a slash.");
                }
                this.absoluteUrl = value;
            }
        }
        private string absoluteUrl;

        /// <summary>
        /// Display description of the search engine.
        /// </summary>
        public string Description {
            get {
                return this.description;
            }
            private set {
                if (null != value && value.Length > 1024) {
                    throw new ArgumentException("Descriptions may not have more than 1024 characters to be standard-compliant.");
                } else {
                    this.description = value;
                }
            }
        }
        private string description;

        /// <summary>
        /// Template for human-readable HTML result URL to be
        /// processed according to the OpenSearch URL template
        /// syntax: https://github.com/dewitt/opensearch/
        /// </summary>
        public string HtmlTemplateUrl { get; set; }

        /// <summary>
        /// URL of small icon to be displayed in association with
        /// search content.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Template for machine-readable RSS result URL to be
        /// processed according to the OpenSearch URL template
        /// syntax: https://github.com/dewitt/opensearch/
        /// </summary>
        public string RssTemplateUrl { get; set; }

        /// <summary>
        /// Short display title that identifies the search engine.
        /// </summary>
        public string ShortName {
            get {
                return this.shortName;
            }
            private set {
                if (null != value && value.Length > 16) {
                    throw new ArgumentException("Short names may not have more than 16 characters to be standard-compliant.");
                } else {
                    this.shortName = value;
                }
            }
        }
        private string shortName;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="absoluteUrl">absolute URL of OpenSearch
        /// description file - it may not be empty, not contain any
        /// special charaters except for dashes and has to start with
        /// a slash</param>
        /// <param name="shortName">short display title that
        /// identifies the search engine</param>
        /// <param name="description">sisplay description of the
        /// search engine</param>
        public OpenSearchDescriptionController(string absoluteUrl, string shortName, string description)
            : base() {
            this.AbsoluteUrl = absoluteUrl;
            this.Description = description;
            this.ShortName = shortName;
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
            bool isProcessed = false;
            if (httpRequest.Url.AbsolutePath == this.AbsoluteUrl) {
                if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                    this.WriteOpenSearchDescription(httpRequest, httpResponse);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            }
            return isProcessed;
        }

        /// <summary>
        /// Writes the OpenSearch description to http response.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        private void WriteOpenSearchDescription(HttpRequest httpRequest, HttpResponse httpResponse) {
            httpResponse.Clear();
            httpResponse.AppendHeader("Cache-Control", "no-store");
            httpResponse.Charset = "utf-8";
            httpResponse.ContentType = "application/opensearchdescription+xml";
            var xmlResponse = new XmlWriter(httpResponse);
            xmlResponse.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlResponse.AppendOpeningTag("OpenSearchDescription", new KeyValuePair<string, string>("xmlns", "http://a9.com/-/spec/opensearch/1.1/"));
            xmlResponse.AppendOpeningTag("ShortName");
            xmlResponse.AppendHtmlEncoded(this.ShortName);
            xmlResponse.AppendClosingTag("ShortName");
            xmlResponse.AppendOpeningTag("Description");
            xmlResponse.AppendHtmlEncoded(this.Description);
            xmlResponse.AppendClosingTag("Description");
            if (!string.IsNullOrEmpty(this.HtmlTemplateUrl)) {
                var htmlUrlAttributes = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("type", "text/html"),
                    new KeyValuePair<string, string>("template", this.HtmlTemplateUrl),
                    new KeyValuePair<string, string>("rel", "results")
                };
                xmlResponse.AppendSelfClosingTag("Url", htmlUrlAttributes);
            }
            if (!string.IsNullOrEmpty(this.RssTemplateUrl)) {
                var rssUrlAttributes = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("type", "application/rss+xml"),
                    new KeyValuePair<string, string>("template", this.RssTemplateUrl),
                    new KeyValuePair<string, string>("rel", "results")
                };
                xmlResponse.AppendSelfClosingTag("Url", rssUrlAttributes);
            }
            {
                var selfAttributes = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("type", "application/opensearchdescription+xml"),
                    new KeyValuePair<string, string>("template", httpRequest.Url.AbsoluteUri),
                    new KeyValuePair<string, string>("rel", "self")
                };
                xmlResponse.AppendSelfClosingTag("Url", selfAttributes);
            }
            if (!string.IsNullOrEmpty(this.ImageUrl)) {
                xmlResponse.AppendOpeningTag("Image");
                xmlResponse.Append(this.ImageUrl);
                xmlResponse.AppendClosingTag("Image");
            }
            xmlResponse.Append("<InputEncoding>UTF-8</InputEncoding>");
            xmlResponse.AppendClosingTag("OpenSearchDescription");
            return;
        }

    }

}
