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
    using Framework.Presentation.Forms;
    using Model;
    using Persistence.Directories;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Object that can be presented in a list or inside of a form.
    /// </summary>
    public class PresentableObject : IPresentableObject {

        /// <summary>
        /// Time of creation of object.
        /// </summary>
        public DateTime CreatedAt {
            get { return this.createdAt.Value; }
            protected set { this.createdAt.Value = value; }
        }
        private readonly PresentableFieldForDateTime createdAt;

        /// <summary>
        /// User who created object.
        /// </summary>
        public IUser CreatedBy {
            get { return this.createdBy.Value; }
            protected set { this.createdBy.Value = value; }
        }
        private readonly PresentableFieldForIUser createdBy;

        /// <summary>
        /// Globally unique identifier of object.
        /// </summary>
        public Guid Id {
            get { return this.id.Value; }
            protected set { this.id.Value = value; }
        }
        private readonly PresentableFieldForGuid id;

        /// <summary>
        /// True if this is a new object, false otherwise.
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Keys of presentable fields.
        /// </summary>
        public IEnumerable<string> Keys {
            get {
                return this.presentableFields.Keys;
            }
        }

        /// <summary>
        /// Time of last modification.
        /// </summary>
        public DateTime ModifiedAt {
            get { return this.modifiedAt.Value; }
            protected set { this.modifiedAt.Value = value; }
        }
        private readonly PresentableFieldForDateTime modifiedAt;

        /// <summary>
        /// User who modified object.
        /// </summary>
        public IUser ModifiedBy {
            get { return this.modifiedBy.Value; }
            protected set { this.modifiedBy.Value = value; }
        }
        private readonly PresentableFieldForIUser modifiedBy;

        /// <summary>
        /// Indicates whether to remove this object on update.
        /// </summary>
        public RemovalType RemoveOnUpdate { get; set; }

        /// <summary>
        /// Collection of all presentable fields of this presentable
        /// object.
        /// </summary>
        private PresentableFieldCollection presentableFields;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PresentableObject()
            : base() {
            this.presentableFields = new PresentableFieldCollection();
            this.createdAt = new PresentableFieldForDateTime(this, nameof(PresentableObject.CreatedAt), UtcDateTime.Now);
            this.AddPresentableField(createdAt);
            this.createdBy = new PresentableFieldForIUser(this, nameof(PresentableObject.CreatedBy), UserDirectory.AnonymousUser);
            this.AddPresentableField(this.createdBy);
            this.id = new PresentableFieldForGuid(this, nameof(PresentableObject.Id), Guid.NewGuid());
            this.AddPresentableField(this.id);
            this.IsNew = true;
            this.modifiedAt = new PresentableFieldForDateTime(this, nameof(PresentableObject.ModifiedAt), this.CreatedAt);
            this.AddPresentableField(this.modifiedAt);
            this.modifiedBy = new PresentableFieldForIUser(this, nameof(PresentableObject.ModifiedBy), this.CreatedBy);
            this.AddPresentableField(modifiedBy);
            this.RemoveOnUpdate = RemovalType.False;
        }

        /// <summary>
        /// Adds a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to add</param>
        public void AddPresentableField(IPresentableField presentableField) {
            this.presentableFields.Add(presentableField);
            return;
        }

        /// <summary>
        /// Adds or updates a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to add
        /// or update</param>
        public void AddOrUpdatePresentableField(IPresentableField presentableField) {
            var existingField = this.FindPresentableField(presentableField.Key);
            if (null != existingField) {
                this.presentableFields.Remove(existingField);
            }
            this.presentableFields.Add(presentableField);
            return;
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
            IPresentableField resultField = null;
            foreach (var presentableField in this.FindPresentableFields(keyChain)) {
                if (null == resultField) {
                    resultField = presentableField;
                } else {
                    throw new ArgumentException("The key chain \"" + KeyChain.ToKey(keyChain) + "\" is not unique for persistent object of type " + this.GetType().FullName + ".", nameof(keyChain));
                }
            }
            return resultField;
        }

        /// <summary>
        /// Finds all presentable fields for a specified key.
        /// </summary>
        /// <param name="key">key to find presentable fields for</param>
        /// <returns>all presentable fields for specified key or null</returns>
        public IEnumerable<IPresentableField> FindPresentableFields(string key) {
            return this.FindPresentableFields(KeyChain.FromKey(key));
        }

        /// <summary>
        /// Finds all presentable fields for a specified key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find presentable
        /// fields for</param>
        /// <returns>all presentable fields for specified key chain
        /// or null</returns>
        public IEnumerable<IPresentableField> FindPresentableFields(string[] keyChain) {
            return PresentableFieldBrowser.FindPresentableFields(keyChain, this.presentableFields);
        }

    }

}