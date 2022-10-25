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
    using Model;
    using Persistence.Fields;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a view for a pane of cards.
    /// </summary>
    public class CardPaneView : PersistentObject {

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
        /// Key chain of description field.
        /// </summary>
        public string[] DescriptionKeyChain {
            get { return KeyChain.FromKey(this.DescriptionKey); }
            set { this.DescriptionKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of field to group items by.
        /// </summary>
        public string GroupByKey {
            get { return this.groupByKey.Value; }
            set { this.groupByKey.Value = value; }
        }
        private readonly PersistentFieldForString groupByKey =
            new PersistentFieldForString(nameof(GroupByKey));

        /// <summary>
        /// Key chain of field to group items by.
        /// </summary>
        public string[] GroupByKeyChain {
            get { return KeyChain.FromKey(this.GroupByKey); }
            set { this.GroupByKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of image field.
        /// </summary>
        public string ImageKey {
            get { return this.imageKey.Value; }
            set { this.imageKey.Value = value; }
        }
        private readonly PersistentFieldForString imageKey =
            new PersistentFieldForString(nameof(ImageKey));

        /// <summary>
        /// Key chain of image field.
        /// </summary>
        public string[] ImageKeyChain {
            get { return KeyChain.FromKey(this.ImageKey); }
            set { this.ImageKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Maximum length to truncate long description text to.
        /// </summary>
        public int MaxLengthOfLongDescription {
            get { return this.maxLengthOfLongDescription.Value; }
            set { this.maxLengthOfLongDescription.Value = value; }
        }
        private readonly PersistentFieldForInt maxLengthOfLongDescription =
            new PersistentFieldForInt(nameof(MaxLengthOfLongDescription), 376);

        /// <summary>
        /// Maximum length to truncate short description text to.
        /// </summary>
        public int MaxLengthOfShortDescription {
            get { return this.maxLengthOfShortDescription.Value; }
            set { this.maxLengthOfShortDescription.Value = value; }
        }
        private readonly PersistentFieldForInt maxLengthOfShortDescription =
            new PersistentFieldForInt(nameof(MaxLengthOfShortDescription), 160);

        /// <summary>
        /// Side length to scale image to.
        /// </summary>
        public int SideLength {
            get { return this.sideLength.Value; }
            set { this.sideLength.Value = value; }
        }
        private readonly PersistentFieldForInt sideLength =
            new PersistentFieldForInt(nameof(SideLength), 192);

        /// <summary>
        /// Title of view.
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
        public CardPaneView()
            : base() {
            this.RegisterPersistentField(this.descriptionKey);
            this.RegisterPersistentField(this.groupByKey);
            this.RegisterPersistentField(this.imageKey);
            this.RegisterPersistentField(this.maxLengthOfLongDescription);
            this.RegisterPersistentField(this.maxLengthOfShortDescription);
            this.RegisterPersistentField(this.sideLength);
            this.RegisterPersistentField(this.title);
        }

        /// <summary>
        /// Gets the CSS classes for a tile respresenting a
        /// presentable object.
        /// </summary>
        /// <param name="presentableObject">presentable object to get
        /// CSS classes for</param>
        /// <returns>CSS classes for a presentable object</returns>
        public virtual IEnumerable<string> GetCssClasses(IPresentableObject presentableObject) {
            yield break;
        }

    }

}