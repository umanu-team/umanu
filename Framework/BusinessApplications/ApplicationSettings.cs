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
    using Framework.Persistence.Filters;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Persistence.Directories;
    using Persistence.Fields;
    using Properties;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Singleton application settings.
    /// </summary>
    public abstract class ApplicationSettings : PersistentObject, IProvidableObject {

        /// <summary>
        /// Group of administrators.
        /// </summary>
        public Group Administrators {
            get { return this.administrators.Value; }
            private set { this.administrators.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<Group> administrators =
            new PersistentFieldForPersistentObject<Group>(nameof(Administrators), CascadedRemovalBehavior.RemoveValuesForcibly);

        /// <summary>
        /// Number of queue jobs to be executed or in execution.
        /// </summary>
        public int JobQueueCount {
            get { return JobQueue.Count; }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ApplicationSettings()
            : base() {
            this.Administrators = new Group(nameof(this.Administrators));
            this.RegisterPersistentField(this.administrators);
        }

        /// <summary>
        /// Makes sure that administrator group contains at least all
        /// users of the related Active Directoy group.
        /// </summary>
        /// <param name="administratorGroupDistinguishedName">
        /// distinguished name of Active Directory group containing
        /// all .NET administrators</param>
        public virtual void EnsureAccessForAdministrators(string administratorGroupDistinguishedName) {
            if (string.IsNullOrEmpty(administratorGroupDistinguishedName)) {
                throw new Exception("Distinguished name of .NET administrator group in Active Directory may not be null or empty.");
            } else {
                var activeDirectory = this.ParentPersistentContainer.ParentPersistenceMechanism.UserDirectory as ActiveDirectory;
                var activeDirectoryAdministratorGroup = activeDirectory.FindGroupByDistinguishedName(administratorGroupDistinguishedName);
                this.Administrators.Members.AddRangeIfNotContained(activeDirectoryAdministratorGroup);
                this.Administrators.UpdateCascadedly();
            }
            return;
        }

        /// <summary>
        /// Finds the settings object in a persistence mechanism.
        /// </summary>
        /// <param name="optionDataProvider">option data provider
        /// to get data from</param>
        /// <returns>settings object of persistence mechanism</returns>
        /// <typeparam name="TApplicationSettings">type of
        /// application settings</typeparam>
        protected static TApplicationSettings FindOneIn<TApplicationSettings>(IOptionDataProvider optionDataProvider) where TApplicationSettings : ApplicationSettings, new() {
            return optionDataProvider.FindOne<TApplicationSettings>(FilterCriteria.Empty, SortCriterionCollection.Empty);
        }

        /// <summary>
        /// Finds the settings object in a persistence mechanism.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        /// <returns>settings object of persistence mechanism</returns>
        /// <typeparam name="TApplicationSettings">type of
        /// application settings</typeparam>
        protected static TApplicationSettings FindOneIn<TApplicationSettings>(PersistenceMechanism persistenceMechanism) where TApplicationSettings : ApplicationSettings, new() {
            return persistenceMechanism.FindContainer<TApplicationSettings>().FindOne(FilterCriteria.Empty, SortCriterionCollection.Empty);
        }

        /// <summary>
        /// Gets all presentable fields.
        /// </summary>
        /// <returns>enumerable of presentable fields</returns>
        protected override IEnumerable<IPresentableField> GetPresentableFields() {
            foreach (var presentableField in base.GetPresentableFields()) {
                yield return presentableField;
            }
            yield return new PresentableFieldForCalculatedValue<int>(this, nameof(ApplicationSettings.JobQueueCount), delegate () {
                return this.JobQueueCount;
            });
        }

        /// <summary>
        /// Gets the title of providable object.
        /// </summary>
        /// <returns>title of providable object</returns>
        public virtual string GetTitle() {
            return Resources.Settings;
        }

    }

}