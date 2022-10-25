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

namespace Framework.Presentation {

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Model;

    /// <summary>
    /// View for search result panes.
    /// </summary>
    public sealed class SearchResultView : PersistentObject {

        /// <summary>
        /// Key of description field.
        /// </summary>
        public string DescriptionKey {
            get { return this.descriptionKey.Value; }
            set { this.descriptionKey.Value = value; }
        }
        private readonly PersistentFieldForString descriptionKey =
            new PersistentFieldForString(nameof(DescriptionKey));

        /// <summary>
        /// Internal key chain of description field.
        /// </summary>
        public string[] DescriptionKeyChain {
            get { return KeyChain.FromKey(this.DescriptionKey); }
            set { this.DescriptionKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of title field.
        /// </summary>
        public string TitleKey {
            get { return this.titleKey.Value; }
            set { this.titleKey.Value = value; }
        }
        private readonly PersistentFieldForString titleKey =
            new PersistentFieldForString(nameof(TitleKey));

        /// <summary>
        /// Internal key chain of title field.
        /// </summary>
        public string[] TitleKeyChain {
            get { return KeyChain.FromKey(this.TitleKey); }
            set { this.TitleKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public SearchResultView()
            : base() {
            this.RegisterPersistentField(this.descriptionKey);
            this.RegisterPersistentField(this.titleKey);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="descriptionKey">key of description field</param>
        public SearchResultView(string descriptionKey)
            : this() {
            this.DescriptionKey = descriptionKey;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="descriptionKeyChain">key chain of
        /// description field</param>
        public SearchResultView(string[] descriptionKeyChain)
            : this() {
            this.DescriptionKeyChain = descriptionKeyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="titleKey">key of title field</param>
        /// <param name="descriptionKey">key of description field</param>
        public SearchResultView(string titleKey, string descriptionKey)
            : this(descriptionKey) {
            this.TitleKey = titleKey;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="titleKeyChain">key chain of title field</param>
        /// <param name="descriptionKey">key of description field</param>
        public SearchResultView(string[] titleKeyChain, string descriptionKey)
            : this(descriptionKey) {
            this.TitleKeyChain = titleKeyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="titleKey">key of title field</param>
        /// <param name="descriptionKeyChain">key chain of
        /// description field</param>
        public SearchResultView(string titleKey, string[] descriptionKeyChain)
            : this(descriptionKeyChain) {
            this.TitleKey = titleKey;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="titleKeyChain">key chain of title field</param>
        /// <param name="descriptionKeyChain">key chain of
        /// description field</param>
        public SearchResultView(string[] titleKeyChain, string[] descriptionKeyChain)
            : this(descriptionKeyChain) {
            this.TitleKeyChain = titleKeyChain;
        }

    }

}