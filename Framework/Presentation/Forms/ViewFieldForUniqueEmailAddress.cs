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

    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using Framework.Properties;

    /// <summary>
    /// Field for a directory unique e-mail address to be presented
    /// in a view.
    /// </summary>
    public class ViewFieldForUniqueEmailAddress : ViewFieldForEmailAddress {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForUniqueEmailAddress()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForUniqueEmailAddress(string title, string key, Mandatoriness mandatoriness)
            : base(title, key, mandatoriness) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForUniqueEmailAddress(string title, string[] keyChain, Mandatoriness mandatoriness)
            : base(title, keyChain, mandatoriness) {
            // nothing to do
        }

        /// <summary>
        /// Returns null if the specified value is valid, an error
        /// message otherwise.
        /// </summary>
        /// <param name="presentableField">presentable field to be
        /// validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>null if the specified value is valid, error
        /// message otherwise</returns>
        public override string Validate(IPresentableFieldForElement presentableField, ValidityCheck validityCheck, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string errorMessage = base.Validate(presentableField, validityCheck, presentableObject, optionDataProvider);
            string value = presentableField.ValueAsString;
            if (string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(value)) {
                var filterCriteria = new FilterCriteria(nameof(IUser.Id), RelationalOperator.IsNotEqualTo, presentableObject.Id)
                    .And(nameof(IUser.EmailAddress), RelationalOperator.IsEqualTo, value, FilterTarget.IsOtherTextValue);
                if (optionDataProvider.UserDirectory.Contains(filterCriteria)) {
                    errorMessage = Resources.PleaseEnterAUniqueEmailAddressForThisField;
                }
            }
            return errorMessage;
        }

    }

}