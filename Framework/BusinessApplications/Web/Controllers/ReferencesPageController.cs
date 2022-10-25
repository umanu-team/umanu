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

    using Framework.BusinessApplications;
    using Framework.BusinessApplications.Buttons;
    using Framework.BusinessApplications.Web;
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// HTTP controller for listing references to a providable
    /// object.
    /// </summary>
    public sealed class ReferencesPageController : ProvidableObjectPageController<IProvidableObject> {

        /// <summary>
        /// Providable object to resolve references for.
        /// </summary>
        private IProvidableObject providableObject;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="providableObject">providable object to
        /// resolve references for</param>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteListPageUrl">absolute URL of parent
        /// list page - it may not be empty, not contain any special
        /// charaters except for dashes and has to start and end with
        /// a slash</param>
        public ReferencesPageController(IProvidableObject providableObject, IBusinessApplication businessApplication, string absoluteListPageUrl)
            : base(businessApplication, absoluteListPageUrl) {
            this.providableObject = providableObject;
        }

        /// <summary>
        /// Fills the page with a references list table.
        /// </summary>
        /// <param name="typesOfTopmostObjects">types of topmost
        /// objects to list as references</param>
        /// <param name="typesOfBarrierObjects">types of presentable
        /// objects to act as barrier when resolving topmost
        /// presentable objects</param>
        /// <returns>true if page exists and could be created, false
        /// otherwise</returns>
        private bool CreateReferencesListTablePage(IEnumerable<Type> typesOfTopmostObjects, IEnumerable<Type> typesOfBarrierObjects) {
            bool success;
            if (null != this.providableObject && !this.providableObject.IsNew) {
                this.Page.Title = this.providableObject.GetTitle();
                var actionBar = new ActionBar();
                var closeButton = new CancelButton(Resources.Close);
                var allUsers = new Group("All users");
                allUsers.Members.Add(UserDirectory.AnonymousUser);
                closeButton.AllowedGroupsForReading.Add(allUsers);
                actionBar.AddButton(closeButton);
                this.AddActionBarToPage(actionBar);
                var references = (this.providableObject as PersistentObject).GetReferencingPersistentObjects(typesOfTopmostObjects, typesOfBarrierObjects);
                var listTableView = new ListTableView();
                listTableView.ViewFields.Add(new ViewFieldForTitle(Resources.Title));
                listTableView.ViewFields.Add(new ViewFieldForDateTime(Resources.CreatedAt, nameof(PersistentObject.CreatedAt), Mandatoriness.Optional, TimeSpan.Zero, DateTimeType.DateAndTime));
                listTableView.ViewFields.Add(new ViewFieldForDateTime(Resources.ModifiedAt, nameof(PersistentObject.ModifiedAt), Mandatoriness.Optional, TimeSpan.Zero, DateTimeType.DateAndTime));
                var listTable = new ListTable(references, listTableView, this.BusinessApplication.FileBaseDirectory, this.BusinessApplication.GetWebFactory(FieldRenderMode.ListTable));
                listTable.OnClickUrlDelegate = delegate (IPresentableObject clickedObject) {
                    return "/l/" + clickedObject.Id.ToString("N") + "/";
                };
                this.Page.ContentSection.AddChildControl(listTable);
                success = true;
            } else {
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Extracts types from a semicolon separated string of
        /// types.
        /// </summary>
        /// <param name="typesString">semicolon separated list of
        /// types</param>
        /// <returns>types from a semicolon separated string of types</returns>
        private static IEnumerable<Type> GetTypesFromString(string typesString) {
            var typeStrings = typesString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var types = new List<Type>(typeStrings.Length);
            foreach (var typeString in typeStrings) {
                var type = Type.GetType(typeString);
                if (null != type) {
                    types.Add(type);
                }
            }
            return types;
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
            var urlSegments = this.GetListPageRelativeUrlSegments(httpRequest);
            if (2L == urlSegments.LongLength) {
                if (this.providableObject.Id.ToString("N") == urlSegments[0] && "references.html" == urlSegments[1]) {
                    IEnumerable<Type> typesOfTopmostObjects;
                    var typesOfTopmostObjectsString = httpRequest.QueryString["topmost"];
                    if (null == typesOfTopmostObjectsString) {
                        typesOfTopmostObjects = new Type[] { typeof(IProvidableObject) };
                    } else {
                        typesOfTopmostObjects = ReferencesPageController.GetTypesFromString(typesOfTopmostObjectsString);
                    }
                    IEnumerable<Type> typesOfBarrierObjects;
                    var typesOfBarrierObjectsString = httpRequest.QueryString["barrier"];
                    if (null == typesOfBarrierObjectsString) {
                        typesOfBarrierObjects = new Type[0];
                    } else {
                        typesOfBarrierObjects = ReferencesPageController.GetTypesFromString(typesOfBarrierObjectsString);
                    }
                    isProcessed = this.CreateReferencesListTablePage(typesOfTopmostObjects, typesOfBarrierObjects);
                    if (isProcessed) {
                        if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                            this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                        } else {
                            OptionsController.RejectRequest(httpResponse);
                        }
                    }
                }
            }
            return isProcessed;
        }

    }

}