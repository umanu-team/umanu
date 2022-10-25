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

namespace Framework.BusinessApplications.Buttons {

    using Framework.BusinessApplications.Web.Controllers;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Button of action bar for link to references page.
    /// </summary>
    public class ReferencesButton : ServerSideButton {

        /// <summary>
        /// Types of presentable objects to act as barrier when
        /// resolving topmost presentable objects. Usually barrier
        /// types are needed to adjust the traversal behaviour on
        /// object tree if child instances can be accessed via
        /// multiple paths.
        /// </summary>
        private IEnumerable<Type> typesOfBarrierPresenableObjects;

        /// <summary>
        /// Types of topmost presentable objects to list as
        /// references.
        /// </summary>
        private IEnumerable<Type> typesOfTopmostPresentableObjects;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        public ReferencesButton(string title)
            : this(title, new Type[0]) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="typesOfTopmostPresentableObjects">types of
        /// topmost presentable objects to list as references</param>
        public ReferencesButton(string title, IEnumerable<Type> typesOfTopmostPresentableObjects)
            : this(title, typesOfTopmostPresentableObjects, new Type[0]) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="typesOfTopmostPresentableObjects">types of
        /// topmost presentable objects to list as references</param>
        /// <param name="typesOfBarrierPresenableObjects">types of
        /// presentable objects to act as barrier when resolving
        /// topmost presentable objects - usually barrier types are
        /// needed to adjust the traversal behaviour on object tree
        /// if child instances can be accessed via multiple paths</param>
        public ReferencesButton(string title, IEnumerable<Type> typesOfTopmostPresentableObjects, IEnumerable<Type> typesOfBarrierPresenableObjects)
            : base(title) {
            this.typesOfBarrierPresenableObjects = typesOfBarrierPresenableObjects;
            this.typesOfTopmostPresentableObjects = typesOfTopmostPresentableObjects;
        }

        /// <summary>
        /// Server-side action to execute on click.
        /// </summary>
        /// <param name="form">current form or null</param>
        /// <param name="promptInput">prompt input of user</param>
        public override void OnClick(IForm form, string promptInput) {
            var urlBuilder = new StringBuilder("references.html");
            bool isFirstTypeOfTopmostPresentableObject = true;
            foreach (var typeOfTopmostPresentableObject in this.typesOfTopmostPresentableObjects) {
                if (isFirstTypeOfTopmostPresentableObject) {
                    isFirstTypeOfTopmostPresentableObject = false;
                    urlBuilder.Append("?topmost=");
                } else {
                    urlBuilder.Append(";");
                }
                urlBuilder.Append(typeOfTopmostPresentableObject.AssemblyQualifiedName);
            }
            bool isFirstTypeOfBarrierPresenableObject = true;
            foreach (var typeOfBarrierPresenableObject in this.typesOfBarrierPresenableObjects) {
                if (isFirstTypeOfBarrierPresenableObject) {
                    isFirstTypeOfBarrierPresenableObject = false;
                    if (isFirstTypeOfTopmostPresentableObject) {
                        urlBuilder.Append('?');
                    } else {
                        urlBuilder.Append('&');
                    }
                    urlBuilder.Append("barrier=");
                } else {
                    urlBuilder.Append(";");
                }
                urlBuilder.Append(typeOfBarrierPresenableObject.AssemblyQualifiedName);
            }
            this.RedirectionTarget = urlBuilder.ToString();
            return;
        }

        /// <summary>
        /// Gets the child controllers for action button.
        /// </summary>
        /// <param name="element">object to get child controllers for</param>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of parent page</param>
        /// <returns>child controllers for form</returns>
        public override IEnumerable<IHttpController> GetChildControllers(IProvidableObject element, IBusinessApplication businessApplication, string absoluteUrl) {
            foreach (var childController in base.GetChildControllers(element, businessApplication, absoluteUrl)) {
                yield return childController;
            }
            if (null != element && !element.IsNew) {
                yield return new ReferencesPageController(element, businessApplication, absoluteUrl);
            }
        }

    }

}