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

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using System;

    /// <summary>
    /// Represents an iCalendar alarm as defined in RFC5545.
    /// </summary>
    public class Alarm : PersistentObject {

        /// <summary>
        /// Repeatition count (REPEAT).
        /// Defines the number of times the alarm should be repeated,
        /// after the initial trigger.
        /// </summary>
        public int? RepetitionCount {
            get { return this.repetitionCount.Value; }
            set { this.repetitionCount.Value = value; }
        }
        private readonly PersistentFieldForNullableInt repetitionCount =
            new PersistentFieldForNullableInt(nameof(RepetitionCount), 0);

        /// <summary>
        /// Repetition duration (DURATION).
        /// Specify the delay period prior to repeating an alarm.
        /// </summary>
        public TimeSpan? RepetitionDuration {
            get {
                TimeSpan? repetitionDuration;
                if (this.repetitionDuration.Value.HasValue) {
                    repetitionDuration = new TimeSpan(this.repetitionDuration.Value.Value);
                } else {
                    repetitionDuration = null;
                }
                return repetitionDuration;
            }
            set {
                if (value.HasValue) {
                    this.repetitionDuration.Value = value.Value.Ticks;
                } else {
                    this.repetitionDuration.Value = null;
                }
            }
        }
        private readonly PersistentFieldForNullableLong repetitionDuration =
            new PersistentFieldForNullableLong(nameof(RepetitionDuration));

        /// <summary>
        /// Relation of trigger time span (TRIGGER;RELATED).
        /// Indicates whether trigger time span is related to start
        /// or to end.
        /// </summary>
        public DurationRelation TriggerRelation {
            get { return (DurationRelation)this.triggerRelation.Value; }
            set { this.triggerRelation.Value = (int)value; }
        }
        private readonly PersistentFieldForInt triggerRelation =
            new PersistentFieldForInt(nameof(TriggerRelation), (int)DurationRelation.Start);

        /// <summary>
        /// Trigger (TRIGGER).
        /// Sppecifies when the alarm will trigger, for example
        /// TimeSpan.FromMinutes(-15) for 15 minutes prior.
        /// </summary>
        public TimeSpan TriggerTimeSpan {
            get { return new TimeSpan(this.triggerTimeSpan.Value); }
            set { this.triggerTimeSpan.Value = value.Ticks; }
        }
        private readonly PersistentFieldForLong triggerTimeSpan =
            new PersistentFieldForLong(nameof(TriggerTimeSpan));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Alarm()
            : base() {
            this.RegisterPersistentField(this.repetitionCount);
            this.RegisterPersistentField(this.repetitionDuration);
            this.RegisterPersistentField(this.triggerRelation);
            this.RegisterPersistentField(this.triggerTimeSpan);
        }

    }

}