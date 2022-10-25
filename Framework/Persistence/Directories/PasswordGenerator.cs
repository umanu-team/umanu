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

namespace Framework.Persistence.Directories {

    using Framework.Model;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Generator for random passwords.
    /// </summary>
    public static class PasswordGenerator {

        /// <summary>
        /// Random number generator initialized with a random seed.
        /// </summary>
        internal static Random Random {
            get {
                if (null == PasswordGenerator.random) {
                    PasswordGenerator.random = new Random(PasswordGenerator.GenerateSeed());
                }
                return PasswordGenerator.random;
            }
        }
        private static Random random = null;

        /// <summary>
        /// Generates a random password. It is ensured that at least
        /// one upper case letter, one lower case letter, one number
        /// and optionally one special character is contained whereas
        /// the special character won't be the first or the last
        /// character.
        /// </summary>
        /// <param name="length">length of the random password</param>
        /// <returns>generated random password</returns>
        public static string GeneratePassword(ushort length) {
            return PasswordGenerator.GeneratePassword(length, PersistentUserDirectory.HasEmailAddressAsUserName);
        }

        /// <summary>
        /// Generates a random password. It is ensured that at least
        /// one upper case letter, one lower case letter, one number
        /// and optionally one special character is contained whereas
        /// the special character won't be the first or the last
        /// character.
        /// </summary>
        /// <param name="length">length of the random password</param>
        /// <param name="hasSpecialCharacters">true if generated
        /// password must contain at least one special character,
        /// false if it must not</param>
        /// <returns>generated random password</returns>
        public static string GeneratePassword(ushort length, bool hasSpecialCharacters) {
            var passwordBuilder = new StringBuilder(length);
            foreach (var characterSet in PasswordGenerator.GeneratePasswordCharacterSets(length, hasSpecialCharacters)) {
                var index = PasswordGenerator.Random.Next(0, characterSet.Length); // second value is EXCLUSIVE upper bound
                passwordBuilder.Append(characterSet[index]);
            }
            return passwordBuilder.ToString();
        }

        /// <summary>
        /// Generates a random password character sets.
        /// </summary>
        /// <param name="length">length of the random password to
        /// generate random character sets for</param>
        /// <param name="hasSpecialCharacters">true if generated
        /// password must contain at least one special character,
        /// false if it must not</param>
        /// <returns>random password character sets</returns>
        private static IEnumerable<string> GeneratePasswordCharacterSets(ushort length, bool hasSpecialCharacters) {
            if (length > 0) {
                var remainingCharacterSetsCount = length;
                const string upperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                const string lowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
                const string numbers = "0123456789";
                const string specialCharacters = "!=?,;.-"; // "+", ":" and "#" cause trouble
                string allCharacters = upperCaseLetters + lowerCaseLetters + numbers;
                if (hasSpecialCharacters) {
                    allCharacters += specialCharacters;
                }
                var mandatoryCharacterSets = new List<string>(4);
                mandatoryCharacterSets.Add(upperCaseLetters);
                mandatoryCharacterSets.Add(lowerCaseLetters);
                mandatoryCharacterSets.Add(numbers);
                if (hasSpecialCharacters) {
                    mandatoryCharacterSets.Add(specialCharacters);
                }
                while (remainingCharacterSetsCount > 0) {
                    var index = PasswordGenerator.Random.Next(0, remainingCharacterSetsCount); // second value is EXCLUSIVE upper bound
                    if (index < mandatoryCharacterSets.Count) {
                        yield return mandatoryCharacterSets[index];
                        mandatoryCharacterSets.RemoveAt(index);
                    } else {
                        yield return allCharacters;
                    }
                    remainingCharacterSetsCount--;
                }
            }
        }

        /// <summary>
        /// Generates a secure seed.
        /// </summary>
        /// <returns>generated secure seed</returns>
        private static int GenerateSeed() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            long seed1 = new Random((int)(UtcDateTime.Now.Ticks % int.MaxValue)).Next(int.MinValue, int.MaxValue);
            stopwatch.Stop();
            long seed2 = stopwatch.ElapsedTicks * Environment.TickCount;
            if (0 == seed1 % 2) {
                seed2 *= -1;
            }
            return (int)((seed1 + seed2) % int.MaxValue);
        }

    }

}