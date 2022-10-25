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

    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Builder class for XML output.
    /// </summary>
    public class XmlWriter {

        /// <summary>
        /// Http response.
        /// </summary>
        private HttpResponse httpResponse;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpResponse">http response</param>
        public XmlWriter(HttpResponse httpResponse) {
            this.httpResponse = httpResponse;
        }

        /// <summary>
        /// Appends a copy of a value to the end.
        /// </summary>
        /// <param name="value">value to append</param>
        public void Append(char value) {
            this.httpResponse.Write(value);
            return;
        }

        /// <summary>
        /// Appends a copy of a value to the end.
        /// </summary>
        /// <param name="value">value to append</param>
        public void Append(string value) {
            this.httpResponse.Write(value);
            return;
        }

        /// <summary>
        /// Appends an attribute at the end, if it is not null or
        /// empty.
        /// </summary>
        /// <param name="key">key of attribute</param>
        /// <param name="value">value of attribute</param>
        public void AppendAttribute(string key, string value) {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value)) {
                this.Append(' ');
                this.Append(key);
                this.Append("=\"");
                this.Append(value);
                this.Append('\"');
            }
            return;
        }

        /// <summary>
        /// Appends a closing tag at the end.
        /// </summary>
        /// <param name="tagName">name of tag</param>
        public void AppendClosingTag(string tagName) {
            this.Append("</");
            this.Append(tagName);
            this.Append('>');
            return;
        }

        /// <summary>
        /// Appends a copy of a value to the end as HTML encoded
        /// string.
        /// </summary>
        /// <param name="value">value to append</param>
        public void AppendHtmlEncoded(string value) {
            this.Append(System.Web.HttpUtility.HtmlEncode(value));
            return;
        }

        /// <summary>
        /// Appends an opening tag at the end.
        /// </summary>
        /// <param name="tagName">name of tag</param>
        public void AppendOpeningTag(string tagName) {
            this.AppendTagWithAttributes(tagName, null, false);
            return;
        }

        /// <summary>
        /// Appends an opening tag at the end.
        /// </summary>
        /// <param name="tagName">name of tag</param>
        /// <param name="attribute">attributes to set</param>
        public void AppendOpeningTag(string tagName, KeyValuePair<string, string> attribute) {
            this.AppendOpeningTag(tagName, new KeyValuePair<string, string>[] { attribute });
            return;
        }

        /// <summary>
        /// Appends an opening tag at the end.
        /// </summary>
        /// <param name="tagName">name of tag</param>
        /// <param name="attributes">enumerable of attributes to set</param>
        public void AppendOpeningTag(string tagName, IEnumerable<KeyValuePair<string, string>> attributes) {
            this.AppendTagWithAttributes(tagName, attributes, false);
            return;
        }

        /// <summary>
        /// Appends a self-closing tag at the end.
        /// </summary>
        /// <param name="tagName">name of self-closing tag</param>
        public void AppendSelfClosingTag(string tagName) {
            this.AppendTagWithAttributes(tagName, null, true);
            return;
        }

        /// <summary>
        /// Appends a self-closing tag at the end.
        /// </summary>
        /// <param name="tagName">name of self-closing tag</param>
        /// <param name="attribute">attribute to set</param>
        public void AppendSelfClosingTag(string tagName, KeyValuePair<string, string> attribute) {
            this.AppendSelfClosingTag(tagName, new KeyValuePair<string, string>[] { attribute });
            return;
        }

        /// <summary>
        /// Appends a self-closing tag at the end.
        /// </summary>
        /// <param name="tagName">name of self-closing tag</param>
        /// <param name="attributes">enumerable of attributes to set</param>
        public void AppendSelfClosingTag(string tagName, IEnumerable<KeyValuePair<string, string>> attributes) {
            this.AppendTagWithAttributes(tagName, attributes, true);
            return;
        }

        /// <summary>
        /// Appends a tag at the end.
        /// </summary>
        /// <param name="tagName">name of tag</param>
        /// <param name="attributes">enumerable of attributes to set
        /// - may be null</param>
        /// <param name="isSelfClosing">true to close opening tag, so
        /// no closing tag is needed</param>
        protected void AppendTagWithAttributes(string tagName, IEnumerable<KeyValuePair<string, string>> attributes, bool isSelfClosing) {
            this.Append('<');
            this.Append(tagName);
            if (null != attributes) {
                foreach (var attribute in attributes) {
                    this.AppendAttribute(attribute.Key, attribute.Value);
                }
            }
            if (isSelfClosing) {
                this.Append(" /");
            }
            this.Append('>');
            return;
        }

        /// <summary>
        /// Sends all buffered output to the client.
        /// </summary>
        public void Flush() {
            try {
                this.httpResponse.Flush();
            } catch (HttpException) {
                // ignore http exceptions like on connections closed by client
            }
            return;
        }

    }

}