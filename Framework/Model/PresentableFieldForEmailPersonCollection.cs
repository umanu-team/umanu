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

namespace Framework.Model {

    using Presentation;
    using Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;

    /// <summary>
    /// Field for element of type IPerson to be presented.
    /// </summary>
    internal sealed class PresentableFieldForEmailPersonCollection : PresentableFieldForCollection<IPerson> {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return typeof(IPerson);
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        public PresentableFieldForEmailPersonCollection(IPresentableObject parentPresentableObject, string key)
            : base(parentPresentableObject, key) {
            // nothing to do
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public override IEnumerable<string> GetValuesAsString() {
            foreach (var value in this) {
                if (null == value) {
                    yield return string.Empty;
                } else {
                    yield return EmailTemplate.GetMailAddressStringFor(value);
                }
            }
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of values of this field and adds it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="item">new value to add</param>
        /// <returns>true if value could be added successfully, false
        /// otherwise</returns>
        public override bool TryAddString(string item) {
            bool success;
            if (string.IsNullOrEmpty(item)) {
                this.Add(null);
                success = true;
            } else {
                try {
                    var mailAddress = new MailAddress(item);
                    if (string.IsNullOrEmpty(mailAddress.Address)) {
                        success = false;
                    } else {
                        this.Add(new Person(mailAddress.DisplayName, mailAddress.Address));
                        success = true;
                    }
                } catch (FormatException) {
                    success = false;
                }
            }
            return success;
        }

    }

}