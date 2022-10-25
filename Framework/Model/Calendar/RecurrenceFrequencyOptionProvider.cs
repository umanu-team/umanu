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

namespace Framework.Model.Calendar {

    using Framework.Persistence.Fields;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Properties;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Option provider for recurrence frequencies.
    /// </summary>
    public sealed class RecurrenceFrequencyOptionProvider : OptionProvider {

        /// <summary>
        /// Minumum recurrence frequency.
        /// </summary>
        public RecurrenceFrequency MinimumRecurrenceFrequency {
            get { return (RecurrenceFrequency)this.minimumRecurrenceFrequency.Value; }
            set { this.minimumRecurrenceFrequency.Value = (int)value; }
        }
        private readonly PersistentFieldForInt minimumRecurrenceFrequency =
            new PersistentFieldForInt(nameof(MinimumRecurrenceFrequency));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public RecurrenceFrequencyOptionProvider()
            : base() {
            this.RegisterPersistentField(this.minimumRecurrenceFrequency);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="minimumRecurrenceFrequency">minumum
        /// recurrence frequency</param>
        public RecurrenceFrequencyOptionProvider(RecurrenceFrequency minimumRecurrenceFrequency)
            : this() {
            this.MinimumRecurrenceFrequency = minimumRecurrenceFrequency;
        }

        /// <summary>
        /// Gets all options of this option provider (IDs of
        /// objects/users need to be set as keys).
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost presentable
        /// object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>all options of this option provider (IDs of
        /// objects/users need to be set as keys)</returns>
        public override IEnumerable<KeyValuePair<string, string>> GetOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            if (this.MinimumRecurrenceFrequency <= RecurrenceFrequency.Secondly) {
                yield return new KeyValuePair<string, string>(((int)RecurrenceFrequency.Secondly).ToString(CultureInfo.InvariantCulture.NumberFormat), Resources.secondly);
            }
            if (this.MinimumRecurrenceFrequency <= RecurrenceFrequency.Minutely) {
                yield return new KeyValuePair<string, string>(((int)RecurrenceFrequency.Minutely).ToString(CultureInfo.InvariantCulture.NumberFormat), Resources.minutely);
            }
            if (this.MinimumRecurrenceFrequency <= RecurrenceFrequency.Hourly) {
                yield return new KeyValuePair<string, string>(((int)RecurrenceFrequency.Hourly).ToString(CultureInfo.InvariantCulture.NumberFormat), Resources.hourly);
            }
            if (this.MinimumRecurrenceFrequency <= RecurrenceFrequency.Daily) {
                yield return new KeyValuePair<string, string>(((int)RecurrenceFrequency.Daily).ToString(CultureInfo.InvariantCulture.NumberFormat), Resources.daily);
            }
            if (this.MinimumRecurrenceFrequency <= RecurrenceFrequency.Weekly) {
                yield return new KeyValuePair<string, string>(((int)RecurrenceFrequency.Weekly).ToString(CultureInfo.InvariantCulture.NumberFormat), Resources.weekly);
            }
            if (this.MinimumRecurrenceFrequency <= RecurrenceFrequency.Monthly) {
                yield return new KeyValuePair<string, string>(((int)RecurrenceFrequency.Monthly).ToString(CultureInfo.InvariantCulture.NumberFormat), Resources.monthly);
            }
            yield return new KeyValuePair<string, string>(((int)RecurrenceFrequency.Yearly).ToString(CultureInfo.InvariantCulture.NumberFormat), Resources.yearly);
        }

    }

}