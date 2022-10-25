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

namespace Framework.BusinessApplications {

    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Leightweight synchronous HTTP client to be used for web
    /// requests.
    /// </summary>
    public class HttpClient {

        /// <summary>
        /// Credentials to be used for requests. Set this to
        /// CredentialCache.DefaultCredentials to pass through
        /// Windows credentials.
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// True if default credentials are used, false otherwise.
        /// </summary>
        public bool UseDefaultCredentials {
            get { return null == this.Credentials; }
        }

        /// <summary>
        /// Proxy to be used for requests.
        /// </summary>
        public IWebProxy WebProxy { get; set; }

        /// <summary>
        /// Instantiates a new instance. Set Credentials to
        /// CredentialCache.DefaultCredentials to pass through
        /// Windows credentials.
        /// </summary>
        public HttpClient()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Creates a new HTTP web request.
        /// </summary>
        /// <param name="method">HTTP method like GET or POST</param>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="accept">value of Accept HTTP header</param>
        /// <param name="requestContent">content to be sent with
        /// request - may be null</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <returns>creaed HTTP request</returns>
        private HttpWebRequest CreateHttpWebRequest(string method, string requestUrl, string accept, byte[] requestContent, string requestContentType) {
            var httpWebRequest = WebRequest.Create(requestUrl) as HttpWebRequest;
            if (!this.UseDefaultCredentials) {
                httpWebRequest.Credentials = this.Credentials;
            }
            httpWebRequest.Method = method;
            if (!string.IsNullOrEmpty(accept)) {
                httpWebRequest.Accept = accept;
            }
            if (null != this.WebProxy) {
                httpWebRequest.Proxy = this.WebProxy;
            }
            if (null != requestContent) {
                var requestContentLength = requestContent.Length;
                if (requestContentLength > 0) {
                    if (null != requestContentType) {
                        httpWebRequest.ContentType = requestContentType;
                    }
                    using (var requestStream = httpWebRequest.GetRequestStream()) {
                        requestStream.Write(requestContent, 0, requestContentLength);
                    }
                }
            }
            return httpWebRequest;
        }

        /// <summary>
        /// Sends an HTTP DELETE request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <returns>received response as string</returns>
        public string Delete(string requestUrl) {
            return this.Delete(requestUrl, out _);
        }

        /// <summary>
        /// Sends an HTTP DELETE request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Delete(string requestUrl, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveString("DELETE", requestUrl, null, string.Empty, null, out _, out _, out responseStatusCode);
        }

        /// <summary>
        /// Gets the HTTP web response to an HTTP web request.
        /// </summary>
        /// <param name="httpWebRequest">HTTP web request</param>
        /// <returns>HTTP web response</returns>
        private static HttpWebResponse GetHttpResponseFor(HttpWebRequest httpWebRequest) {
            HttpWebResponse httpWebResponse;
            try {
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            } catch (WebException exception) {
                if (WebExceptionStatus.ProtocolError == exception.Status) {
                    httpWebResponse = (HttpWebResponse)exception.Response;
                } else {
                    throw;
                }
            }
            return httpWebResponse;
        }

        /// <summary>
        /// Sends an HTTP GET request and receives the reply as
        /// byte[].
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <returns>received response as byte[]</returns>
        public byte[] GetByteArray(string requestUrl) {
            return this.GetByteArray(requestUrl, out _);
        }

        /// <summary>
        /// Sends an HTTP GET request and receives the reply as
        /// byte[].
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as byte[]</returns>
        public byte[] GetByteArray(string requestUrl, out HttpStatusCode responseStatusCode) {
            return this.GetByteArray(requestUrl, null, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP GET request and receives the reply as
        /// byte[].
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="accept">value of Accept HTTP header</param>
        /// <returns>received response as byte[]</returns>
        public byte[] GetByteArray(string requestUrl, string accept) {
            return this.GetByteArray(requestUrl, accept, out _);
        }

        /// <summary>
        /// Sends an HTTP GET request and receives the reply as
        /// byte[].
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="accept">value of Accept HTTP header</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as byte[]</returns>
        public byte[] GetByteArray(string requestUrl, string accept, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveByteArray("GET", requestUrl, accept, string.Empty, null, out _, out _, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP GET request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <returns>received response as string</returns>
        public string GetString(string requestUrl) {
            return this.GetString(requestUrl, out _);
        }

        /// <summary>
        /// Sends an HTTP GET request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string GetString(string requestUrl, out HttpStatusCode responseStatusCode) {
            return this.GetString(requestUrl, null, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP GET request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <returns>received response as string</returns>
        /// <param name="accept">value of Accept HTTP header</param>
        public string GetString(string requestUrl, string accept) {
            return this.GetString(requestUrl, accept, out _);
        }

        /// <summary>
        /// Sends an HTTP GET request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="accept">value of Accept HTTP header</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string GetString(string requestUrl, string accept, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveString("GET", requestUrl, accept, string.Empty, null, out _, out _, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP PATCH request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be patched</param>       
        /// <returns>received response as string</returns>
        public string Patch(string requestUrl, byte[] requestContent) {
            return this.Patch(requestUrl, requestContent, out _);
        }

        /// <summary>
        /// Sends an HTTP PATCH request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be patched</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Patch(string requestUrl, byte[] requestContent, out HttpStatusCode responseStatusCode) {
            return this.Patch(requestUrl, requestContent, null, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP PATCH request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be patched</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <returns>received response as string</returns>
        public string Patch(string requestUrl, byte[] requestContent, string requestContentType) {
            return this.Patch(requestUrl, requestContent, requestContentType, out _);
        }

        /// <summary>
        /// Sends an HTTP PATCH request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be patched</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Patch(string requestUrl, byte[] requestContent, string requestContentType, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveString("PATCH", requestUrl, null, requestContent, requestContentType, out _, out _, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP PATCH request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be patched</param>
        /// <returns>received response as string</returns>
        public string Patch(string requestUrl, string requestContent) {
            return this.Patch(requestUrl, requestContent, out _);
        }

        /// <summary>
        /// Sends an HTTP PATCH request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be patched</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Patch(string requestUrl, string requestContent, out HttpStatusCode responseStatusCode) {
            return this.Patch(requestUrl, requestContent, null, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP PATCH request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be patched</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <returns>received response as string</returns>
        public string Patch(string requestUrl, string requestContent, string requestContentType) {
            return this.Patch(requestUrl, requestContent, requestContentType, out _);
        }

        /// <summary>
        /// Sends an HTTP PATCH request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be patched</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Patch(string requestUrl, string requestContent, string requestContentType, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveString("PATCH", requestUrl, null, requestContent, requestContentType, out _, out _, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP POST request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be posted</param>
        /// <returns>received response as string</returns>
        public string Post(string requestUrl, byte[] requestContent) {
            return this.Post(requestUrl, requestContent, out _);
        }

        /// <summary>
        /// Sends an HTTP POST request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be posted</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Post(string requestUrl, byte[] requestContent, out HttpStatusCode responseStatusCode) {
            return this.Post(requestUrl, requestContent, null, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP POST request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be posted</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <returns>received response as string</returns>
        public string Post(string requestUrl, byte[] requestContent, string requestContentType) {
            return this.Post(requestUrl, requestContent, requestContentType, out _);
        }

        /// <summary>
        /// Sends an HTTP POST request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be posted</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Post(string requestUrl, byte[] requestContent, string requestContentType, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveString("POST", requestUrl, null, requestContent, requestContentType, out _, out _, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP POST request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be posted</param>
        /// <returns>received response as string</returns>
        public string Post(string requestUrl, string requestContent) {
            return this.Post(requestUrl, requestContent, out _);
        }

        /// <summary>
        /// Sends an HTTP POST request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be posted</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Post(string requestUrl, string requestContent, out HttpStatusCode responseStatusCode) {
            return this.Post(requestUrl, requestContent, null, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP POST request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be posted</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <returns>received response as string</returns>
        public string Post(string requestUrl, string requestContent, string requestContentType) {
            return this.Post(requestUrl, requestContent, requestContentType, out _);
        }

        /// <summary>
        /// Sends an HTTP POST request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be posted</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Post(string requestUrl, string requestContent, string requestContentType, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveString("POST", requestUrl, null, requestContent, requestContentType, out _, out _, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP PUT request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be put</param>
        /// <returns>received response as string</returns>
        public string Put(string requestUrl, byte[] requestContent) {
            return this.Put(requestUrl, requestContent, out _);
        }

        /// <summary>
        /// Sends an HTTP PUT request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be put</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Put(string requestUrl, byte[] requestContent, out HttpStatusCode responseStatusCode) {
            return this.Put(requestUrl, requestContent, null, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP PUT request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be put</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>       
        /// <returns>received response as string</returns>
        public string Put(string requestUrl, byte[] requestContent, string requestContentType) {
            return this.Put(requestUrl, requestContent, requestContentType, out _);
        }

        /// <summary>
        /// Sends an HTTP PUT request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be put</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Put(string requestUrl, byte[] requestContent, string requestContentType, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveString("PUT", requestUrl, null, requestContent, requestContentType, out _, out _, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP PUT request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be put</param>
        /// <returns>received response as string</returns>
        public string Put(string requestUrl, string requestContent) {
            return this.Put(requestUrl, requestContent, out _);
        }

        /// <summary>
        /// Sends an HTTP PUT request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be put</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Put(string requestUrl, string requestContent, out HttpStatusCode responseStatusCode) {
            return this.Put(requestUrl, requestContent, null, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP PUT request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be put</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <returns>received response as string</returns>
        public string Put(string requestUrl, string requestContent, string requestContentType) {
            return this.Put(requestUrl, requestContent, requestContentType, out _);
        }

        /// <summary>
        /// Sends an HTTP PUT request and receives the reply as
        /// string.
        /// </summary>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="requestContent">content to be put</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string Put(string requestUrl, string requestContent, string requestContentType, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveString("PUT", requestUrl, null, requestContent, requestContentType, out _, out _, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP request and receives the reply as byte[].
        /// </summary>
        /// <param name="method">HTTP method like GET or POST</param>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="accept">value of Accept HTTP header</param>
        /// <param name="requestContent">content to be sent with
        /// request - may be null</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <param name="responseContentEncoding">endoding of
        /// response</param>
        /// <param name="responseContentType">MIME content type of
        /// response</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as byte[]</returns>
        public byte[] SendReceiveByteArray(string method, string requestUrl, string accept, byte[] requestContent, string requestContentType, out Encoding responseContentEncoding, out string responseContentType, out HttpStatusCode responseStatusCode) {
            var responseBytes = new List<byte>();
            var httpWebRequest = this.CreateHttpWebRequest(method, requestUrl, accept, requestContent, requestContentType);
            using (var httpWebResponse = HttpClient.GetHttpResponseFor(httpWebRequest)) {
                using (var responseStream = httpWebResponse.GetResponseStream()) {
                    if (string.IsNullOrEmpty(httpWebResponse.ContentEncoding)) {
                        responseContentEncoding = null;
                    } else {
                        responseContentEncoding = Encoding.GetEncoding(httpWebResponse.ContentEncoding);
                    }
                    responseContentType = httpWebResponse.ContentType;
                    responseStatusCode = httpWebResponse.StatusCode;
                    int responseByte;
                    while ((responseByte = responseStream.ReadByte()) > -1) {
                        responseBytes.Add((byte)responseByte);
                    }
                }
            }
            return responseBytes.ToArray();
        }

        /// <summary>
        /// Sends an HTTP request and receives the reply as byte[].
        /// </summary>
        /// <param name="method">HTTP method like GET or POST</param>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="accept">value of Accept HTTP header</param>
        /// <param name="requestContent">content to be sent with
        /// request - may be null or empty</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <param name="responseContentEncoding">endoding of
        /// response</param>
        /// <param name="responseContentType">MIME content type of
        /// response</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as byte[]</returns>
        public byte[] SendReceiveByteArray(string method, string requestUrl, string accept, string requestContent, string requestContentType, out Encoding responseContentEncoding, out string responseContentType, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveByteArray(method, requestUrl, accept, string.IsNullOrEmpty(requestContent) ? null : Encoding.UTF8.GetBytes(requestContent), requestContentType, out responseContentEncoding, out responseContentType, out responseStatusCode);
        }

        /// <summary>
        /// Sends an HTTP request and receives the reply as string.
        /// </summary>
        /// <param name="method">HTTP method like GET or POST</param>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="accept">value of Accept HTTP header</param>
        /// <param name="requestContent">content to be sent with
        /// request - may be null</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <param name="responseContentEncoding">endoding of
        /// response</param>
        /// <param name="responseContentType">MIME content type of
        /// response</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string SendReceiveString(string method, string requestUrl, string accept, byte[] requestContent, string requestContentType, out Encoding responseContentEncoding, out string responseContentType, out HttpStatusCode responseStatusCode) {
            string responseString;
            var httpWebRequest = this.CreateHttpWebRequest(method, requestUrl, accept, requestContent, requestContentType);
            using (var httpWebResponse = HttpClient.GetHttpResponseFor(httpWebRequest)) {
                using (var responseStream = httpWebResponse.GetResponseStream()) {
                    if (string.IsNullOrEmpty(httpWebResponse.ContentEncoding)) {
                        responseContentEncoding = Encoding.UTF8;
                    } else {
                        responseContentEncoding = Encoding.GetEncoding(httpWebResponse.ContentEncoding);
                    }
                    responseContentType = httpWebResponse.ContentType;
                    responseStatusCode = httpWebResponse.StatusCode;
                    using (var responseReader = new StreamReader(responseStream, responseContentEncoding)) {
                        responseString = responseReader.ReadToEnd();
                    }
                }
            }
            return responseString;
        }

        /// <summary>
        /// Sends an HTTP request and receives the reply as string.
        /// </summary>
        /// <param name="method">HTTP method like GET or POST</param>
        /// <param name="requestUrl">URL to be requested</param>
        /// <param name="accept">value of Accept HTTP header - may 
        /// be null</param>
        /// <param name="requestContent">content to be sent with
        /// request - may be null or empty</param>
        /// <param name="requestContentType">MIME type of request
        /// content - may be null</param>
        /// <param name="responseContentEncoding">endoding of
        /// response</param>
        /// <param name="responseContentType">MIME content type of
        /// response</param>
        /// <param name="responseStatusCode">HTTP status code of
        /// response</param>
        /// <returns>received response as string</returns>
        public string SendReceiveString(string method, string requestUrl, string accept, string requestContent, string requestContentType, out Encoding responseContentEncoding, out string responseContentType, out HttpStatusCode responseStatusCode) {
            return this.SendReceiveString(method, requestUrl, accept, string.IsNullOrEmpty(requestContent) ? null : Encoding.UTF8.GetBytes(requestContent), requestContentType, out responseContentEncoding, out responseContentType, out responseStatusCode);
        }

    }

}