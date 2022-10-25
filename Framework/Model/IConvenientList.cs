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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// This interface extends IList&lt;TValue&gt; by further
    /// convenient methods.
    /// </summary>
    /// <typeparam name="TValue">type of values to be stored in list</typeparam>
    public interface IConvenientList<TValue> : IList<TValue> {

        /// <summary>
        /// Gets or sets the total number of elements the internal
        /// data structure can hold without resizing.
        /// </summary>
        int Capacity { get; set; }

        /// <summary>
        /// Adds a collection of objects to the list. 
        /// </summary>
        /// <param name="collection">collection of objects to add to
        /// the list</param>
        void AddRange(IEnumerable<TValue> collection);

        /// <summary>
        /// Returns a read-only wrapper for the list.
        /// </summary>
        /// <returns>read-only wrapper for list</returns>
        ReadOnlyCollection<TValue> AsReadOnly();

        /// <summary>
        /// Searches the sorted list for an item. The list must
        /// already be sorted according to the default comparer
        /// implementation; otherwise, the result is incorrect.
        /// </summary>
        /// <param name="item">item to locate - can be null for
        /// reference types</param>
        int BinarySearch(TValue item);

        /// <summary>
        /// Searches the sorted list for an item. The list must
        /// already be sorted according to the comparison
        /// implementation; otherwise, the result is incorrect.
        /// </summary>
        /// <param name="item">item to locate - can be null for
        /// reference types</param>
        /// <param name="comparison">comparison delagate to use when
        /// comparing two elements</param>
        /// <returns>zero-based index of item in the sorted list if
        /// item was found, otherwise a negative number that is the
        /// bitwise complement of the index of the next element that
        /// is larger than item or, if there is no larger element,
        /// the bitwise complement of Count</returns>
        int BinarySearch(TValue item, Comparison<TValue> comparison);

        /// <summary>
        /// Searches the sorted list for an item. The list must
        /// already be sorted according to the comparer
        /// implementation; otherwise, the result is incorrect.
        /// </summary>
        /// <param name="item">item to locate - can be null for
        /// reference types</param>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        /// <returns>zero-based index of item in the sorted list if
        /// item was found, otherwise a negative number that is the
        /// bitwise complement of the index of the next element that
        /// is larger than item or, if there is no larger element,
        /// the bitwise complement of Count</returns>
        int BinarySearch(TValue item, IComparer<TValue> comparer);

        /// <summary>
        /// Searches the sorted list for an item. The list must
        /// already be sorted according to the comparer
        /// implementation; otherwise, the result is incorrect.
        /// </summary>
        /// <param name="index">zero-based starting index of the
        /// range to search</param>
        /// <param name="count">length of the range to search</param>
        /// <param name="item">item to locate - can be null for
        /// reference types</param>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        /// <returns>zero-based index of item in the sorted list if
        /// item was found, otherwise a negative number that is the
        /// bitwise complement of the index of the next element that
        /// is larger than item or, if there is no larger element,
        /// the bitwise complement of Count</returns>
        int BinarySearch(int index, int count, TValue item, IComparer<TValue> comparer);

        /// <summary>
        /// Converts the elements of the list to another type, and
        /// returns a list containing the converted elements.
        /// </summary>
        /// <typeparam name="TOutput">type of the elements of the
        /// target array</typeparam>
        /// <param name="converter">delegate that converts each
        /// element from one type to another type</param>
        /// <returns>list containing the converted elements</returns>
        List<TOutput> ConvertAll<TOutput>(Converter<TValue, TOutput> converter);

        /// <summary>
        /// Copies the objects of the list to an array, starting at
        /// the beginning of the array.
        /// </summary>
        /// <param name="array">array to copy the list into</param>
        void CopyTo(TValue[] array);

        /// <summary>
        /// Copies a range of objects of the list to an array,
        /// starting at a particular array index. 
        /// </summary>
        /// <param name="listIndex">list index to start at</param>
        /// <param name="array">array to copy the list into</param>
        /// <param name="arrayIndex">array index to start at</param>
        /// <param name="count">number of list objects to copy</param>
        void CopyTo(int listIndex, TValue[] array, int arrayIndex, int count);

        /// <summary>
        /// Determines whether the list contains elements that match
        /// the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the elements to search for</param>
        /// <returns>true if the list contains one or more elements
        /// that match the conditions defined by the specified
        /// predicate, false otherwise</returns>
        bool Exists(Predicate<TValue> match);

        /// <summary>
        /// Finds the the first element that matches the conditions
        /// defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>first element that matches the conditions
        /// defined by the specified predicate, default value of type
        /// T otherwise</returns>
        TValue Find(Predicate<TValue> match);

        /// <summary>
        /// Finds all elements that match the conditions defined by
        /// the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>all elements that match the conditions defined
        /// by the specified predicate</returns>
        List<TValue> FindAll(Predicate<TValue> match);

        /// <summary>
        /// Finds the index of the first element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int FindIndex(Predicate<TValue> match);

        /// <summary>
        /// Finds the index of the first element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int FindIndex(int startIndex, Predicate<TValue> match);

        /// <summary>
        /// Finds the index of the first element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int FindIndex(int startIndex, int count, Predicate<TValue> match);

        /// <summary>
        /// Finds the indexes of all elements that match the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>list of zero-based indexes of all occurrences of
        /// matching objects within the entire list</returns>
        List<int> FindAllIndexes(Predicate<TValue> match);

        /// <summary>
        /// Finds the indexes of all elements that match the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>list of zero-based indexes of all occurrences of
        /// matching objects within the entire list</returns>
        List<int> FindAllIndexes(int startIndex, Predicate<TValue> match);

        /// <summary>
        /// Finds the indexes of all elements that match the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>list of zero-based indexes of all occurrences of
        /// matching objects within the entire list</returns>
        List<int> FindAllIndexes(int startIndex, int count, Predicate<TValue> match);

        /// <summary>
        /// Finds the the last element that matches the conditions
        /// defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>last element that matches the conditions
        /// defined by the specified predicate, default value of type
        /// T otherwise</returns>
        TValue FindLast(Predicate<TValue> match);

        /// <summary>
        /// Finds the index of the last element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int FindLastIndex(Predicate<TValue> match);

        /// <summary>
        /// Finds the index of the last element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int FindLastIndex(int startIndex, Predicate<TValue> match);

        /// <summary>
        /// Finds the index of the last element that matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the element to search for</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int FindLastIndex(int startIndex, int count, Predicate<TValue> match);

        /// <summary>
        /// Performs the specified action on each element of the
        /// list.
        /// </summary>
        /// <param name="action">delegate to perform on each element
        /// of the list</param>
        void ForEach(Action<TValue> action);

        /// <summary>
        /// Creates a shallow copy of a range of elements in the
        /// list.
        /// </summary>
        /// <param name="startIndex">zero-based list index at which
        /// the range starts</param>
        /// <param name="count">number of elements in the range</param>
        /// <returns>shallow copy of a range of elements in the list</returns>
        List<TValue> GetRange(int startIndex, int count);

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the first occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int IndexOf(TValue item, int startIndex);

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the first occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <returns>zero-based index of the first occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int IndexOf(TValue item, int startIndex, int count);

        /// <summary>
        /// Inserts a collection of objects into the list at a
        /// specific index.
        /// </summary>
        /// <param name="startIndex">index to insert item at</param>
        /// <param name="collection">collection of objects to insert
        /// into the list</param>
        void InsertRange(int startIndex, IEnumerable<TValue> collection);

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the last occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int LastIndexOf(TValue item);

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the last occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int LastIndexOf(TValue item, int startIndex);

        /// <summary>
        /// Searches for the specified object and returns the
        /// zero-based index of the last occurrence within the list.
        /// </summary>
        /// <param name="item">object to locate in list - can be null
        /// for reference types</param>
        /// <param name="startIndex">zero-based starting index of the
        /// search</param>
        /// <param name="count">number of elements in the section to
        /// search</param>
        /// <returns>zero-based index of the last occurrence of the
        /// object within the entire list if found, -1 otherwise</returns>
        int LastIndexOf(TValue item, int startIndex, int count);

        /// <summary>
        /// Removes all elements that match the conditions defined by
        /// the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions of the elements to remove</param>
        /// <returns>number of elements removed from list</returns>
        int RemoveAll(Predicate<TValue> match);

        /// <summary>
        /// Removes the object at a specific index from list.
        /// </summary>
        /// <param name="startIndex">index to start removing objects
        /// at</param>
        /// <param name="count">number of objects to remove from list</param>
        void RemoveRange(int startIndex, int count);

        /// <summary>
        /// Reverses the order of the elements in the list.
        /// </summary>
        void Reverse();

        /// <summary>
        /// Reverses the order of the elements in the list.
        /// </summary>
        /// <param name="index">zero-based starting index of the
        /// range to reverse</param>
        /// <param name="count">number of elements in the range to
        /// reverse</param>
        void Reverse(int index, int count);

        /// <summary>
        /// Sorts the elements in the entire list using the default
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        void Sort();

        /// <summary>
        /// Sorts the elements in the list using the default
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        void Sort(Comparison<TValue> comparison);

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        void Sort(IComparer<TValue> comparer);

        /// <summary>
        /// Sorts the elements in the list using the specified
        /// comparer. This method uses the QuickSort algorithm and
        /// performs an unstable sort, which means the order of equal
        /// elements may change.
        /// </summary>
        /// <param name="index">zero-based starting index of the
        /// range to sort</param>
        /// <param name="count">length of the range to sort</param>
        /// <param name="comparer">comparer implementation to use
        /// when comparing elements, or null to use the default
        /// comparer</param>
        void Sort(int index, int count, IComparer<TValue> comparer);

        /// <summary>
        /// Copies the elements of the list to a new array.
        /// </summary>
        /// <returns>array containing copies of the elements of the
        /// list</returns>
        TValue[] ToArray();

        /// <summary>
        /// Sets the capacity to the actual number of elements in the
        /// list, if the lists contains at least one value.
        /// </summary>
        void TrimExcess();

        /// <summary>
        /// Determines whether every element in the list matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">predicate delegate that defines the
        /// conditions</param>
        /// <returns>true if list is empty or if all elements in the
        /// list match the condition, false otherwise</returns>
        bool TrueForAll(Predicate<TValue> match);

    }

}