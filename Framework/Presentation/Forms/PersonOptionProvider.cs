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
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for any provider class of person options to be 
    /// pulled in fields of views.
    /// </summary>
    public abstract class PersonOptionProvider : OptionProvider {

        /// <summary>
        /// Indicates whether users should be resolved from 
        /// Active Directory in read-only mode if they can not be 
        /// found in the options (any more).
        /// </summary>
        public bool IsResolvingMissingUsersInReadOnlyMode {
            get { return this.isResolvingMissingUsersInReadOnlyMode.Value; }
            set { this.isResolvingMissingUsersInReadOnlyMode.Value = value; }
        }
        private readonly PersistentFieldForBool isResolvingMissingUsersInReadOnlyMode =
            new PersistentFieldForBool(nameof(IsResolvingMissingUsersInReadOnlyMode), false);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PersonOptionProvider()
            : base() {
            this.RegisterPersistentField(this.isResolvingMissingUsersInReadOnlyMode);
        }

        /// <summary>
        /// Finds the read-only value for a key.
        /// </summary>
        /// <param name="key">key to get value for</param>
        /// <param name="options">options to find value for key in</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>value for key if key is contained or null if key
        /// is not contained</returns>
        protected override string FindReadOnlyValueForKey(string key, IEnumerable<KeyValuePair<string, string>> options, IOptionDataProvider optionDataProvider) {
            var readOnlyValueForKey = base.FindReadOnlyValueForKey(key, options, optionDataProvider);
            if (this.IsResolvingMissingUsersInReadOnlyMode && string.IsNullOrEmpty(readOnlyValueForKey)) {
                var filterCriteriaForUser = new FilterCriteria(nameof(IUser.UserName), RelationalOperator.IsEqualTo, key, FilterTarget.IsOtherTextValue);
                var user = optionDataProvider.UserDirectory.FindOne(filterCriteriaForUser, SortCriterionCollection.Empty);
                readOnlyValueForKey = user?.DisplayName;
            }
            return readOnlyValueForKey;
        }

        /// <summary>
        /// Gets all options of this option provider (IDs of
        /// objects or user names of users need to be set as keys).
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost presentable 
        /// object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>all options of this option provider (IDs of
        /// objects or user names of users need to be set as keys)</returns>
        public sealed override IEnumerable<KeyValuePair<string, string>> GetOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            foreach (var personOption in this.GetPersonOptions(parentPresentableObject, topmostPresentableObject, optionDataProvider)) {
                yield return new KeyValuePair<string, string>(personOption.Key?.UserName, personOption.Value);
            }
        }

        /// <summary>
        /// Gets all options of this option provider.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>all options of this option provider</returns>
        public abstract IEnumerable<KeyValuePair<IUser, string>> GetPersonOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider);

    }

}