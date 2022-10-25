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

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using System.Threading;

    /// <summary>
    /// Represents a multilingual string with invariant value and
    /// multiple translations.
    /// </summary>
    public sealed class MultilingualString : PersistentObject {

        /// <summary>
        /// Invariant value.
        /// </summary>
        public string InvariantValue {
            get { return this.invariantValue.Value; }
            set { this.invariantValue.Value = value; }
        }
        private readonly PersistentFieldForString invariantValue =
            new PersistentFieldForString(nameof(InvariantValue));

        /// <summary>
        /// List of translations with ISO 639-1 two letter language
        /// code as key.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<KeyValuePair> Translations { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public MultilingualString()
            : base() {
            this.RegisterPersistentField(this.invariantValue);
            this.Translations = new PersistentFieldForPersistentObjectCollection<KeyValuePair>(nameof(this.Translations), CascadedRemovalBehavior.RemoveValuesForcibly);
            this.Translations.PreviousKeys.Add("Options"); // TODO: Remov this line after successfull migration of all projects.
            this.RegisterPersistentField(this.Translations);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="invariantValue">invariant value</param>
        public MultilingualString(string invariantValue)
            : this() {
            this.InvariantValue = invariantValue;
        }

        /// <summary>
        /// Gets the translated value for the current culture.
        /// </summary>
        /// <returns>translated value for current culture</returns>
        public override string ToString() {
            string translatedValue = this.InvariantValue;
            var twoLetterISOLanguageName = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            foreach (var translation in this.Translations) {
                if (translation.KeyField == twoLetterISOLanguageName) {
                    translatedValue = translation.Value;
                    break;
                }
            }
            return translatedValue;
        }

    }

}