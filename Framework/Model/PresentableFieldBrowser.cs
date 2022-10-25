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

    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper class for browsing presentable fields.
    /// </summary>
    internal static class PresentableFieldBrowser {

        /// <summary>
        /// Finds all presentable fields for a specified key chain in
        /// an enumerable of presentable fields subsequently.
        /// </summary>
        /// <param name="keyChain">key chain to find presentable
        /// fields for</param>
        /// <param name="presentableFields">presentable fields to be
        /// browsed</param>
        /// <returns>all presentable fields for specified key chain
        /// or null</returns>
        public static IEnumerable<IPresentableField> FindPresentableFields(string[] keyChain, IEnumerable<IPresentableField> presentableFields) {
            if (keyChain.LongLength > 0L) {
                KeyChain.SplitKey(keyChain[0], out var key, out var index);
                IPresentableField presentableFieldForKey = null;
                foreach (var presentableField in presentableFields) {
                    if (presentableField.Key == key) {
                        presentableFieldForKey = presentableField;
                        break;
                    }
                }
                if (null != presentableFieldForKey) {
                    if (keyChain.LongLength < 2L) { // first key of chain is last key (whose index would be ignored)
                        yield return presentableFieldForKey;
                    } else if (index < 0) { // first key of chain has no index
                        if (presentableFieldForKey is IPresentableFieldForElement presentableFieldForElement) {
                            if (presentableFieldForElement.ValueAsObject is IPresentableObject childObject) {
                                foreach (var presentableField in childObject.FindPresentableFields(KeyChain.RemoveFirstLinkOf(keyChain))) {
                                    yield return presentableField;
                                }
                            }
                        } else if (presentableFieldForKey is IPresentableFieldForCollection presentableFieldForCollection) {
                            foreach (var objectValue in presentableFieldForCollection.GetValuesAsObject()) {
                                if (objectValue is IPresentableObject childObject) {
                                    foreach (var presentableField in childObject.FindPresentableFields(KeyChain.RemoveFirstLinkOf(keyChain))) {
                                        yield return presentableField;
                                    }
                                }
                            }
                        }
                    } else { // first key of chain has an index
                        if (presentableFieldForKey is IPresentableFieldForCollection presentableFieldForCollection) {
                            var i = 0;
                            foreach (var objectValue in presentableFieldForCollection.GetValuesAsObject()) {
                                if (i == index) {
                                    if (objectValue is IPresentableObject childObject) {
                                        foreach (var presentableField in childObject.FindPresentableFields(KeyChain.RemoveFirstLinkOf(keyChain))) {
                                            yield return presentableField;
                                        }
                                    }
                                    break;
                                }
                                i++;
                            }
                            if (i != index) {
                                throw new ArgumentException("Index is out of bounds in key chain at \"" + key + "\".", nameof(keyChain));
                            }
                        }
                    }
                }
            }
        }

    }

}