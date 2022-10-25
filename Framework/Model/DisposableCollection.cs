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

    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Generic collection of disposable objects.
    /// </summary>
    /// <typeparam name="T">type of disposable objects</typeparam>
    public class DisposableCollection<T> : Collection<T>, IDisposable where T : IDisposable {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public DisposableCollection()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Disposes all contained disposable objects.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
            return;
        }

        /// <summary>
        /// Frees all unmanaged resources and also managed resources
        /// if desired.
        /// </summary>
        /// <param name="freeManagedResources">true to free unmanaged
        /// resources only, false to free managed resources as well</param>
        private void Dispose(bool freeManagedResources) {
            if (freeManagedResources) {
                foreach(var element in this) {
                    element.Dispose();
                }
            }
            return;
        }

    }

}