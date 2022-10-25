﻿/*********************************************************************
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
    using Framework.Properties;

    /// <summary>
    /// Field for lookup value to be presented in a view.
    /// </summary>
    public abstract class ViewFieldForLookup : ViewFieldForElementWithPlaceholder, IViewFieldWithLookupProvider {

        /// <summary>
        /// Minimum length of the search term to
        /// trigger a lookup.
        /// </summary>
        public byte MinSearchLength {
            get { return this.minSearchLength.Value; }
            set { this.minSearchLength.Value = value; }
        }
        private readonly PersistentFieldForByte minSearchLength =
            new PersistentFieldForByte(nameof(MinSearchLength), 1);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForLookup()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage = Resources.PleaseSelectAValidValueForThisField;
            errorMessage += " " + this.GetInfoMessageAboutManditoriness();
            return errorMessage;
        }

        /// <summary>
        /// Gets a boolean value indicating whether values not
        /// contained in lookup provider are allowed.
        /// </summary>
        /// <returns>true if values not contained in lookup provider
        /// are allowed, false otherwise</returns>
        public virtual bool GetIsFillInAllowed() {
            return false;
        }

        /// <summary>
        /// Gets the provider for suggested values.
        /// </summary>
        public abstract LookupProvider GetLookupProvider();

    }

}