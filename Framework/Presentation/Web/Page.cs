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
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Lightweight web page.
    /// </summary>
    public class Page : IPage {

        /// <summary>
        /// Path to additional JavaScript file.
        /// </summary>
        public string AdditionalJavaScriptUrl { get; set; }

        /// <summary>
        /// Content section of page.
        /// </summary>
        public WebControl ContentSection { get; private set; }

        /// <summary>
        /// Cascading Style Sheet (CSS) classes to apply to body tag.
        /// </summary>
        public IList<string> CssClasses { get; private set; }

        /// <summary>
        /// SEO description of page.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Footer section of page.
        /// </summary>
        public WebControl FooterSection { get; private set; }

        /// <summary>
        /// Header section of page.
        /// </summary>
        public WebControl HeaderSection { get; private set; }

        /// <summary>
        /// Path to JavaScript file.
        /// </summary>
        public string JavaScriptUrl { get; set; }

        /// <summary>
        /// List of links to be placed inside of head attribute of
        /// page.
        /// </summary>
        public IList<HeadLink> Links { get; private set; }

        /// <summary>
        /// Navigation selction of page.
        /// </summary>
        public NavigationSection NavigationSection { get; private set; }

        /// <summary>
        /// Poster section of page.
        /// </summary>
        public WebControl PosterSection { get; private set; }

        /// <summary>
        /// Name of tag to use for surrounding the content of the
        /// HTML body - this may be null or empty.
        /// </summary>
        public string SurroundingTagForBodyContent { get; set; }

        /// <summary>
        /// Title of page.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Page() {
            this.CssClasses = new List<string>();
            this.HeaderSection = new WebControl("header");
            this.Links = new List<HeadLink>();
            this.PosterSection = new WebControl("div");
            this.NavigationSection = new NavigationSection();
            this.ContentSection = new WebControl("main");
            this.ContentSection.Attributes.Add("role", "main");
            this.FooterSection = new WebControl("footer");
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="settings">page settings to apply</param>
        public Page(IPageSettings settings)
            : this() {
            this.SetPageSettings(settings);
        }

        /// <summary>
        /// Creates all child controls. This is called prior to
        /// HandleEvents().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public void CreateChildControls(HttpRequest httpRequest) {
            this.HeaderSection.CreateChildControls(httpRequest);
            this.PosterSection.CreateChildControls(httpRequest);
            this.NavigationSection.CreateChildControls(httpRequest);
            this.ContentSection.CreateChildControls(httpRequest);
            this.FooterSection.CreateChildControls(httpRequest);
            return;
        }

        /// <summary>
        /// Clears header section, navigation section, content
        /// section and footer section of this page.
        /// </summary>
        public void Clear() {
            this.HeaderSection.Clear();
            this.PosterSection.Clear();
            this.NavigationSection.NavigationItems.Clear();
            this.ContentSection.Clear();
            this.FooterSection.Clear();
            return;
        }

        /// <summary>
        /// Handles all server-side postback events. This is called
        /// after CreateChildControls(), but prior to Render().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        /// <param name="httpResponse">HTTP response for client</param>
        public void HandleEvents(HttpRequest httpRequest, HttpResponse httpResponse) {
            this.HeaderSection.HandleEvents(httpRequest, httpResponse);
            this.PosterSection.HandleEvents(httpRequest, httpResponse);
            this.NavigationSection.HandleEvents(httpRequest, httpResponse);
            this.ContentSection.HandleEvents(httpRequest, httpResponse);
            this.FooterSection.HandleEvents(httpRequest, httpResponse);
            return;
        }

        /// <summary>
        /// Renders the HTML markup of this control. This is called
        /// after HandleEvents().
        /// </summary>
        /// <param name="httpResponse">HTTP response for client</param>
        public void Render(HttpResponse httpResponse) {
            httpResponse.Clear();
            httpResponse.AppendHeader("Cache-Control", "no-store");
            httpResponse.Charset = "utf-8";
            httpResponse.ContentType = "text/html";
            var html = new HtmlWriter(httpResponse);
            html.Append("<!doctype html><html");
            if (!string.IsNullOrEmpty(Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName)) {
                html.Append(" lang=\"");
                html.Append(Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
                html.Append("\"");
            }
            html.Append("><head>");
            html.Append("<meta charset=\"utf-8\" />");
            html.Append("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />");
            html.Append("<meta name=\"viewport\" content=\"width=device-width, maximum-scale=1, minimum-scale=1, user-scalable=0\" />");
            if (!string.IsNullOrEmpty(this.Description)) {
                html.Append("<meta name=\"description\" content=\"");
                html.AppendHtmlEncoded(this.Description);
                html.Append("\" />");
            }
            if (!string.IsNullOrEmpty(this.Title)) {
                html.Append("<title>");
                html.AppendHtmlEncoded(this.Title);
                html.Append("</title>");
            }
            foreach (var link in this.Links) {
                html.Append("<link");
                if (!string.IsNullOrEmpty(link.Rel)) {
                    html.Append(" rel=\"");
                    html.Append(link.Rel);
                    html.Append("\"");
                }
                if (!string.IsNullOrEmpty(link.Type)) {
                    html.Append(" type=\"");
                    html.Append(link.Type);
                    html.Append("\"");
                }
                if (!string.IsNullOrEmpty(link.CrossOrigin)) {
                    html.Append(" crossorigin=\"");
                    html.Append(link.CrossOrigin);
                    html.Append("\"");
                }
                if (!string.IsNullOrEmpty(link.Href)) {
                    html.Append(" href=\"");
                    html.Append(link.Href);
                    html.Append("\"");
                }
                if (!string.IsNullOrEmpty(link.Title)) {
                    html.Append(" title=\"");
                    html.Append(link.Title);
                    html.Append("\"");
                }
                html.Append(" />");
            }
            if (!string.IsNullOrEmpty(this.JavaScriptUrl)) {
                html.Append("<script src=\"");
                html.Append(this.JavaScriptUrl);
                html.Append("\" type=\"text/javascript\"></script>");
            }
            if (!string.IsNullOrEmpty(this.AdditionalJavaScriptUrl)) {
                html.Append("<script src=\"");
                html.Append(this.AdditionalJavaScriptUrl);
                html.Append("\" type=\"text/javascript\"></script>");
            }
            html.Append("</head>");
            html.AppendOpeningTag("body", this.CssClasses);
            bool pageHasSurroundingTagForBodyContent = !string.IsNullOrEmpty(this.SurroundingTagForBodyContent);
            if (pageHasSurroundingTagForBodyContent) {
                html.AppendOpeningTag(this.SurroundingTagForBodyContent);
            }
            if (!this.HeaderSection.IsEmpty) {
                this.HeaderSection.Render(html);
            }
            if (!this.NavigationSection.IsEmpty) {
                this.NavigationSection.Render(html);
            }
            if (!this.PosterSection.IsEmpty || this.PosterSection.Attributes.Count > 0) {
                this.PosterSection.Render(html);
            }
            html.Flush();
            if (!this.ContentSection.IsEmpty) {
                this.ContentSection.Render(html);
            }
            if (!this.FooterSection.IsEmpty) {
                this.FooterSection.Render(html);
            }
            if (pageHasSurroundingTagForBodyContent) {
                html.AppendClosingTag(this.SurroundingTagForBodyContent);
            }
            html.Append("</body></html>");
            return;
        }

        /// <summary>
        /// Adds a link to a favicon file.
        /// </summary>
        /// <param name="faviconUrl">path to favicon file</param>
        public void AddFavicon(string faviconUrl) {
            if (!string.IsNullOrEmpty(faviconUrl)) {
                this.Links.Add(new HeadLink("shortcut icon", faviconUrl));
            }
            return;
        }

        /// <summary>
        /// Adds a link to an open search description file.
        /// </summary>
        /// <param name="openSearchDescriptionUrl">path to open
        /// search description file</param>
        /// <param name="title">title of associated search</param>
        public void AddOpenSearchDescription(string openSearchDescriptionUrl, string title) {
            if (!string.IsNullOrEmpty(openSearchDescriptionUrl)) {
                this.Links.Add(new HeadLink("search", openSearchDescriptionUrl) {
                    Title = title,
                    Type = "application/opensearchdescription+xml"
                });
            }
            return;
        }

        /// <summary>
        /// Adds a link to an RSS file.
        /// </summary>
        /// <param name="rssFeedUrl">path to RSS file</param>
        public void AddRssFeed(string rssFeedUrl) {
            if (!string.IsNullOrEmpty(rssFeedUrl)) {
                this.Links.Add(new HeadLink("alternate", rssFeedUrl) {
                    Type = "application/rss+xml"
                });
            }
            return;
        }

        /// <summary>
        /// Adds a link to a style sheet file.
        /// </summary>
        /// <param name="styleSheetUrl">path to style sheet file</param>
        public void AddStyleSheet(string styleSheetUrl) {
            if (!string.IsNullOrEmpty(styleSheetUrl)) {
                this.Links.Add(new HeadLink("stylesheet", styleSheetUrl));
            }
            return;
        }

        /// <summary>
        /// Adds a link to a web app manifest file.
        /// </summary>
        /// <param name="webAppManifestUrl">path to web app manifest
        /// file</param>
        public void AddWebAppManifest(string webAppManifestUrl) {
            if (!string.IsNullOrEmpty(webAppManifestUrl)) {
                this.Links.Add(new HeadLink("manifest", webAppManifestUrl) {
                    CrossOrigin = "use-credentials"
                });
            }
            return;
        }

        /// <summary>
        /// Applies a set of page settings.
        /// </summary>
        /// <param name="settings">page settings to apply</param>
        public void SetPageSettings(IPageSettings settings) {
            this.AddFavicon(settings.FaviconUrl);
            this.AddStyleSheet(settings.StyleSheetUrl);
            this.AddStyleSheet(settings.AdditionalStyleSheetUrl);
            this.AdditionalJavaScriptUrl = settings.AdditionalJavaScriptUrl;
            this.JavaScriptUrl = settings.JavaScriptUrl;
            this.SurroundingTagForBodyContent = settings.SurroundingTagForBodyContent;
        }

    }

}