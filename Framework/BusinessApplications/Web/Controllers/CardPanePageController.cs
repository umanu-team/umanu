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

namespace Framework.BusinessApplications.Web.Controllers {

    using Framework.BusinessApplications.DataControllers;
    using Framework.Presentation;
    using Framework.Presentation.Web;
    using Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// HTTP controller for responding business web pages for card
    /// panes.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class CardPanePageController<T> : MasterDetailPageController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Data controller for card pane and forms.
        /// </summary>
        public MasterDetailDataController<T> MasterDetailDataController { get; private set; }

        /// <summary>
        /// View to apply to card page.
        /// </summary>
        public CardPaneView View { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of card pane page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="masterDetailDataController">data controller
        /// for card pane and forms</param>
        /// <param name="cardPaneView">view to apply to card pane</param>
        public CardPanePageController(IBusinessApplication businessApplication, string absoluteUrl, MasterDetailDataController<T> masterDetailDataController, CardPaneView cardPaneView)
            : base(businessApplication, absoluteUrl, masterDetailDataController) {
            this.MasterDetailDataController = masterDetailDataController;
            this.View = cardPaneView;
        }

        /// <summary>
        /// Gets the card pane controls to be rendered.
        /// </summary>
        /// <param name="businessObjects">business objects to be
        /// displayed</param>
        /// <param name="isSubset">true if business objects are a
        /// subset, false if they are total</param>
        /// <param name="description">description text to be
        /// displayed</param>
        /// <param name="pageTitle">title of list page to be set</param>
        /// <returns>new card pane controls to be rendered</returns>
        protected override ICollection<Control> GetControlsForBusinessObjects(ICollection<T> businessObjects,  bool isSubset, string description, out string pageTitle) {
            pageTitle = this.View.Title;
            var groups = this.GetGroupedBusinessObjects(businessObjects);
            var controlsForBusinessObjects = new List<Control>(groups.Count);
            var onClickUrlDelegate = this.GetOnClickUrlDelegate(businessObjects);
            var webFactory = this.BusinessApplication.GetWebFactory(FieldRenderMode.ListTable);
            foreach (var group in groups) {
                if (!string.IsNullOrEmpty(group.Key)) {
                    var groupHeadline = new Label("h1", group.Key);
                    controlsForBusinessObjects.Add(groupHeadline);
                }
                controlsForBusinessObjects.Add(new CardPane(group.Value, this.View, webFactory) {
                    Description = description,
                    FileBaseDirectory = this.BusinessApplication.FileBaseDirectory,
                    OnClickUrlDelegate = onClickUrlDelegate
                });
            }
            return controlsForBusinessObjects;
        }

        /// <summary>
        /// Gets grouped business objects.
        /// </summary>
        /// <param name="businessObjects">busniess objects to be
        /// grouped</param>
        /// <returns>dictionary of grouped business objects</returns>
        private IDictionary<string, ICollection<T>> GetGroupedBusinessObjects(ICollection<T> businessObjects) {
            var groups = new Dictionary<string, ICollection<T>>();
            if (null == this.View.GroupByKeyChain) {
                groups.Add(null, businessObjects);
            } else {
                foreach (var businessObject in businessObjects) {
                    var groupTitle = (businessObject.FindPresentableField(this.View.GroupByKeyChain) as IPresentableFieldForElement)?.ValueAsString ?? string.Empty;
                    if (groups.TryGetValue(groupTitle, out ICollection<T> groupValues)) {
                        groupValues.Add(businessObject);
                    } else {
                        groups.Add(groupTitle, new List<T>(new T[] { businessObject }));
                    }
                }
            }
            return groups;
        }

        /// <summary>
        /// Gets key chains to be preloaded.
        /// </summary>
        /// <returns>key chains to be preloaded</returns>
        protected override IList<string[]> GetKeyChainsToPreload() {
            var keyChainsToPreload = this.MasterDetailDataController.GetKeyChainsToPreloadForListTableView();
            if (null != this.View.DescriptionKeyChain && !keyChainsToPreload.Contains(this.View.DescriptionKeyChain)) {
                keyChainsToPreload.Add(this.View.DescriptionKeyChain);
            }
            if (null != this.View.GroupByKeyChain && !keyChainsToPreload.Contains(this.View.GroupByKeyChain)) {
                keyChainsToPreload.Add(this.View.GroupByKeyChain);
            }
            if (null != this.View.ImageKeyChain && !keyChainsToPreload.Contains(this.View.ImageKeyChain)) {
                keyChainsToPreload.Add(this.View.ImageKeyChain);
            }
            return keyChainsToPreload;
        }

    }

}