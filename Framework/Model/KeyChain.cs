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

    /// <summary>
    /// Helper class for conversion of keys and key chains.
    /// </summary>
    public static class KeyChain {

        /// <summary>
        /// Concatenates two keys into a key chain.
        /// </summary>
        /// <param name="key1">first key to be concatenated</param>
        /// <param name="key2">second key to be concatenated</param>
        /// <returns>concatenated key chain</returns>
        public static string[] Concat(string key1, string key2) {
            return new string[] { key1, key2 };
        }

        /// <summary>
        /// Concatenates a key chain and a key into a key chain.
        /// </summary>
        /// <param name="keyChain">key chain to be concatenated</param>
        /// <param name="key">key to be concatenated</param>
        /// <returns>concatenated key chain</returns>
        public static string[] Concat(string[] keyChain, string key) {
            var keyChainLength = keyChain.LongLength;
            var concatedKeyChain = new string[keyChainLength + 1];
            Array.Copy(keyChain, concatedKeyChain, keyChainLength);
            concatedKeyChain[keyChainLength] = key;
            return concatedKeyChain;
        }

        /// <summary>
        /// Concatenates a key and a key chain into a key chain.
        /// </summary>
        /// <param name="key">key to be concatenated</param>
        /// <param name="keyChain">key chain to be concatenated</param>
        /// <returns>concatenated key chain</returns>
        public static string[] Concat(string key, string[] keyChain) {
            var keyChainLength = keyChain.LongLength;
            var concatedKeyChain = new string[keyChainLength + 1];
            concatedKeyChain[0] = key;
            Array.Copy(keyChain, 0L, concatedKeyChain, 1L, keyChainLength);
            return concatedKeyChain;
        }

        /// <summary>
        /// Concatenates two key chains.
        /// </summary>
        /// <param name="keyChain1">first key chain to be
        /// concatenated</param>
        /// <param name="keyChain2">second key chain to be
        /// concatenated</param>
        /// <returns>concatenated key chains</returns>
        public static string[] Concat(string[] keyChain1, string[] keyChain2) {
            var keyChain = new string[keyChain1.LongLength + keyChain2.LongLength];
            Array.Copy(keyChain1, keyChain, keyChain1.LongLength);
            Array.Copy(keyChain2, 0L, keyChain, keyChain1.LongLength, keyChain2.LongLength);
            return keyChain;
        }

        /// <summary>
        /// Concatenates two keys into a key.
        /// </summary>
        /// <param name="key1">first key to be concatenated</param>
        /// <param name="key2">second key to be concatenated</param>
        /// <returns>concatenated key</returns>
        internal static string ConcatToKey(string key1, string key2) {
            return key1 + '.' + key2;
        }

        /// <summary>
        /// Converts a key to a key cahin.
        /// </summary>
        /// <param name="key">key to convert to key chain</param>
        /// <returns>key chain converted from key</returns>
        public static string[] FromKey(string key) {
            string[] keyChain;
            if (string.IsNullOrEmpty(key)) {
                keyChain = new string[0];
            } else {
                keyChain = key.Split(new char[] { '.' }, StringSplitOptions.None);
            }
            return keyChain;
        }

        /// <summary>
        /// Indicates whether a key chain is contained in an
        /// enumerable of key chains.
        /// </summary>
        /// <param name="keyChains">enumerable of key chains to
        /// browse</param>
        /// <param name="keyChain">key chain to be found</param>
        /// <returns>true if key chain is contained in enumerable of
        /// key chains, false otherwise</returns>
        public static bool IsContainedIn(IEnumerable<string[]> keyChains, string[] keyChain) {
            bool isContained = false;
            foreach (var currentKeyChain in keyChains) {
                if (currentKeyChain.LongLength == keyChain.LongLength) {
                    isContained = true;
                    for (long i = 0; i < keyChain.LongLength; i++) {
                        if (currentKeyChain[i] != keyChain[i]) {
                            isContained = false;
                            break;
                        }
                    }
                    if (isContained) {
                        break;
                    }
                }
            }
            return isContained;
        }

        /// <summary>
        /// Removes the first link of a key chain.
        /// </summary>
        /// <param name="keyChain">key chain to remove first link
        /// from</param>
        /// <returns>shortened key chain</returns>
        public static string[] RemoveFirstLinkOf(string[] keyChain) {
            return KeyChain.RemoveLeadingLinksOf(keyChain, 1L);
        }

        /// <summary>
        /// Removes all indexes from a key.
        /// </summary>
        /// <param name="key">key to remove indexes from</param>
        /// <returns>key without indexes</returns>
        public static string RemoveIndexesFrom(string key) {
            var keyChain = KeyChain.FromKey(key);
            KeyChain.RemoveIndexesFrom(keyChain);
            return KeyChain.ToKey(keyChain);
        }

        /// <summary>
        /// Removes all indexes from a key chain.
        /// </summary>
        /// <param name="keyChain">key chain to remove indexes from</param>
        public static void RemoveIndexesFrom(string[] keyChain) {
            for (long i = 0; i < keyChain.LongLength; i++) {
                var link = keyChain[i];
                var underscoreIndex = link.IndexOf('_');
                if (underscoreIndex > -1) {
                    keyChain[i] = link.Substring(0, underscoreIndex);
                }
            }
            return;
        }

        /// <summary>
        /// Removes the last link of a key chain.
        /// </summary>
        /// <param name="keyChain">key chain to remove last link
        /// from</param>
        /// <returns>shortened key chain</returns>
        public static string[] RemoveLastLinkOf(string[] keyChain) {
            return KeyChain.RemoveTrailingLinksOf(keyChain, 1L);
        }

        /// <summary>
        /// Removes a specified number of links from beginning of a
        /// key chain.
        /// </summary>
        /// <param name="keyChain">key chain to remove specified
        /// number of links from</param>
        /// <param name="numberOfLinksToBeRemoved">number of links to
        /// be removed</param>
        /// <returns>shortened key chain</returns>
        public static string[] RemoveLeadingLinksOf(string[] keyChain, long numberOfLinksToBeRemoved) {
            long shortenedKeyChainLength = keyChain.LongLength - numberOfLinksToBeRemoved;
            var shortenedKeyChain = new string[shortenedKeyChainLength];
            Array.Copy(keyChain, numberOfLinksToBeRemoved, shortenedKeyChain, 0L, shortenedKeyChainLength);
            return shortenedKeyChain;
        }

        /// <summary>
        /// Removes a specified number of links from ending of a key
        /// chain.
        /// </summary>
        /// <param name="keyChain">key chain to remove specified
        /// number of links from</param>
        /// <param name="numberOfLinksToBeRemoved">number of links to
        /// be removed</param>
        /// <returns>shortened key chain</returns>
        public static string[] RemoveTrailingLinksOf(string[] keyChain, long numberOfLinksToBeRemoved) {
            long shortenedKeyChainLength = keyChain.LongLength - numberOfLinksToBeRemoved;
            var shortenedKeyChain = new string[shortenedKeyChainLength];
            Array.Copy(keyChain, 0L, shortenedKeyChain, 0L, shortenedKeyChainLength);
            return shortenedKeyChain;
        }

        /// <summary>
        /// Splits a key with index in key part and index.
        /// </summary>
        /// <param name="key">key with index</param>
        /// <param name="keyPart">key part of key</param>
        /// <param name="index">index of key</param>
        public static void SplitKey(string key, out string keyPart, out int index) {
            if (key.EndsWith("]", StringComparison.InvariantCulture)) {
                int indexOfSquaredBracketOpen = key.LastIndexOf('[');
                if (indexOfSquaredBracketOpen < 0) {
                    throw new ArgumentException("Syntax error in key chain at \"" + key + "\".", nameof(key));
                } else {
                    keyPart = key.Substring(0, indexOfSquaredBracketOpen);
                    if (!int.TryParse(key.Substring(indexOfSquaredBracketOpen + 1, key.Length - indexOfSquaredBracketOpen - 2), out index)) {
                        throw new ArgumentException("Syntax error in key chain at \"" + key + "\".", nameof(key));
                    }
                }
            } else {
                keyPart = key;
                index = -1;
            }
            return;
        }

        /// <summary>
        /// Indicates whether a long key chain starts with a specific
        /// shorter key chain.
        /// </summary>
        /// <param name="longKeyChain">long key chain to find short
        /// key chain at beginning in</param>
        /// <param name="shortKeyChain">short key chain to check
        /// existence at beginning of long key chain of</param>
        /// <returns>true if long key chain starts with short key
        /// chain, false otherwise</returns>
        public static bool StartsWith(string[] longKeyChain, string[] shortKeyChain) {
            var isLongKeyChainStartingWithShortKeyChain = shortKeyChain.LongLength <= longKeyChain.LongLength;
            if (isLongKeyChainStartingWithShortKeyChain) {
                for (long i = 0L; i < shortKeyChain.LongLength; i++) {
                    if (shortKeyChain[i] != longKeyChain[i]) {
                        isLongKeyChainStartingWithShortKeyChain = false;
                        break;
                    }
                }
            }
            return isLongKeyChainStartingWithShortKeyChain;
        }

        /// <summary>
        /// Converts a key chain to a key.
        /// </summary>
        /// <param name="keyChain">key chain to convert to key</param>
        /// <returns>key converted from key chain</returns>
        public static string ToKey(IEnumerable<string> keyChain) {
            return string.Join(".", keyChain);
        }

    }

}