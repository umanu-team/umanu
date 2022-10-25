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

namespace Framework.Presentation.Forms {

    using Framework.Persistence.Fields;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Field to be presented in a view.
    /// </summary>
    public class ViewField : ViewObject, IProvidableObject {

        /// <summary>
        /// Display title of field.
        /// </summary>
        public string Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PersistentFieldForString title =
            new PersistentFieldForString(nameof(Title));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewField()
            : base() {
            this.RegisterPersistentField(this.title);
        }

        /// <summary>
        /// Gets the read-only value of a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to get
        /// read-only value of</param>
        /// <param name="topmostPresentableObject">topmost
        /// presentable parent object to get read-only value of</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>read-only value of presentable field</returns>
        public virtual string GetReadOnlyValueFor(IPresentableField presentableField, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            throw new NotImplementedException(nameof(GetReadOnlyValueFor) + "() has to be implemented in each derived class of view field.");
        }

        /// <summary>
        /// Gets the title of providable object.
        /// </summary>
        /// <returns>title of providable object</returns>
        public string GetTitle() {
            return this.Title;
        }

        /// <summary>
        /// Orders a list of view fields by key chain.
        /// </summary>
        /// <param name="viewFields">list of view fields to be
        /// ordered by key chain</param>
        public static void SortByKeyChain(List<ViewField> viewFields) {
            viewFields.Sort(delegate (ViewField a, ViewField b) {
                int result;
                var aAsViewFieldForEditableValue = a as ViewFieldForEditableValue;
                var bAsViewFieldForEditableValue = b as ViewFieldForEditableValue;
                if (null == aAsViewFieldForEditableValue && null == bAsViewFieldForEditableValue) {
                    result = 0;
                } else if (null == aAsViewFieldForEditableValue) {
                    result = -1;
                } else if (null == bAsViewFieldForEditableValue) {
                    result = 1;
                } else {
                    result = aAsViewFieldForEditableValue.Key.CompareTo(bAsViewFieldForEditableValue.Key);
                }
                return result;
            });
            return;
        }

    }

}