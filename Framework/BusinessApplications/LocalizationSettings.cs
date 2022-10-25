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

    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;

    /// <summary>
    /// Localization settings for a user.
    /// </summary>
    internal sealed class LocalizationSettings : IProvidableObject {

        /// <summary>
        /// Allowed groups for reading/writing this object.
        /// </summary>
        public AllowedGroups AllowedGroups {
            get {
                var allowedGroups = new AllowedGroups();
                var allUsers = new Group("All users");
                allUsers.Members.Add(UserDirectory.AnonymousUser);
                allowedGroups.ForReading.Add(allUsers);
                return allowedGroups;
            }
        }

        /// <summary>
        /// Time of creation of this object.
        /// </summary>
        public DateTime CreatedAt {
            get { return DateTime.Now; }
        }

        /// <summary>
        /// User who created this object.
        /// </summary>
        public IUser CreatedBy {
            get { return UserDirectory.AnonymousUser; }
        }

        /// <summary>
        /// Preferred culture.
        /// </summary>
        public CultureInfo Culture {
            get {
                CultureInfo cultureInfo;
                if (string.IsNullOrEmpty(this.CultureName)) {
                    cultureInfo = CultureInfo.InvariantCulture;
                } else {
                    cultureInfo = CultureInfo.GetCultureInfo(this.CultureName);
                }
                return cultureInfo;
            }
            set {
                if (null == value) {
                    this.CultureName = null;
                } else {
                    this.CultureName = value.Name;
                }
            }
        }

        /// <summary>
        /// Short name of preferred culture.
        /// </summary>
        public string CultureName {
            get {
                return this.cultureName.Value;
            }
            set {
                this.cultureName.Value = value;
            }
        }
        private PresentableFieldForString cultureName;

        /// <summary>
        /// Indicates whether this object has versions.
        /// </summary>
        public bool HasVersions {
            get { return false; }
        }

        /// <summary>
        /// Globally unique identifier of this object.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// True if this is a new object, false otherwise.
        /// </summary>
        public bool IsNew {
            get { return true; }
        }

        /// <summary>
        /// True if this object is read-only, false otherwise.
        /// </summary>
        public bool IsWriteProtected {
            get { return false; }
        }

        /// <summary>
        /// Time of last modification.
        /// </summary>
        public DateTime ModifiedAt {
            get { return DateTime.Now; }
        }

        /// <summary>
        /// User who modified this object.
        /// </summary>
        public IUser ModifiedBy {
            get { return UserDirectory.AnonymousUser; }
        }

        /// <summary>
        /// Directory password.
        /// </summary>
        public string Password {
            get {
                return this.password.Value;
            }
            set {
                this.password.Value = value;
            }
        }
        private PresentableFieldForString password;

        /// <summary>
        /// Indicates whether to remove this object on update.
        /// </summary>
        public RemovalType RemoveOnUpdate { get; set; }

        /// <summary>
        /// Enumerable of previous persistent versions of this
        /// object.
        /// </summary>
        public ReadOnlyCollection<Model.Version> Versions {
            get { return new List<Model.Version>(0).AsReadOnly(); }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public LocalizationSettings() {
            this.cultureName = new PresentableFieldForString(this, nameof(CultureName));
            this.Id = Guid.NewGuid();
            this.password = new PresentableFieldForString(this, nameof(Password));
        }

        /// <summary>
        /// Finds the first presentable field for a specified key.
        /// </summary>
        /// <param name="key">key to find presentable field for</param>
        /// <returns>first presentable field for specified key or
        /// null</returns>
        public IPresentableField FindPresentableField(string key) {
            return this.FindPresentableField(KeyChain.FromKey(key));
        }

        /// <summary>
        /// Finds the first presentable field for a specified key
        /// chain.
        /// </summary>
        /// <param name="keyChain">key chain to find presentable
        /// field for</param>
        /// <returns>first presentable field for specified key chain
        /// or null</returns>
        public IPresentableField FindPresentableField(string[] keyChain) {
            IPresentableField resultField;
            if (1L == keyChain.LongLength && nameof(LocalizationSettings.CultureName) == keyChain[0]) {
                resultField = this.cultureName;
            } else if (1L == keyChain.LongLength && nameof(LocalizationSettings.Password) == keyChain[0]) {
                resultField = this.password;
            } else {
                resultField = null;
            }
            return resultField;
        }

        /// <summary>
        /// Finds all presentable fields for a specified key.
        /// </summary>
        /// <param name="key">key to find presentable fields for</param>
        /// <returns>all presentable fields for specified key or null</returns>
        public IEnumerable<IPresentableField> FindPresentableFields(string key) {
            yield return this.FindPresentableField(key);
        }

        /// <summary>
        /// Finds all presentable fields for a specified key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find presentable
        /// fields for</param>
        /// <returns>all presentable fields for specified key chain
        /// or null</returns>
        public IEnumerable<IPresentableField> FindPresentableFields(string[] keyChain) {
            yield return this.FindPresentableField(keyChain);
        }

        /// <summary>
        /// Gets the title of object.
        /// </summary>
        public string GetTitle() {
            return string.Empty;
        }

    }

}