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

namespace Framework.BusinessApplications {

    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Option provider for settings.
    /// </summary>
    public class SettingsOptionProvider<T> : OptionProvider where T : ApplicationSettings, new() {

        /// <summary>
        /// If key of presentable object is set to this property, the
        /// current value is returned even if it is not contained as
        /// option in settings any more. Set this property to avoid
        /// disappearal of values on removal of options.
        /// </summary>
        public string PresentableObjectKey {
            get { return this.presentableObjectKey.Value; }
            set { this.presentableObjectKey.Value = value; }
        }
        private readonly PersistentFieldForString presentableObjectKey =
            new PersistentFieldForString(nameof(PresentableObjectKey));

        /// <summary>
        /// If key chain of presentable object is set to this
        /// property, the current value is returned even if it is not
        /// contained as option in settings any more. Set this
        /// property to avoid disappearal of values on removal of
        /// options.
        /// </summary>
        public string[] PresentableObjectKeyChain {
            get { return KeyChain.FromKey(this.PresentableObjectKey); }
            set { this.PresentableObjectKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of settings property to get options from.
        /// </summary>
        public string SettingsKey {
            get { return this.settingsKey.Value; }
            set { this.settingsKey.Value = value; }
        }
        private readonly PersistentFieldForString settingsKey =
            new PersistentFieldForString(nameof(SettingsKey));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public SettingsOptionProvider()
            : base() {
            this.RegisterPersistentField(this.presentableObjectKey);
            this.RegisterPersistentField(this.settingsKey);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="settingsKey">key of settings property to get
        /// options from</param>
        public SettingsOptionProvider(string settingsKey)
            : this() {
            this.SettingsKey = settingsKey;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="settingsKey">key of settings property to get
        /// options from</param>
        /// <param name="presentableObjectKey">if key of presentable
        /// object is provided, the current value is returned even if
        /// it is not contained as option in settings any more</param>
        public SettingsOptionProvider(string settingsKey, string presentableObjectKey)
            : this(settingsKey) {
            this.PresentableObjectKey = presentableObjectKey;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="settingsKey">key of settings property to get
        /// options from</param>
        /// <param name="presentableObjectKeyChain">if key chain of
        /// presentable object is provided, the current value is
        /// returned even if it is not contained as option in
        /// settings any more</param>
        public SettingsOptionProvider(string settingsKey, string[] presentableObjectKeyChain)
            : this(settingsKey) {
            this.PresentableObjectKeyChain = presentableObjectKeyChain;
        }

        /// <summary>
        /// Gets all options of this option provider (IDs of
        /// objects or user names of users need to be set as keys).
        /// </summary>
        /// <param name="topmostPresentableObject">topmost presentable
        /// object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>all options of this option provider (IDs of
        /// objects or user names of users need to be set as keys)</returns>
        public IEnumerable<KeyValuePair<string, string>> GetOptions(IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            if (!string.IsNullOrEmpty(this.PresentableObjectKey)) {
                var presentableField = topmostPresentableObject.FindPresentableField(this.PresentableObjectKeyChain);
                var presentableFieldForElement = presentableField as IPresentableFieldForElement;
                if (null == presentableFieldForElement) {
                    var presentableFieldForCollection = presentableField as IPresentableFieldForCollection;
                    foreach (var value in presentableFieldForCollection.GetValuesAsString()) {
                        yield return new KeyValuePair<string, string>(value, value);
                    }
                } else {
                    yield return new KeyValuePair<string, string>(presentableFieldForElement.ValueAsString, presentableFieldForElement.ValueAsString);
                }
            }
            var settings = optionDataProvider.FindOne<T>(FilterCriteria.Empty, SortCriterionCollection.Empty);
            var options = settings.FindPresentableField(this.SettingsKey) as IPresentableFieldForCollection<string>;
            foreach (var option in options) {
                if (!string.IsNullOrEmpty(option)) {
                    string value = option.Trim();
                    yield return new KeyValuePair<string, string>(value, value);
                }
            }
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
        public override IEnumerable<KeyValuePair<string, string>> GetOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            return this.GetOptions(topmostPresentableObject, optionDataProvider);
        }

    }

}