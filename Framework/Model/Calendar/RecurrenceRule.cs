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

/*
      RFC 5545, 3.3.10.:

      This value type is a structured value consisting of a
      list of one or more recurrence grammar parts.  Each rule part is
      defined by a NAME=VALUE pair.  The rule parts are separated from
      each other by the SEMICOLON character.  The rule parts are not
      ordered in any particular sequence.  Individual rule parts MUST
      only be specified once.  Compliant applications MUST accept rule
      parts ordered in any sequence, but to ensure backward
      compatibility with applications that pre-date this revision of
      iCalendar the FREQ rule part MUST be the first rule part specified
      in a RECUR value.

      The FREQ rule part identifies the type of recurrence rule.  This
      rule part MUST be specified in the recurrence rule.  Valid values
      include SECONDLY, to specify repeating events based on an interval
      of a second or more; MINUTELY, to specify repeating events based
      on an interval of a minute or more; HOURLY, to specify repeating
      events based on an interval of an hour or more; DAILY, to specify
      repeating events based on an interval of a day or more; WEEKLY, to
      specify repeating events based on an interval of a week or more;
      MONTHLY, to specify repeating events based on an interval of a
      month or more; and YEARLY, to specify repeating events based on an
      interval of a year or more.

      The INTERVAL rule part contains a positive integer representing at
      which intervals the recurrence rule repeats.  The default value is
      "1", meaning every second for a SECONDLY rule, every minute for a
      MINUTELY rule, every hour for an HOURLY rule, every day for a
      DAILY rule, every week for a WEEKLY rule, every month for a
      MONTHLY rule, and every year for a YEARLY rule.  For example,
      within a DAILY rule, a value of "8" means every eight days.

      The UNTIL rule part defines a DATE or DATE-TIME value that bounds
      the recurrence rule in an inclusive manner.  If the value
      specified by UNTIL is synchronized with the specified recurrence,
      this DATE or DATE-TIME becomes the last instance of the
      recurrence.  The value of the UNTIL rule part MUST have the same
      value type as the "DTSTART" property.  Furthermore, if the
      "DTSTART" property is specified as a date with local time, then
      the UNTIL rule part MUST also be specified as a date with local
      time.  If the "DTSTART" property is specified as a date with UTC
      time or a date with local time and time zone reference, then the
      UNTIL rule part MUST be specified as a date with UTC time.  In the
      case of the "STANDARD" and "DAYLIGHT" sub-components the UNTIL
      rule part MUST always be specified as a date with UTC time.  If
      specified as a DATE-TIME value, then it MUST be specified in a UTC
      time format.  If not present, and the COUNT rule part is also not
      present, the "RRULE" is considered to repeat forever.

      The COUNT rule part defines the number of occurrences at which to
      range-bound the recurrence.  The "DTSTART" property value always
      counts as the first occurrence.

      The BYSECOND rule part specifies a COMMA-separated list of seconds
      within a minute.  Valid values are 0 to 60.  The BYMINUTE rule
      part specifies a COMMA-separated list of minutes within an hour.
      Valid values are 0 to 59.  The BYHOUR rule part specifies a COMMA-
      separated list of hours of the day.  Valid values are 0 to 23.
      The BYSECOND, BYMINUTE and BYHOUR rule parts MUST NOT be specified
      when the associated "DTSTART" property has a DATE value type.
      These rule parts MUST be ignored in RECUR value that violate the
      above requirement (e.g., generated by applications that pre-date
      this revision of iCalendar).

      The BYDAY rule part specifies a COMMA-separated list of days of
      the week; SU indicates Sunday; MO indicates Monday; TU indicates
      Tuesday; WE indicates Wednesday; TH indicates Thursday; FR
      indicates Friday; and SA indicates Saturday.

      Each BYDAY value can also be preceded by a positive (+n) or
      negative (-n) integer.  If present, this indicates the nth
      occurrence of a specific day within the MONTHLY or YEARLY "RRULE".

      For example, within a MONTHLY rule, +1MO (or simply 1MO)
      represents the first Monday within the month, whereas -1MO
      represents the last Monday of the month.  The numeric value in a
      BYDAY rule part with the FREQ rule part set to YEARLY corresponds
      to an offset within the month when the BYMONTH rule part is
      present, and corresponds to an offset within the year when the
      BYWEEKNO or BYMONTH rule parts are present.  If an integer
      modifier is not present, it means all days of this type within the
      specified frequency.  For example, within a MONTHLY rule, MO
      represents all Mondays within the month.  The BYDAY rule part MUST
      NOT be specified with a numeric value when the FREQ rule part is
      not set to MONTHLY or YEARLY.  Furthermore, the BYDAY rule part
      MUST NOT be specified with a numeric value with the FREQ rule part
      set to YEARLY when the BYWEEKNO rule part is specified.

      The BYMONTHDAY rule part specifies a COMMA-separated list of days
      of the month.  Valid values are 1 to 31 or -31 to -1.  For
      example, -10 represents the tenth to the last day of the month.
      The BYMONTHDAY rule part MUST NOT be specified when the FREQ rule
      part is set to WEEKLY.

      The BYYEARDAY rule part specifies a COMMA-separated list of days
      of the year.  Valid values are 1 to 366 or -366 to -1.  For
      example, -1 represents the last day of the year (December 31st)
      and -306 represents the 306th to the last day of the year (March
      1st).  The BYYEARDAY rule part MUST NOT be specified when the FREQ
      rule part is set to DAILY, WEEKLY, or MONTHLY.

      The BYWEEKNO rule part specifies a COMMA-separated list of
      ordinals specifying weeks of the year.  Valid values are 1 to 53
      or -53 to -1.  This corresponds to weeks according to week
      numbering as defined in [ISO.8601.2004].  A week is defined as a
      seven day period, starting on the day of the week defined to be
      the week start (see WKST).  Week number one of the calendar year
      is the first week that contains at least four (4) days in that
      calendar year.  This rule part MUST NOT be used when the FREQ rule
      part is set to anything other than YEARLY.  For example, 3
      represents the third week of the year.

         Note: Assuming a Monday week start, week 53 can only occur when
         Thursday is January 1 or if it is a leap year and Wednesday is
         January 1.

      The BYMONTH rule part specifies a COMMA-separated list of months
      of the year.  Valid values are 1 to 12.

      The WKST rule part specifies the day on which the workweek starts.
      Valid values are MO, TU, WE, TH, FR, SA, and SU.  This is
      significant when a WEEKLY "RRULE" has an interval greater than 1,
      and a BYDAY rule part is specified.  This is also significant when
      in a YEARLY "RRULE" when a BYWEEKNO rule part is specified.  The
      default value is MO.

      The BYSETPOS rule part specifies a COMMA-separated list of values
      that corresponds to the nth occurrence within the set of
      recurrence instances specified by the rule.  BYSETPOS operates on
      a set of recurrence instances in one interval of the recurrence
      rule.  For example, in a WEEKLY rule, the interval would be one
      week A set of recurrence instances starts at the beginning of the
      interval defined by the FREQ rule part.  Valid values are 1 to 366
      or -366 to -1.  It MUST only be used in conjunction with another
      BYxxx rule part.  For example "the last work day of the month"
      could be represented as:

       FREQ=MONTHLY;BYDAY=MO,TU,WE,TH,FR;BYSETPOS=-1

      Each BYSETPOS value can include a positive (+n) or negative (-n)
      integer.  If present, this indicates the nth occurrence of the
      specific occurrence within the set of occurrences specified by the
      rule.

      Recurrence rules may generate recurrence instances with an invalid
      date (e.g., February 30) or nonexistent local time (e.g., 1:30 AM
      on a day where the local time is moved forward by an hour at 1:00
      AM).  Such recurrence instances MUST be ignored and MUST NOT be
      counted as part of the recurrence set.

      Information, not contained in the rule, necessary to determine the
      various recurrence instance start time and dates are derived from
      the Start Time ("DTSTART") component attribute.  For example,
      "FREQ=YEARLY;BYMONTH=1" doesn't specify a specific day within the
      month or a time.  This information would be the same as what is
      specified for "DTSTART".

      BYxxx rule parts modify the recurrence in some manner.  BYxxx rule
      parts for a period of time that is the same or greater than the
      frequency generally reduce or limit the number of occurrences of
      the recurrence generated.  For example, "FREQ=DAILY;BYMONTH=1"
      reduces the number of recurrence instances from all days (if
      BYMONTH rule part is not present) to all days in January.  BYxxx
      rule parts for a period of time less than the frequency generally
      increase or expand the number of occurrences of the recurrence.
      For example, "FREQ=YEARLY;BYMONTH=1,2" increases the number of
      days within the yearly recurrence set from 1 (if BYMONTH rule part
      is not present) to 2.

      If multiple BYxxx rule parts are specified, then after evaluating
      the specified FREQ and INTERVAL rule parts, the BYxxx rule parts
      are applied to the current set of evaluated occurrences in the
      following order: BYMONTH, BYWEEKNO, BYYEARDAY, BYMONTHDAY, BYDAY,
      BYHOUR, BYMINUTE, BYSECOND and BYSETPOS; then COUNT and UNTIL are
      evaluated.

      The table below summarizes the dependency of BYxxx rule part
      expand or limit behavior on the FREQ rule part value.

      The term "N/A" means that the corresponding BYxxx rule part MUST
      NOT be used with the corresponding FREQ value.

      BYDAY has some special behavior depending on the FREQ value and
      this is described in separate notes below the table.

   +----------+--------+--------+-------+-------+------+-------+------+
   |          |SECONDLY|MINUTELY|HOURLY |DAILY  |WEEKLY|MONTHLY|YEARLY|
   +----------+--------+--------+-------+-------+------+-------+------+
   |BYMONTH   |Limit   |Limit   |Limit  |Limit  |Limit |Limit  |Expand|
   +----------+--------+--------+-------+-------+------+-------+------+
   |BYWEEKNO  |N/A     |N/A     |N/A    |N/A    |N/A   |N/A    |Expand|
   +----------+--------+--------+-------+-------+------+-------+------+
   |BYYEARDAY |Limit   |Limit   |Limit  |N/A    |N/A   |N/A    |Expand|
   +----------+--------+--------+-------+-------+------+-------+------+
   |BYMONTHDAY|Limit   |Limit   |Limit  |Limit  |N/A   |Expand |Expand|
   +----------+--------+--------+-------+-------+------+-------+------+
   |BYDAY     |Limit   |Limit   |Limit  |Limit  |Expand|Note 1 |Note 2|
   +----------+--------+--------+-------+-------+------+-------+------+
   |BYHOUR    |Limit   |Limit   |Limit  |Expand |Expand|Expand |Expand|
   +----------+--------+--------+-------+-------+------+-------+------+
   |BYMINUTE  |Limit   |Limit   |Expand |Expand |Expand|Expand |Expand|
   +----------+--------+--------+-------+-------+------+-------+------+
   |BYSECOND  |Limit   |Expand  |Expand |Expand |Expand|Expand |Expand|
   +----------+--------+--------+-------+-------+------+-------+------+
   |BYSETPOS  |Limit   |Limit   |Limit  |Limit  |Limit |Limit  |Limit |
   +----------+--------+--------+-------+-------+------+-------+------+

      Note 1:  Limit if BYMONTHDAY is present; otherwise, special expand
               for MONTHLY.

      Note 2:  Limit if BYYEARDAY or BYMONTHDAY is present; otherwise,
               special expand for WEEKLY if BYWEEKNO present; otherwise,
               special expand for MONTHLY if BYMONTH present; otherwise,
               special expand for YEARLY.

      Here is an example of evaluating multiple BYxxx rule parts.

       DTSTART;TZID=America/New_York:19970105T083000
       RRULE:FREQ=YEARLY;INTERVAL=2;BYMONTH=1;BYDAY=SU;BYHOUR=8,9;
        BYMINUTE=30

      First, the "INTERVAL=2" would be applied to "FREQ=YEARLY" to
      arrive at "every other year".  Then, "BYMONTH=1" would be applied
      to arrive at "every January, every other year".  Then, "BYDAY=SU"
      would be applied to arrive at "every Sunday in January, every
      other year".  Then, "BYHOUR=8,9" would be applied to arrive at
      "every Sunday in January at 8 AM and 9 AM, every other year".
      Then, "BYMINUTE=30" would be applied to arrive at "every Sunday in
      January at 8:30 AM and 9:30 AM, every other year".  Then, lacking
      information from "RRULE", the second is derived from "DTSTART", to
      end up in "every Sunday in January at 8:30:00 AM and 9:30:00 AM,
      every other year".  Similarly, if the BYMINUTE, BYHOUR, BYDAY,
      BYMONTHDAY, or BYMONTH rule part were missing, the appropriate
      minute, hour, day, or month would have been retrieved from the
      "DTSTART" property.

      (...)

   Example:  The following is a rule that specifies 10 occurrences that
      occur every other day:

       FREQ=DAILY;COUNT=10;INTERVAL=2

 */

namespace Framework.Model.Calendar {

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Definedes recurrence rule specifications.
    /// BYSETPOS is not supported.
    /// </summary>
    public class RecurrenceRule : PersistentObject {

        /// <summary>
        /// The BYDAY rule part specifies a COMMA-separated list of
        /// days of the week. Each BYDAY value can also be preceded
        /// by a positive (+n) or negative (-n) integer.  If present,
        /// this indicates the nth occurrence of a specific day
        /// within the MONTHLY or YEARLY "RRULE".
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<ByDay> ByDay { get; private set; }

        /// <summary>
        /// Specifies a COMMA-separated list of hours that
        /// corresponds to the nth occurrence within the set of
        /// recurrence instances specified by the rule.
        /// </summary>
        public PersistentFieldForByteCollection ByHour { get; private set; }

        /// <summary>
        /// Specifies a COMMA-separated list of minutes that
        /// corresponds to the nth occurrence within the set of
        /// recurrence instances specified by the rule.
        /// </summary>
        public PersistentFieldForByteCollection ByMinute { get; private set; }

        /// <summary>
        /// Specifies a COMMA-separated list of months that
        /// corresponds to the nth occurrence within the set of
        /// recurrence instances specified by the rule (1 to 12).
        /// </summary>
        public PersistentFieldForByteCollection ByMonth { get; private set; }

        /// <summary>
        /// Specifies a COMMA-separated list of days
        /// of the month.  Valid values are 1 to 31 or -31 to -1.  For
        /// example, -10 represents the tenth to the last day of the month.
        /// The BYMONTHDAY rule part MUST NOT be specified when the FREQ rule
        /// part is set to WEEKLY.
        /// </summary>
        public PersistentFieldForIntCollection ByMonthDay { get; private set; }

        /// <summary>
        /// Specifies a COMMA-separated list of seconds that
        /// corresponds to the nth occurrence within the set of
        /// recurrence instances specified by the rule.
        /// </summary>
        public PersistentFieldForByteCollection BySecond { get; private set; }

        /// <summary>
        /// Specifies a COMMA-separated list of values that
        /// corresponds to the nth occurrence within the set of
        /// recurrence instances specified by the rule.
        /// </summary>
        public PersistentFieldForShortCollection BySetPos { get; private set; }

        /// <summary>
        /// Specifies a COMMA-separated list of
        /// ordinals specifying weeks of the year.  Valid values are 1 to 53
        /// or -53 to -1.  This corresponds to weeks according to week
        /// numbering as defined in [ISO.8601.2004].  A week is defined as a
        /// seven day period, starting on the day of the week defined to be
        /// the week start (see WKST).  Week number one of the calendar year
        /// is the first week that contains at least four (4) days in that
        /// calendar year.  This rule part MUST NOT be used when the FREQ rule
        /// part is set to anything other than YEARLY.  For example, 3
        /// represents the third week of the year.
        /// </summary>
        public PersistentFieldForSByteCollection ByWeekNumber { get; private set; }

        /// <summary>
        /// Sspecifies a COMMA-separated list of days
        /// of the year.  Valid values are 1 to 366 or -366 to -1.  For
        /// example, -1 represents the last day of the year (December 31st)
        /// and -306 represents the 306th to the last day of the year (March
        /// 1st).  The BYYEARDAY rule part MUST NOT be specified when the FREQ
        /// rule part is set to DAILY, WEEKLY, or MONTHLY.
        /// </summary>
        public PersistentFieldForIntCollection ByYearDay { get; private set; }

        /// <summary>
        /// The COUNT rule part defines the number of occurrences at
        /// which to range-bound the recurrence.The "DTSTART"
        /// property value always counts as the first occurrence. A
        /// COUNT of 0 leads to infinite occurrences if no end
        /// date/time is set.
        /// </summary>
        public uint? Count {
            get { return this.count.Value; }
            set { this.count.Value = value; }
        }
        private readonly PersistentFieldForNullableUInt count =
            new PersistentFieldForNullableUInt(nameof(Count));

        /// <summary>
        /// Defines the date and time by which the recurrence ends.
        /// </summary>
        public DateTime? EndDateTime {
            get { return this.endDateTime.Value; }
            set { this.endDateTime.Value = value; }
        }
        private readonly PersistentFieldForNullableDateTime endDateTime =
            new PersistentFieldForNullableDateTime(nameof(EndDateTime));

        /// <summary>
        /// Frequency of recurrence.
        /// </summary>
        public RecurrenceFrequency? Frequency {
            get { return (RecurrenceFrequency?)this.frequency.Value; }
            set { this.frequency.Value = (int?)value; }
        }
        private readonly PersistentFieldForNullableInt frequency =
            new PersistentFieldForNullableInt(nameof(Frequency));

        /// <summary>
        /// Indicates whether recurrence rule has valid values.
        /// </summary>
        public bool HasValue {
            get {
                return this.Frequency.HasValue && this.Interval > 0;
            }
        }

        /// <summary>
        /// Interval at which the recurrence rule repeats (e.g. every
        /// 2nd week).
        /// </summary>
        public uint Interval {
            get { return this.interval.Value; }
            set {
                if (value < 1) {
                    throw new ArgumentException(nameof(Interval) + " must be > 0.");
                }
                this.interval.Value = value;
            }
        }
        private readonly PersistentFieldForUInt interval =
            new PersistentFieldForUInt(nameof(Interval), 1);

        /// <summary>
        /// Specifies the day on which the workweek starts.
        /// </summary>
        public DayOfWeek WeekStart {
            get { return (DayOfWeek)this.weekStart.Value; }
            set { this.weekStart.Value = (int)value; }
        }
        private readonly PersistentFieldForInt weekStart =
            new PersistentFieldForInt(nameof(WeekStart), (int)DayOfWeek.Monday);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public RecurrenceRule()
            : base() {
            this.ByDay = new PersistentFieldForPersistentObjectCollection<ByDay>(nameof(ByDay), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.ByDay);
            this.ByHour = new PersistentFieldForByteCollection(nameof(ByHour));
            this.RegisterPersistentField(this.ByHour);
            this.ByMinute = new PersistentFieldForByteCollection(nameof(ByMinute));
            this.RegisterPersistentField(this.ByMinute);
            this.ByMonth = new PersistentFieldForByteCollection(nameof(ByMonth));
            this.RegisterPersistentField(this.ByMonth);
            this.ByMonthDay = new PersistentFieldForIntCollection(nameof(ByMonthDay));
            this.RegisterPersistentField(this.ByMonthDay);
            this.BySecond = new PersistentFieldForByteCollection(nameof(BySecond));
            this.RegisterPersistentField(this.BySecond);
            this.BySetPos = new PersistentFieldForShortCollection(nameof(BySetPos));
            this.RegisterPersistentField(this.BySetPos);
            this.ByWeekNumber = new PersistentFieldForSByteCollection(nameof(ByWeekNumber));
            this.RegisterPersistentField(this.ByWeekNumber);
            this.ByYearDay = new PersistentFieldForIntCollection(nameof(ByYearDay));
            this.RegisterPersistentField(this.ByYearDay);
            this.RegisterPersistentField(this.count);
            this.RegisterPersistentField(this.endDateTime);
            this.RegisterPersistentField(this.frequency);
            this.RegisterPersistentField(this.interval);
            this.RegisterPersistentField(this.weekStart);
        }

        /// <summary>
        /// Expands a date/time value by day.
        /// </summary>
        /// <param name="dateTime">date/time to expand</param>
        /// <returns>expanded date/times</returns>
        private IEnumerable<DateTime> ExpandByDay(DateTime dateTime) {
            if (this.ByDay.Count < 1) {
                yield return dateTime;
            } else if (RecurrenceFrequency.Weekly == this.Frequency || (RecurrenceFrequency.Yearly == this.Frequency && this.ByWeekNumber.Count > 0)) {
                DateTime firstDateTimeOfWeek = dateTime;
                while (firstDateTimeOfWeek.DayOfWeek != this.WeekStart) {
                    firstDateTimeOfWeek = firstDateTimeOfWeek.AddDays(-1);
                }
                const double totalDaysOfWeek = 7;
                for (double d = 0; d < totalDaysOfWeek; d++) {
                    DateTime dateTimeOfWeek = firstDateTimeOfWeek.AddDays(d);
                    foreach (ByDay byDay in this.ByDay) {
                        if (byDay.MatchesForWeek(dateTimeOfWeek)) {
                            yield return dateTimeOfWeek;
                            break;
                        }
                    }
                }
            } else if (RecurrenceFrequency.Monthly == this.Frequency || (RecurrenceFrequency.Yearly == this.Frequency && this.ByMonth.Count > 0)) {
                var firstDateTimeOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
                int totalDaysOfMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                for (double d = 0; d < totalDaysOfMonth; d++) {
                    DateTime dateTimeOfMonth = firstDateTimeOfMonth.AddDays(d);
                    foreach (ByDay byDay in this.ByDay) {
                        if (byDay.MatchesForMonth(dateTimeOfMonth)) {
                            yield return dateTimeOfMonth;
                            break;
                        }
                    }
                }
            } else if (RecurrenceFrequency.Yearly == this.Frequency) {
                var firstDateTimeOfYear = new DateTime(dateTime.Year, 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
                int totalDaysOfYear = UtcDateTime.DaysInYear(dateTime.Year);
                for (double d = 0; d < totalDaysOfYear; d++) {
                    DateTime dateTimeOfYear = firstDateTimeOfYear.AddDays(d);
                    foreach (ByDay byDay in this.ByDay) {
                        if (byDay.MatchesForYear(dateTimeOfYear)) {
                            yield return dateTimeOfYear;
                            break;
                        }
                    }
                }
            } else {
                throw new InvalidOperationException("Expanding dates by day is not supported for recurrence frequency " + this.Frequency + ".");
            }
        }

        /// <summary>
        /// Expands a date/time value by hour.
        /// </summary>
        /// <param name="dateTime">date/time to expand</param>
        /// <returns>expanded date/times</returns>
        private IEnumerable<DateTime> ExpandByHour(DateTime dateTime) {
            if (this.ByHour.Count < 1) {
                yield return dateTime;
            } else {
                var hours = new byte[this.ByHour.Count];
                this.ByHour.CopyTo(hours);
                Array.Sort(hours);
                foreach (byte hour in hours) {
                    yield return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
                }
            }
        }

        /// <summary>
        /// Expands a date/time value by minute.
        /// </summary>
        /// <param name="dateTime">date/time to expand</param>
        /// <returns>expanded date/times</returns>
        private IEnumerable<DateTime> ExpandByMinute(DateTime dateTime) {
            if (this.ByMinute.Count < 1) {
                yield return dateTime;
            } else {
                var minutes = new byte[this.ByMinute.Count];
                this.ByMinute.CopyTo(minutes);
                Array.Sort(minutes);
                foreach (byte minute in minutes) {
                    yield return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
                }
            }
        }

        /// <summary>
        /// Expands a date/time value by month.
        /// </summary>
        /// <param name="dateTime">date/time to expand</param>
        /// <returns>expanded date/times</returns>
        private IEnumerable<DateTime> ExpandByMonth(DateTime dateTime) {
            if (this.ByMonth.Count < 1) {
                yield return dateTime;
            } else {
                var months = new byte[this.ByMonth.Count];
                this.ByMonth.CopyTo(months);
                Array.Sort(months);
                foreach (byte month in months) {
                    if (dateTime.Day <= DateTime.DaysInMonth(dateTime.Year, month)) {
                        yield return new DateTime(dateTime.Year, month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
                    }
                }
            }
        }

        /// <summary>
        /// Expands a date/time value by day of month.
        /// </summary>
        /// <param name="dateTime">date/time to expand</param>
        /// <returns>expanded date/times</returns>
        private IEnumerable<DateTime> ExpandByMonthDay(DateTime dateTime) {
            if (this.ByMonthDay.Count < 1) {
                yield return dateTime;
            } else {
                int totalDaysOfMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                var monthDays = new List<int>(this.ByMonthDay.Count);
                foreach (int monthDay in this.ByMonthDay) {
                    if (monthDay > 0) {
                        monthDays.Add(monthDay);
                    } else if (monthDay < 0) {
                        monthDays.Add(totalDaysOfMonth + monthDay + 1);
                    }
                }
                monthDays.Sort();
                foreach (byte monthDay in monthDays) {
                    if (monthDay <= DateTime.DaysInMonth(dateTime.Year, dateTime.Month)) {
                        yield return new DateTime(dateTime.Year, dateTime.Month, monthDay, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
                    }
                }
            }
        }

        /// <summary>
        /// Expands a date/time value by second.
        /// </summary>
        /// <param name="dateTime">date/time to expand</param>
        /// <returns>expanded date/times</returns>
        private IEnumerable<DateTime> ExpandBySecond(DateTime dateTime) {
            if (this.BySecond.Count < 1) {
                yield return dateTime;
            } else {
                var seconds = new byte[this.BySecond.Count];
                this.BySecond.CopyTo(seconds);
                Array.Sort(seconds);
                foreach (byte second in seconds) {
                    yield return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, second, dateTime.Millisecond, dateTime.Kind);
                }
            }
        }

        /// <summary>
        /// Expands a date/time value by number of week.
        /// </summary>
        /// <param name="dateTime">date/time to expand</param>
        /// <returns>expanded date/times</returns>
        private IEnumerable<DateTime> ExpandByWeekNumber(DateTime dateTime) {
            if (this.ByWeekNumber.Count < 1) {
                yield return dateTime;
            } else {
                var calendar = new GregorianCalendar(GregorianCalendarTypes.Localized);
                const CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstFourDayWeek;
                int totalWeeksOfYear = calendar.GetWeekOfYear(new DateTime(dateTime.Year, 12, 31, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind), calendarWeekRule, this.WeekStart);
                var weekNumbers = new List<int>(this.ByWeekNumber.Count);
                foreach (int weekNumber in this.ByWeekNumber) {
                    if (weekNumber > 0) {
                        weekNumbers.Add(weekNumber);
                    } else if (weekNumber < 0) {
                        weekNumbers.Add(totalWeeksOfYear + weekNumber + 1);
                    }
                }
                weekNumbers.Sort();
                var firstWeekOfYear = dateTime.AddDays(-7 * (calendar.GetWeekOfYear(dateTime, calendarWeekRule, this.WeekStart) - 1));
                foreach (int weekNumber in weekNumbers) {
                    yield return firstWeekOfYear.AddDays((weekNumber - 1) * 7);
                }
            }
        }

        /// <summary>
        /// Expands a date/time value by day of year.
        /// </summary>
        /// <param name="dateTime">date/time to expand</param>
        /// <returns>expanded date/times</returns>
        private IEnumerable<DateTime> ExpandByYearDay(DateTime dateTime) {
            if (this.ByYearDay.Count < 1) {
                yield return dateTime;
            } else {
                int totalDaysOfYear = UtcDateTime.DaysInYear(dateTime.Year);
                var yearDays = new List<int>(this.ByYearDay.Count);
                foreach (int yearDay in this.ByYearDay) {
                    if (yearDay > 0) {
                        yearDays.Add(yearDay);
                    } else if (yearDay < 0) {
                        yearDays.Add(totalDaysOfYear + yearDay + 1);
                    }
                }
                yearDays.Sort();
                var firstDayOfYear = new DateTime(dateTime.Year, 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
                foreach (byte yearDay in yearDays) {
                    yield return firstDayOfYear.AddDays(yearDay - 1);
                }
            }
        }

        /// <summary>
        /// Gets all date/times matching the recurrence rule.
        /// </summary>
        /// <param name="start">start date/time</param>
        /// <returns>all date/times matching the recurrence rule</returns>
        public IEnumerable<DateTime> GetDateTimes(DateTime start) {
            if (!this.Frequency.HasValue) {
                throw new InvalidOperationException("Frequency must not be null in order to calculate recurrent date/times.");
            } else if (this.Interval < 1) {
                throw new InvalidOperationException("Interval must be > 0 in order to calculate recurrent date/times.");
            }
            uint count;
            if (this.Count.HasValue && this.Count > 0) {
                count = this.Count.Value;
            } else {
                count = uint.MaxValue;
            }
            DateTime lastDateTime = start;
            while (count > 0 && (null == this.EndDateTime || lastDateTime <= this.EndDateTime)) {
                bool isBroken = false;
                if (RecurrenceFrequency.Secondly == this.Frequency.Value) {
                    if (!this.IsLimitedByMonth(lastDateTime)
                        && !this.IsLimitedByYearDay(lastDateTime)
                        && !this.IsLimitedByMonthDay(lastDateTime)
                        && !this.IsLimitedByDay(lastDateTime)
                        && !this.IsLimitedByHour(lastDateTime)
                        && !this.IsLimitedByMinute(lastDateTime)
                        && !this.IsLimitedBySecond(lastDateTime)) {
                        yield return lastDateTime;
                        count--;
                    }
                    lastDateTime = lastDateTime.AddSeconds(this.Interval);
                } else if (RecurrenceFrequency.Minutely == this.Frequency) {
                    if (!this.IsLimitedByMonth(lastDateTime)
                        && !this.IsLimitedByYearDay(lastDateTime)
                        && !this.IsLimitedByMonthDay(lastDateTime)
                        && !this.IsLimitedByDay(lastDateTime)
                        && !this.IsLimitedByHour(lastDateTime)
                        && !this.IsLimitedByMinute(lastDateTime)) {
                        foreach (var expandedBySecond in this.ExpandBySecond(lastDateTime)) {
                            if (expandedBySecond >= start) {
                                lastDateTime = expandedBySecond;
                                if (count > 0 && (null == this.EndDateTime || lastDateTime <= this.EndDateTime)) {
                                    yield return lastDateTime;
                                    count--;
                                } else {
                                    break;
                                }
                            }
                        }
                    }
                    lastDateTime = lastDateTime.AddMinutes(this.Interval);
                } else if (RecurrenceFrequency.Hourly == this.Frequency) {
                    if (!this.IsLimitedByMonth(lastDateTime)
                        && !this.IsLimitedByYearDay(lastDateTime)
                        && !this.IsLimitedByMonthDay(lastDateTime)
                        && !this.IsLimitedByDay(lastDateTime)
                        && !this.IsLimitedByHour(lastDateTime)) {
                        foreach (var expandedByMinute in this.ExpandByMinute(lastDateTime)) {
                            foreach (var expandedBySecond in this.ExpandBySecond(expandedByMinute)) {
                                if (expandedBySecond >= start) {
                                    lastDateTime = expandedBySecond;
                                    if (count > 0 && (null == this.EndDateTime || lastDateTime <= this.EndDateTime)) {
                                        yield return lastDateTime;
                                        count--;
                                    } else {
                                        isBroken = true;
                                        break;
                                    }
                                }
                            }
                            if (isBroken) {
                                break;
                            }
                        }
                    }
                    lastDateTime = lastDateTime.AddHours(this.Interval);
                } else if (RecurrenceFrequency.Daily == this.Frequency) {
                    if (!this.IsLimitedByMonth(lastDateTime)
                        && !this.IsLimitedByYearDay(lastDateTime)
                        && !this.IsLimitedByMonthDay(lastDateTime)
                        && !this.IsLimitedByDay(lastDateTime)) {
                        foreach (var expandedByHour in this.ExpandByHour(lastDateTime)) {
                            foreach (var expandedByMinute in this.ExpandByMinute(expandedByHour)) {
                                foreach (var expandedBySecond in this.ExpandBySecond(expandedByMinute)) {
                                    if (expandedBySecond >= start) {
                                        lastDateTime = expandedBySecond;
                                        if (count > 0 && (null == this.EndDateTime || lastDateTime <= this.EndDateTime)) {
                                            yield return lastDateTime;
                                            count--;
                                        } else {
                                            isBroken = true;
                                            break;
                                        }
                                    }
                                }
                                if (isBroken) {
                                    break;
                                }
                            }
                            if (isBroken) {
                                break;
                            }
                        }
                    }
                    lastDateTime = lastDateTime.AddDays(this.Interval);
                } else if (RecurrenceFrequency.Weekly == this.Frequency) {
                    if (!this.IsLimitedByMonth(lastDateTime)) {
                        foreach (var expandedByDay in this.ExpandByDay(lastDateTime)) {
                            foreach (var expandedByHour in this.ExpandByHour(expandedByDay)) {
                                foreach (var expandedByMinute in this.ExpandByMinute(expandedByHour)) {
                                    foreach (var expandedBySecond in this.ExpandBySecond(expandedByMinute)) {
                                        if (expandedBySecond >= start) {
                                            lastDateTime = expandedBySecond;
                                            if (count > 0 && (null == this.EndDateTime || lastDateTime <= this.EndDateTime)) {
                                                yield return lastDateTime;
                                                count--;
                                            } else {
                                                isBroken = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (isBroken) {
                                        break;
                                    }
                                }
                                if (isBroken) {
                                    break;
                                }
                            }
                            if (isBroken) {
                                break;
                            }
                        }
                    }
                    lastDateTime = lastDateTime.AddDays(this.Interval * 7);
                } else if (RecurrenceFrequency.Monthly == this.Frequency) {
                    if (!this.IsLimitedByMonth(lastDateTime)) {
                        if (this.ByMonthDay.Count > 0) {
                            foreach (var expandedByMonthDay in this.ExpandByMonthDay(lastDateTime)) {
                                if (!this.IsLimitedByDay(expandedByMonthDay)) {
                                    foreach (var expandedByHour in this.ExpandByHour(expandedByMonthDay)) {
                                        foreach (var expandedByMinute in this.ExpandByMinute(expandedByHour)) {
                                            foreach (var expandedBySecond in this.ExpandBySecond(expandedByMinute)) {
                                                if (expandedBySecond >= start) {
                                                    lastDateTime = expandedBySecond;
                                                    if (count > 0 && (null == this.EndDateTime || lastDateTime <= this.EndDateTime)) {
                                                        yield return lastDateTime;
                                                        count--;
                                                    } else {
                                                        isBroken = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (isBroken) {
                                                break;
                                            }
                                        }
                                        if (isBroken) {
                                            break;
                                        }
                                    }
                                }
                                if (isBroken) {
                                    break;
                                }
                            }
                        } else {
                            foreach (var expandedByDay in this.ExpandByDay(lastDateTime)) {
                                foreach (var expandedByHour in this.ExpandByHour(expandedByDay)) {
                                    foreach (var expandedByMinute in this.ExpandByMinute(expandedByHour)) {
                                        foreach (var expandedBySecond in this.ExpandBySecond(expandedByMinute)) {
                                            if (expandedBySecond >= start) {
                                                lastDateTime = expandedBySecond;
                                                if (count > 0 && (null == this.EndDateTime || lastDateTime <= this.EndDateTime)) {
                                                    yield return lastDateTime;
                                                    count--;
                                                } else {
                                                    isBroken = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (isBroken) {
                                            break;
                                        }
                                    }
                                    if (isBroken) {
                                        break;
                                    }
                                }
                                if (isBroken) {
                                    break;
                                }
                            }
                        }
                    }
                    lastDateTime = lastDateTime.AddMonths((int)this.Interval);
                } else if (RecurrenceFrequency.Yearly == this.Frequency) {
                    foreach (var expandedByMonth in this.ExpandByMonth(lastDateTime)) {
                        foreach (var expandedByWeekNumber in this.ExpandByWeekNumber(expandedByMonth)) {
                            if (this.ByYearDay.Count > 0 || this.ByMonthDay.Count > 0) {
                                foreach (var expandedByYearDay in this.ExpandByYearDay(expandedByWeekNumber)) {
                                    foreach (var expandedByMonthDay in this.ExpandByMonthDay(expandedByYearDay)) {
                                        if (!this.IsLimitedByDay(expandedByMonthDay)) {
                                            foreach (var expandedByHour in this.ExpandByHour(expandedByMonthDay)) {
                                                foreach (var expandedByMinute in this.ExpandByMinute(expandedByHour)) {
                                                    foreach (var expandedBySecond in this.ExpandBySecond(expandedByMinute)) {
                                                        if (expandedBySecond >= start) {
                                                            lastDateTime = expandedBySecond;
                                                            if (count > 0 && (null == this.EndDateTime || lastDateTime <= this.EndDateTime)) {
                                                                yield return lastDateTime;
                                                                count--;
                                                            } else {
                                                                isBroken = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (isBroken) {
                                                        break;
                                                    }
                                                }
                                                if (isBroken) {
                                                    break;
                                                }
                                            }
                                        }
                                        if (isBroken) {
                                            break;
                                        }
                                    }
                                    if (isBroken) {
                                        break;
                                    }
                                }
                            } else {
                                foreach (var expandedByDay in this.ExpandByDay(expandedByWeekNumber)) {
                                    foreach (var expandedByHour in this.ExpandByHour(expandedByDay)) {
                                        foreach (var expandedByMinute in this.ExpandByMinute(expandedByHour)) {
                                            foreach (var expandedBySecond in this.ExpandBySecond(expandedByMinute)) {
                                                if (expandedBySecond >= start) {
                                                    lastDateTime = expandedBySecond;
                                                    if (count > 0 && (null == this.EndDateTime || lastDateTime <= this.EndDateTime)) {
                                                        yield return lastDateTime;
                                                        count--;
                                                    } else {
                                                        isBroken = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (isBroken) {
                                                break;
                                            }
                                        }
                                        if (isBroken) {
                                            break;
                                        }
                                    }
                                    if (isBroken) {
                                        break;
                                    }
                                }
                            }
                            if (isBroken) {
                                break;
                            }
                        }
                        if (isBroken) {
                            break;
                        }
                    }
                    lastDateTime = lastDateTime.AddYears((int)this.Interval);
                }
            }
        }

        /// <summary>
        /// Determines whether a specific date/time is limited by
        /// day.
        /// </summary>
        /// <param name="dateTime">date/time to check</param>
        /// <returns>true if date/time is limited, false otherwise</returns>
        private bool IsLimitedByDay(DateTime dateTime) {
            bool isLimited;
            if (this.ByDay.Count < 1) {
                isLimited = false;
            } else {
                isLimited = true;
                foreach (ByDay byDay in this.ByDay) {
                    if (byDay.DayOfWeek == dateTime.DayOfWeek) {
                        isLimited = false;
                        break;
                    }
                }
            }
            return isLimited;
        }

        /// <summary>
        /// Determines whether a specific date/time is limited by
        /// hour.
        /// </summary>
        /// <param name="dateTime">date/time to check</param>
        /// <returns>true if date/time is limited, false otherwise</returns>
        private bool IsLimitedByHour(DateTime dateTime) {
            return RecurrenceRule.IsLimitedByValues(dateTime.Hour, this.ByHour);
        }

        /// <summary>
        /// Determines whether a specific date/time is limited by
        /// minute.
        /// </summary>
        /// <param name="dateTime">date/time to check</param>
        /// <returns>true if date/time is limited, false otherwise</returns>
        private bool IsLimitedByMinute(DateTime dateTime) {
            return RecurrenceRule.IsLimitedByValues(dateTime.Minute, this.ByMinute);
        }

        /// <summary>
        /// Determines whether a specific date/time is limited by
        /// month.
        /// </summary>
        /// <param name="dateTime">date/time to check</param>
        /// <returns>true if date/time is limited, false otherwise</returns>
        private bool IsLimitedByMonth(DateTime dateTime) {
            return RecurrenceRule.IsLimitedByValues(dateTime.Month, this.ByMonth);
        }

        /// <summary>
        /// Determines whether a specific date/time is limited by
        /// day of month.
        /// </summary>
        /// <param name="dateTime">date/time to check</param>
        /// <returns>true if date/time is limited, false otherwise</returns>
        private bool IsLimitedByMonthDay(DateTime dateTime) {
            bool isLimited;
            if (this.ByMonthDay.Count < 1) {
                isLimited = false;
            } else {
                int positiveDayOfMonth = dateTime.Month;
                int totalDaysOfMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                int negativeDayOfMonth = positiveDayOfMonth - totalDaysOfMonth - 1;
                if (this.ByMonthDay.Contains(positiveDayOfMonth) || this.ByMonthDay.Contains(negativeDayOfMonth)) {
                    isLimited = false;
                } else {
                    isLimited = true;
                }
            }
            return isLimited;
        }

        /// <summary>
        /// Determines whether a specific date/time is limited by
        /// second.
        /// </summary>
        /// <param name="dateTime">date/time to check</param>
        /// <returns>true if date/time is limited, false otherwise</returns>
        private bool IsLimitedBySecond(DateTime dateTime) {
            return RecurrenceRule.IsLimitedByValues(dateTime.Second, this.BySecond);
        }

        /// <summary>
        /// Determines whether a specific value is limited by a
        /// collection of allowed values.
        /// </summary>
        /// <param name="value">value to check</param>
        /// <param name="allowedValues">collection of allowed values</param>
        /// <returns>true if value is limited, false otherwise</returns>
        private static bool IsLimitedByValues(int value, ICollection<byte> allowedValues) {
            bool isLimited;
            if (allowedValues.Count < 1) {
                isLimited = false;
            } else {
                isLimited = true;
                foreach (byte allowedValue in allowedValues) {
                    if (value == allowedValue) {
                        isLimited = false;
                        break;
                    }
                }
            }
            return isLimited;
        }

        /// <summary>
        /// Determines whether a specific date/time is limited by
        /// day of year.
        /// </summary>
        /// <param name="dateTime">date/time to check</param>
        /// <returns>true if date/time is limited, false otherwise</returns>
        private bool IsLimitedByYearDay(DateTime dateTime) {
            bool isLimited;
            if (this.ByYearDay.Count < 1) {
                isLimited = false;
            } else {
                int positiveDayOfYear = dateTime.DayOfYear;
                int totalDaysOfYear = UtcDateTime.DaysInYear(dateTime.Year);
                int negativeDayOfYear = positiveDayOfYear - totalDaysOfYear - 1;
                if (this.ByYearDay.Contains(positiveDayOfYear) || this.ByYearDay.Contains(negativeDayOfYear)) {
                    isLimited = false;
                } else {
                    isLimited = true;
                }
            }
            return isLimited;
        }

    }

}