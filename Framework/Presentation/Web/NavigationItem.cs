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

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Link or headline of navigation section.
    /// </summary>
    public class NavigationItem {

        /// <summary>
        /// True if this is the default navigation item of its menu
        /// level, false otherwise.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// True if no link is set and this is a headline only, false
        /// otherwise.
        /// </summary>
        public bool IsHeadline {
            get {
                return string.IsNullOrEmpty(this.Link);
            }
        }

        /// <summary>
        /// True if item is selected, false otherwise.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// True if item or child navigation items of item are
        /// selected, false otherwise.
        /// </summary>
        public bool IsSelectedCascadedly {
            get {
                bool isSelectedCascadedly = this.IsSelected;
                if (!this.IsSelected) {
                    foreach (var navigationItem in this.NavigationItems) {
                        if (navigationItem.IsSelectedCascadedly) {
                            isSelectedCascadedly = true;
                            break;
                        }
                    }
                }
                return isSelectedCascadedly;
            }
        }

        /// <summary>
        /// Target link of item.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// List of child navigation items.
        /// </summary>
        public List<NavigationItem> NavigationItems { get; private set; }

        /// <summary>
        /// Display caption of item.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of item</param>
        public NavigationItem(string title) {
            this.IsDefault = false;
            this.IsSelected = false;
            this.NavigationItems = new List<NavigationItem>();
            this.Title = title;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of item</param>
        /// <param name="link">target link of item</param>
        public NavigationItem(string title, string link)
            : this(title) {
            this.Link = link;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of item</param>
        /// <param name="link">target link of item</param>
        /// <param name="isSelected">true if item is selected, false
        /// otherwise</param>
        public NavigationItem(string title, string link, bool isSelected)
            : this(title, link) {
            this.IsSelected = isSelected;
        }

        /// <summary>
        /// Sets selection state based on http request.
        /// </summary>
        /// <param name="urlPath">current url path</param>
        /// <returns>true if this item or a child item was selected,
        /// false otherwise</returns>
        internal virtual bool AutoSelect(string urlPath) {
            bool pathIsSelected = false;
            foreach (var navigationItem in this.NavigationItems) {
                pathIsSelected = navigationItem.AutoSelect(urlPath);
                if (pathIsSelected) {
                    break;
                }
            }
            if (!pathIsSelected && !string.IsNullOrEmpty(this.Link) && urlPath.StartsWith(this.Link, StringComparison.Ordinal)) {
                this.IsSelected = true;
                pathIsSelected = true;
            } else {
                this.IsSelected = false;
            }
            return pathIsSelected;
        }

    }

}