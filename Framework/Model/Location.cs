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

    /// <summary>
    /// Represents a location.
    /// </summary>
    public class Location : PostalAddress {

        /// <summary>
        /// Attachment files associated with this location.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<File> Attachments { get; private set; }

        /// <summary>
        /// Contact information associated with the location.
        /// </summary>
        public Group Contacts {
            get { return this.contacts.Value; }
            set { this.contacts.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<Group> contacts =
            new PersistentFieldForPersistentObject<Group>(nameof(Contacts), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Description of location.
        /// </summary>
        public string Description {
            get { return this.description.Value; }
            set { this.description.Value = value; }
        }
        private readonly PersistentFieldForString description =
            new PersistentFieldForString(nameof(Description));

        /// <summary>
        /// Opening hours of location.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<OpeningHours> OpeningHours { get; private set; }

        /// <summary>
        /// Photo of location.
        /// </summary>
        public ImageFile Photo {
            get { return this.photo.Value; }
            set { this.photo.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<ImageFile> photo =
            new PersistentFieldForPersistentObject<ImageFile>(nameof(Photo), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Name of location.
        /// </summary>
        public string Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PersistentFieldForString title =
            new PersistentFieldForString(nameof(Title));

        /// <summary>
        /// URL of web site.
        /// </summary>
        public string WebSite {
            get { return this.webSite.Value; }
            set { this.webSite.Value = value; }
        }
        private readonly PersistentFieldForString webSite =
            new PersistentFieldForString(nameof(WebSite));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Location()
            : base() {
            this.Attachments = new PersistentFieldForPersistentObjectCollection<File>(nameof(Attachments), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.Attachments);
            this.Contacts = new Group("Location Contacts");
            this.RegisterPersistentField(this.contacts);
            this.RegisterPersistentField(this.description);
            this.OpeningHours = new PersistentFieldForPersistentObjectCollection<OpeningHours>(nameof(OpeningHours), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.OpeningHours);
            this.RegisterPersistentField(this.photo);
            this.RegisterPersistentField(this.title);
            this.RegisterPersistentField(this.webSite);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">name of location</param>
        public Location(string title)
            : this() {
            this.Title = title;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">name of location</param>
        /// <param name="latitude">latitudes north of the equator
        /// shall be specified as a positive value less than or equal
        /// to 90. Latitudes south of the equator shall be specified
        /// as a negative value</param>
        /// <param name="longitude">longitudes east of the prime
        /// meridian shall be specified as a positive value  less
        /// than or equal to 180. Longitudes west of the meridian
        /// shall be specified as a negative value</param>
        public Location(string title, decimal latitude, decimal longitude)
            : this(title) {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

    }

}
