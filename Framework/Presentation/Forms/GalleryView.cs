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

    using Framework.Model;
    using Persistence;
    using Persistence.Fields;
    using System;

    /// <summary>
    /// Represents a gallery view for a collection of images.
    /// </summary>
    public class GalleryView : PersistentObject {

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
        /// View field for image files of gallery.
        /// </summary>
        public ViewFieldForImageFile ViewFieldForImageFile {
            get { return this.viewFieldForImageFile.Value; }
            set { this.viewFieldForImageFile.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<ViewFieldForImageFile> viewFieldForImageFile =
            new PersistentFieldForPersistentObject<ViewFieldForImageFile>(nameof(ViewFieldForImageFile), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// List of fields for elements contained in this view.
        /// </summary>
        public IConvenientList<ViewField> ViewFields {
            get {
                return this.viewFields;
            }
        }
        private readonly PersistentFieldForPersistentObjectCollection<ViewField> viewFields;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public GalleryView()
            : base() {
            this.RegisterPersistentField(this.title);
            this.RegisterPersistentField(this.viewFieldForImageFile);
            this.viewFields = new PersistentFieldForPersistentObjectCollection<ViewField>(nameof(this.ViewFields), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.viewFields);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="viewFieldForImageFile">view field for image
        /// files of gallery</param>
        public GalleryView(ViewFieldForImageFile viewFieldForImageFile)
            : this() {
            this.ViewFieldForImageFile = viewFieldForImageFile;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="listTableView">list table view to create
        /// gallery view for - the first view field for image file
        /// will be used as the gallery image</param>
        public GalleryView(IListTableView listTableView)
            : this() {
            this.Title = listTableView.Title;
            foreach (var viewField in listTableView.ViewFields) {
                var viewFieldForImageFile = viewField as ViewFieldForImageFile;
                if (null == viewFieldForImageFile || null != this.ViewFieldForImageFile) {
                    this.ViewFields.Add(viewField);
                } else {
                    this.ViewFieldForImageFile = viewFieldForImageFile;
                }
            }
            if (null == this.ViewFieldForImageFile) {
                throw new ArgumentException("List table view does not contain a view field for image file.", nameof(listTableView));
            }
        }

    }

}