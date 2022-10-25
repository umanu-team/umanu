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
    using Framework.Persistence.Fields;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Provider class for options of type IUser to be pulled in
    /// fields of views.
    /// </summary>
    public class PersonOptionCollection : OptionProvider, IList<IUser> {

        /// <summary>
        /// Gets the number of objects contained.
        /// </summary>
        public int Count {
            get {
                return this.Options.Count;
            }
        }

        /// <summary>
        /// True if this object is read-only, false otherwise.
        /// </summary>
        public bool IsReadOnly {
            get { return this.IsWriteProtected; }
        }

        /// <summary>
        /// Protected list of options.
        /// </summary>
        protected PersistentFieldForIUserCollection Options { get; private set; }

        /// <summary>
        /// Gets the value at a specific index.
        /// </summary>
        /// <param name="index">index to get value for</param>
        /// <returns>value at the specific index</returns>
        public IUser this[int index] {
            get { return this.Options[index]; }
            set { this.Options[index] = value; }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PersonOptionCollection() {
            this.Options = new PersistentFieldForIUserCollection("Options");
            this.RegisterPersistentField(this.Options);
        }

        /// <summary>
        /// Adds an object to the list. 
        /// </summary>
        /// <param name="item">object to add to the list</param>
        public void Add(IUser item) {
            this.Options.Add(item);
            return;
        }

        /// <summary>
        /// Adds a range of pairs of key and value to the list.
        /// </summary>
        /// <param name="items">collections of pairs of key and value</param>
        public void AddRange(IEnumerable<IUser> items) {
            this.Options.AddRange(items);
            return;
        }

        /// <summary>
        /// Removes all objects from the list.
        /// </summary>
        public virtual void Clear() {
            this.Options.Clear();
            return;
        }

        /// <summary>
        /// Determines whether the list contains a specific object.
        /// </summary>
        /// <param name="item">object to locate in list - can be null</param>
        /// <returns>true if specific object is contained, false
        /// otherwise</returns>
        public virtual bool Contains(IUser item) {
            return this.Options.Contains(item);
        }

        /// <summary>
        /// Copies the objects of the list to an array, starting at a
        /// particular array index. 
        /// </summary>
        /// <param name="array">array to copy the list into</param>
        /// <param name="arrayIndex">array index to start at</param>
        public virtual void CopyTo(IUser[] array, int arrayIndex) {
            this.Options.CopyTo(array, arrayIndex);
            return;
        }

        /// <summary>
        /// Gets all options of this option prvider.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>all options of this option prvider</returns>
        public override IEnumerable<KeyValuePair<string, string>> GetOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            foreach (var user in this.Options) {
                if (null != user) {
                    yield return new KeyValuePair<string, string>(user.UserName, user.DisplayName + " (" + user.UserName + ")");
                }
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the first occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        public virtual int IndexOf(IUser item) {
            return this.Options.IndexOf(item);
        }

        /// <summary>
        /// Inserts an object into the list at a specific index.
        /// </summary>
        /// <param name="index">index to insert item at</param>
        /// <param name="item">object to insert into the list</param>
        public virtual void Insert(int index, IUser item) {
            this.Options.Insert(index, item);
            return;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>enumerator that iterates through the list</returns>
        public virtual IEnumerator<IUser> GetEnumerator() {
            return this.Options.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>enumerator that iterates through the list</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            foreach(var option in this.Options) {
                yield return option;
            }
        }

        /// <summary>
        /// Removes a specific object from the list.
        /// </summary>
        /// <param name="item">specific object to remove from list</param>
        /// <returns>true if object was successfully removed from
        /// list, false otherwise or if object was not contained in
        /// list</returns>
        public virtual bool Remove(IUser item) {
            return this.Options.Remove(item);
        }

        /// <summary>
        /// Removes the object at a specific index from list.
        /// </summary>
        /// <param name="index">specific index to remove object at</param>
        public virtual void RemoveAt(int index) {
            this.Options.RemoveAt(index);
            return;
        }

        /// <summary>
        /// Sorts the list by a given field.
        /// </summary>
        /// <param name="fieldKey">key of field to order list by</param>
        public virtual void Sort(string fieldKey) {
            this.Options.Sort(fieldKey);
            return;
        }

    }

}