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
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Resources;

    /// <summary>
    /// Option provider for loading localized string ressources from
    /// a specified assembly.
    /// </summary>
    public class AssemblyResourceOptionProvider : OptionProvider {

        /// <summary>
        /// Resource manager to load the string resources from.
        /// </summary>
        protected ResourceManager ResourceManager {
            get {
                if (null == this.resourceManager) {
                    this.resourceManager = new ResourceManager(this.ResourceSource);
                }
                return this.resourceManager;
            }
        }
        private ResourceManager resourceManager;

        /// <summary>
        /// Type of resource to load string resources from.
        /// </summary>
        public Type ResourceSource {
            get {
                return Type.GetType(this.resourceSource.Value);
            }
            set {
                this.resourceSource.Value = value.AssemblyQualifiedName;
                this.resourceManager = null;
            }
        }
        private readonly PersistentFieldForString resourceSource =
            new PersistentFieldForString(nameof(ResourceSource));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public AssemblyResourceOptionProvider()
            : base() {
            this.RegisterPersistentField(this.resourceSource);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="resourceSource">type of resource to load
        /// string resources from</param>
        public AssemblyResourceOptionProvider(Type resourceSource)
            : this() {
            this.ResourceSource = resourceSource;
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
        public override IEnumerable<KeyValuePair<string, string>> GetOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            ResourceSet resourceSet = this.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
            foreach (DictionaryEntry resource in resourceSet) {
                string key = resource.Key as string;
                if (!string.IsNullOrEmpty(key)) {
                    yield return new KeyValuePair<string, string>(key, this.ResourceManager.GetString(key));
                }
            }
        }

    }

}