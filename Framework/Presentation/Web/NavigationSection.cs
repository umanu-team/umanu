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

    using Framework.Presentation.Exceptions;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Navigation section control.
    /// </summary>
    public class NavigationSection : Control {

        /// <summary>
        /// Name of CSS class to apply for selected items.
        /// </summary>
        public string CssClassForActiveItems { get; set; }

        /// <summary>
        /// Name of CSS class to apply for selected items.
        /// </summary>
        public string CssClassForSelectedItems { get; set; }

        /// <summary>
        /// True to render first navigation level and selected
        /// navigation paths only, false to render all navigation
        /// items cascadedly.
        /// </summary>
        public bool IsCollapsed { get; set; }

        /// <summary>
        /// True if this navigation sections has no navigation items,
        /// false otherwise.
        /// </summary>
        public bool IsEmpty {
            get {
                return this.NavigationItems.Count < 1 && null == this.TitleControl;
            }
        }

        /// <summary>
        /// Maximum number of navigation levels to display.
        /// </summary>
        public uint MaxNavigationLevels { get; set; }

        /// <summary>
        /// List of child navigation items.
        /// </summary>
        public List<NavigationItem> NavigationItems { get; private set; }

        /// <summary>
        /// Surrounding HTML tags to use for rendering items.
        /// </summary>
        public NavigationSectionTag Tags { get; set; }

        /// <summary>
        /// Title control to render prior to navigation items.
        /// </summary>
        public Control TitleControl { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public NavigationSection()
            : base("nav") {
            this.CssClassForActiveItems = "active";
            this.CssClassForSelectedItems = "selected";
            this.IsCollapsed = false;
            this.MaxNavigationLevels = uint.MaxValue;
            this.NavigationItems = new List<NavigationItem>();
            this.Tags = NavigationSectionTag.Div;
        }

        /// <summary>
        /// Selects items based on http request.
        /// </summary>
        /// <param name="httpRequest">http request for auto resolval
        /// of selected items</param>
        public virtual void AutoSelectItems(HttpRequest httpRequest) {
            string urlPath = string.Empty;
            if (null != httpRequest && null != httpRequest.Url && null != httpRequest.Url.AbsolutePath) {
                urlPath = httpRequest.Url.PathAndQuery;
            }
            foreach (var navigationItem in this.NavigationItems) {
                navigationItem.AutoSelect(urlPath);
            }
            return;
        }

        /// <summary>
        /// Creates all child controls. This is called prior to
        /// HandleEvents().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(HttpRequest httpRequest) {
            if (null != this.TitleControl) {
                this.TitleControl.CreateChildControls(httpRequest);
            }
            return;
        }

        /// <summary>
        /// Handles all server-side postback events. This is called
        /// after CreateChildControls(), but prior to Render().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        /// <param name="httpResponse">HTTP response for client</param>
        public override void HandleEvents(HttpRequest httpRequest, HttpResponse httpResponse) {
            if (null != this.TitleControl) {
                this.TitleControl.HandleEvents(httpRequest, httpResponse);
            }
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            if (null != this.TitleControl) {
                this.TitleControl.Render(html);
            }
            if (this.NavigationItems.Count > 0 && this.MaxNavigationLevels > 0) {
                string itemTag;
                if (NavigationSectionTag.None == this.Tags) {
                    itemTag = null;
                } else if (NavigationSectionTag.UlLi == this.Tags) {
                    itemTag = "li";
                } else if (NavigationSectionTag.Div == this.Tags) {
                    itemTag = "div";
                } else if (NavigationSectionTag.Span == this.Tags) {
                    itemTag = "span";
                } else {
                    throw new PresentationException("Navigation sections tags \"" + this.Tags.ToString() + "\" are unknown.");
                }
                this.RenderNavigationItems(html, this.NavigationItems, itemTag, 1);
            }
            return;
        }

        /// <summary>
        /// Renders an enumerable of navigation items.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="navigationItems">navigation items to render</param>
        /// <param name="itemTag">HTML tag to use for rendering items</param>
        /// <param name="navigationLevel">current navigation level</param>
        private void RenderNavigationItems(HtmlWriter html, IEnumerable<NavigationItem> navigationItems, string itemTag, uint navigationLevel) {
            if (NavigationSectionTag.UlLi == this.Tags) {
                html.Append("<ul>");
            }
            foreach (var navigationItem in navigationItems) {
                bool isItemTagNullOrEmpty = string.IsNullOrEmpty(itemTag);
                if (!isItemTagNullOrEmpty) {
                    if (navigationItem.IsSelected && !string.IsNullOrEmpty(this.CssClassForSelectedItems)) {
                        var cssClasses = new List<string>(2);
                        if (!string.IsNullOrEmpty(this.CssClassForActiveItems) && navigationItem.NavigationItems.Count > 0) {
                            cssClasses.Add(this.CssClassForActiveItems);
                        }
                        cssClasses.Add(this.CssClassForSelectedItems);
                        html.AppendOpeningTag(itemTag, cssClasses);
                    } else if (navigationItem.IsSelectedCascadedly && !string.IsNullOrEmpty(this.CssClassForActiveItems)) {
                        html.AppendOpeningTag(itemTag, this.CssClassForActiveItems);
                    } else {
                        html.AppendOpeningTag(itemTag);
                    }
                }
                if (navigationItem.IsHeadline) {
                    html.Append("<span>");
                    html.AppendHtmlEncoded(navigationItem.Title);
                } else {
                    var attributes = new Dictionary<string, string>(2);
                    var cssClasses = new List<string>(3);
                    attributes.Add("href", navigationItem.Link);
                    if (navigationItem.NavigationItems.Count > 0) {
                        cssClasses.Add("parent");
                    }
                    if (isItemTagNullOrEmpty && navigationItem.IsSelected && !string.IsNullOrEmpty(this.CssClassForSelectedItems)) {
                        cssClasses.Add(this.CssClassForSelectedItems);
                        if (!string.IsNullOrEmpty(this.CssClassForActiveItems) && navigationItem.NavigationItems.Count > 0) {
                            cssClasses.Add(this.CssClassForActiveItems);
                        }
                    } else if (isItemTagNullOrEmpty && navigationItem.IsSelectedCascadedly && !string.IsNullOrEmpty(this.CssClassForActiveItems)) {
                        cssClasses.Add(this.CssClassForActiveItems);
                    }
                    if (cssClasses.Count > 0) {
                        string classes = string.Empty;
                        foreach (var cssClass in cssClasses) {
                            if (classes.Length > 0) {
                                classes += " ";
                            }
                            classes += cssClass;
                        }
                        attributes.Add("class", classes);
                    }
                    html.AppendOpeningTag("a", attributes);
                    html.AppendHtmlEncoded(navigationItem.Title);
                }
                if (navigationItem.IsHeadline) {
                    html.Append("</span>");
                } else {
                    html.Append("</a>");
                }
                if (navigationItem.NavigationItems.Count > 0 && navigationLevel < this.MaxNavigationLevels
                    && (!this.IsCollapsed || 1 != navigationLevel || (1 == navigationLevel && navigationItem.IsSelectedCascadedly))) {
                    this.RenderNavigationItems(html, navigationItem.NavigationItems, itemTag, navigationLevel + 1);
                }
                if (!string.IsNullOrEmpty(itemTag)) {
                    html.AppendClosingTag(itemTag);
                }
            }
            if (NavigationSectionTag.UlLi == this.Tags) {
                html.Append("</ul>");
            }
            return;
        }

    }

}