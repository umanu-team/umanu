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

namespace Framework.Presentation.Converters {

    using Framework.Persistence;
    using Model;
    using Model.Calendar;
    using Presentation;
    using Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using static System.TimeZoneInfo;

    /// <summary>
    /// Builder class for iCalendar output.
    /// </summary>
    internal sealed class VCalendarBuilder {

        /// <summary>
        /// True to include attachments in output, false to skip
        /// attachments.
        /// </summary>
        public bool IsWritingAttachments { get; set; }

        /// <summary>
        /// RFC 5546 method to be used.
        /// </summary>
        public Method Method { get; private set; }

        /// <summary>
        /// String builder.
        /// </summary>
        private readonly StringBuilder stringBuilder;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="method">RFC 5546 method to be used</param>
        public VCalendarBuilder(Method method) {
            this.IsWritingAttachments = true;
            this.Method = method;
            this.stringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Appends an alarm.
        /// </summary>
        /// <param name="alarm">alarm to be appened</param>
        public void Append(Alarm alarm) {
            if (null != alarm) {
                this.Append("BEGIN", "VALARM");
                if (alarm is AudioAlarm audioAlarm) {
                    this.Append("ACTION", "AUDIO");
                    if (null != audioAlarm.Attachment) {
                        this.Append("ATTACH", audioAlarm.Attachment);
                    }
                } else if (alarm is DisplayAlarm displayAlarm) {
                    this.Append("ACTION", "DISPLAY");
                    if (!string.IsNullOrEmpty(displayAlarm.Description)) {
                        this.AppendText("DESCRIPTION", displayAlarm.Description);
                    }
                } else if (alarm is EmailAlarm emailAlarm) {
                    this.Append("ACTION", "EMAIL");
                    foreach (var attachment in emailAlarm.Attachments) {
                        this.Append("ATTACH", attachment);
                    }
                    foreach (var attendee in emailAlarm.Attendees) {
                        this.Append("ATTENDEE", attendee);
                    }
                    if (!string.IsNullOrEmpty(emailAlarm.Description)) {
                        this.AppendText("DESCRIPTION", emailAlarm.Description);
                    }
                    if (!string.IsNullOrEmpty(emailAlarm.Title)) {
                        this.AppendText("SUMMARY", emailAlarm.Title);
                    }
                }
                if (alarm.RepetitionDuration.HasValue) {
                    this.Append("DURATION", alarm.RepetitionDuration.Value);
                }
                if (alarm.RepetitionCount.HasValue) {
                    this.Append("REPEAT", alarm.RepetitionCount.Value);
                }
                this.Append("TRIGGER;RELATED=" + alarm.TriggerRelation.ToString().ToUpperInvariant(), alarm.TriggerTimeSpan);
                this.Append("END", "VALARM");
            }
            return;
        }

        /// <summary>
        /// Appends an event.
        /// </summary>
        /// <param name="item">event to append</param>
        public void Append(Event item) {
            this.Append("BEGIN", "VEVENT");
            this.Append("UID", item.Id.ToString("N"));
            if (!this.IsFor(Method.Refresh)) {
                foreach (var attachment in item.Attachments) {
                    this.Append("ATTACH", attachment);
                }
                if (item.Categories.Count > 0) {
                    this.AppendText("CATEGORIES", item.Categories);
                }
                if (item.Classification.HasValue) {
                    this.Append("CLASS", item.Classification.Value.ToString().ToUpperInvariant());
                }
                foreach (var contact in item.Contacts?.Members) {
                    this.Append("CONTACT", contact);
                }
                this.Append("CREATED", item.CreatedAt, item.TimeZone);
                if (!string.IsNullOrEmpty(item.Description)) {
                    this.AppendDescription(item.Description);
                }
                if (item.EndDateTime.HasValue) {
                    this.Append("DTEND", item.EndDateTime.Value, item.TimeZone);
                } else {
                    if (this.IsFor(new Method[] { Method.Publish, Method.Request, Method.Reply, Method.Add, Method.Counter })) {
                        throw new ArgumentException("Mandatory property \"" + nameof(item.EndDateTime) + "\" for RFC 5546 method \"" + this.Method.ToString() + "\" is not set.", nameof(item));
                    }
                }
                if (item.StartDateTime.HasValue) {
                    this.Append("DTSTART", item.StartDateTime.Value, item.TimeZone);
                } else {
                    if (this.IsFor(new Method[] { Method.Publish, Method.Request, Method.Reply, Method.Add, Method.Counter })) {
                        throw new ArgumentException("Mandatory property \"" + nameof(item.StartDateTime) + "\" for RFC 5546 method \"" + this.Method.ToString() + "\" is not set.", nameof(item));
                    }
                }
                if (null != item.Location) {
                    if (item.Location.Latitude.HasValue && item.Location.Longitude.HasValue) {
                        this.Append("GEO", item.Location.Latitude.Value.ToString(CultureInfo.InvariantCulture) + ";" + item.Location.Longitude.Value.ToString(CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(item.Location.Title)) {
                        this.AppendText("LOCATION", item.Location.Title);
                    }
                }
                this.Append("LAST-MODIFIED", item.ModifiedAt, item.TimeZone);
                if (item.Priority.HasValue && Priority.NotDefined != item.Priority) {
                    this.Append("PRIORITY", (int)item.Priority.Value);
                }
                if (item.Status.HasValue) {
                    this.Append("STATUS", item.Status.Value.ToString().ToUpperInvariant());
                }
                this.AppendText("SUMMARY", item.Title);
                if (item.TimeTransparency.HasValue) {
                    this.Append("TRANSP", item.TimeTransparency.Value.ToString().ToUpperInvariant());
                }
                if (!string.IsNullOrEmpty(item.WebSite)) {
                    this.AppendText("URL", item.WebSite);
                }
            }
            if (this.IsFor(Method.None)) {
                this.Append("DTSTAMP", item.ModifiedAt, item.TimeZone);
            } else {
                if (!this.IsFor(Method.Publish)) {
                    foreach (var attendee in item.Attendees) {
                        this.Append("ATTENDEE", attendee);
                    }
                }
                foreach (var comment in item.Comments) {
                    this.AppendText("COMMENT", comment);
                }
                this.Append("DTSTAMP", item.CreatedAt, item.TimeZone);
                if (null != item.Organizer) {
                    this.Append("ORGANIZER", item.Organizer);
                }
                if (!this.IsFor(Method.Add)) {
                    if (item.RecurrenceId.HasValue) {
                        this.Append("RECURRENCE-ID", item.RecurrenceId.Value, item.TimeZone);
                    }
                }
                if (!this.IsFor(Method.Refresh)) {
                    if (item.ResourceCollection.Count > 0) {
                        this.AppendText("RESOURCES", item.ResourceCollection);
                    }
                    this.Append("SEQUENCE", item.SequenceNumber.ToString(CultureInfo.InvariantCulture));
                }
            }
            if (!this.IsFor(new Method[] { Method.Add, Method.Refresh })) {
                if (item.ExceptionDateTimes.Count > 0) {
                    this.Append("EXDATE", item.ExceptionDateTimes, item.TimeZone);
                }
                if (item.RecurrenceDateTimes.Count > 0) {
                    this.Append("RDATE", item.RecurrenceDateTimes, item.TimeZone);
                }
                if (null != item.RecurrenceRule) {
                    this.Append("RRULE", item.RecurrenceRule, item.TimeZone);
                }
            }
            if (!this.IsFor(new Method[] { Method.Reply, Method.Cancel, Method.Refresh, Method.DeclineCounter })) {
                foreach (var alarm in item.Alarms) {
                    if (alarm is DisplayAlarm displayAlarm && string.IsNullOrEmpty(displayAlarm.Description)) {
                        var modifiedAlarm = new DisplayAlarm();
                        modifiedAlarm.CopyFrom(displayAlarm, CopyBehaviorForAllowedGroups.DoNotCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.DeepCopy);
                        modifiedAlarm.Description = item.Title;
                        this.Append(modifiedAlarm);
                    } else {
                        this.Append(alarm);
                    }
                }
            }
            this.Append("END", "VEVENT");
            return;
        }

        /// <summary>
        /// Appends an event.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// append</param>
        /// <param name="view">event list view to be applied for
        /// field mapping</param>
        public void Append(IPresentableObject presentableObject, VEventListView view) {
            var item = new Event {
                Location = new Location(),
                RecurrenceRule = new RecurrenceRule(),
                Id = VCalendarBuilder.GetValueOfItemProperty<Guid>(presentableObject, view.IdKey)
            };
            VCalendarBuilder.SetValuesToItemProperty(item.Alarms, presentableObject, view.AlarmsKey);
            VCalendarBuilder.SetValuesToItemProperty(item.Attachments, presentableObject, view.AttachmentsKey);
            VCalendarBuilder.SetValuesToItemProperty(item.Categories, presentableObject, view.CategoriesKey);
            item.Classification = VCalendarBuilder.GetValueOfItemProperty<Classification?>(presentableObject, view.ClassificationKey);
            VCalendarBuilder.SetValuesToItemProperty(item.Comments, presentableObject, view.CommentsKey);
            VCalendarBuilder.SetValuesToItemProperty(item.Contacts.Members, presentableObject, view.ContactsKey);
            item.CreatedAt = VCalendarBuilder.GetValueOfItemProperty<DateTime>(presentableObject, view.CreatedAtKey);
            item.Description = VCalendarBuilder.GetStringValueOfItemProperty(presentableObject, view.DescriptionKey);
            item.EndDateTime = VCalendarBuilder.GetValueOfItemProperty<DateTime?>(presentableObject, view.EndDateTimeKey);
            VCalendarBuilder.SetValuesToItemProperty(item.ExceptionDateTimes, presentableObject, view.ExceptionDateTimesKey);
            item.Location.Latitude = VCalendarBuilder.GetValueOfItemProperty<decimal?>(presentableObject, view.LocationLatitudeKey);
            item.Location.Longitude = VCalendarBuilder.GetValueOfItemProperty<decimal?>(presentableObject, view.LocationLongitudeFieldKey);
            item.Location.Title = VCalendarBuilder.GetStringValueOfItemProperty(presentableObject, view.LocationTitleKey);
            item.ModifiedAt = VCalendarBuilder.GetValueOfItemProperty<DateTime>(presentableObject, view.ModifiedAtKey);
            item.Organizer = VCalendarBuilder.GetValueOfItemProperty<IUser>(presentableObject, view.OrganizerKey);
            item.Priority = VCalendarBuilder.GetValueOfItemProperty<Priority?>(presentableObject, view.PriorityKey);
            item.RecurrenceId = VCalendarBuilder.GetValueOfItemProperty<DateTime?>(presentableObject, view.RecurrenceIdKey);
            VCalendarBuilder.SetValuesToItemProperty(item.RecurrenceDateTimes, presentableObject, view.RecurrenceDateTimesKey);
            VCalendarBuilder.SetValuesToItemProperty(item.RecurrenceRule.ByDay, presentableObject, view.RecurrenceRuleByDayKey);
            VCalendarBuilder.SetValuesToItemProperty(item.RecurrenceRule.ByHour, presentableObject, view.RecurrenceRuleByHourKey);
            VCalendarBuilder.SetValuesToItemProperty(item.RecurrenceRule.ByMinute, presentableObject, view.RecurrenceRuleByMinuteKey);
            VCalendarBuilder.SetValuesToItemProperty(item.RecurrenceRule.ByMonth, presentableObject, view.RecurrenceRuleByMonthKey);
            VCalendarBuilder.SetValuesToItemProperty(item.RecurrenceRule.ByMonthDay, presentableObject, view.RecurrenceRuleByMonthDayKey);
            VCalendarBuilder.SetValuesToItemProperty(item.RecurrenceRule.BySecond, presentableObject, view.RecurrenceRuleBySecondKey);
            VCalendarBuilder.SetValuesToItemProperty(item.RecurrenceRule.BySetPos, presentableObject, view.RecurrenceRuleBySetPosKey);
            VCalendarBuilder.SetValuesToItemProperty(item.RecurrenceRule.ByWeekNumber, presentableObject, view.RecurrenceRuleByWeekNumberKey);
            VCalendarBuilder.SetValuesToItemProperty(item.RecurrenceRule.ByYearDay, presentableObject, view.RecurrenceRuleByYearDayKey);
            item.RecurrenceRule.Count = VCalendarBuilder.GetValueOfItemProperty<uint?>(presentableObject, view.RecurrenceRuleCountKey);
            item.RecurrenceRule.EndDateTime = VCalendarBuilder.GetValueOfItemProperty<DateTime?>(presentableObject, view.EndDateTimeKey);
            item.RecurrenceRule.Frequency = VCalendarBuilder.GetValueOfItemProperty<RecurrenceFrequency?>(presentableObject, view.RecurrenceRuleFrequencyKey);
            item.RecurrenceRule.Interval = VCalendarBuilder.GetValueOfItemProperty<uint>(presentableObject, view.RecurrenceRuleIntervalKey);
            item.RecurrenceRule.WeekStart = VCalendarBuilder.GetValueOfItemProperty<DayOfWeek>(presentableObject, view.RecurrenceRuleWeekStartKey);
            VCalendarBuilder.SetValuesToItemProperty(item.ResourceCollection, presentableObject, view.ResourcesKey);
            item.SequenceNumber = VCalendarBuilder.GetValueOfItemProperty<uint>(presentableObject, view.SequenceKey);
            item.StartDateTime = VCalendarBuilder.GetValueOfItemProperty<DateTime?>(presentableObject, view.StartDateTimeKey);
            item.Status = VCalendarBuilder.GetValueOfItemProperty<EventStatus?>(presentableObject, view.StatusKey);
            item.Title = VCalendarBuilder.GetStringValueOfItemProperty(presentableObject, view.TitleKey);
            item.TimeTransparency = VCalendarBuilder.GetValueOfItemProperty<TimeTransparency?>(presentableObject, view.TimeTransparencyKey);
            item.WebSite = VCalendarBuilder.GetStringValueOfItemProperty(presentableObject, view.WebSiteKey);
            this.Append(item);
            return;
        }

        /// <summary>
        /// Appends a pair of key and value.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="value">date/time value to append</param>
        /// <param name="timeZone">time zone to append date/time
        /// values for - a corresponding time zone definition has to
        /// be appended before if the time zone is not UTC</param>
        public void Append(string key, DateTime value, TimeZoneInfo timeZone) {
            this.Append(key, new DateTime[] { value }, timeZone);
            return;
        }

        /// <summary>
        /// Appends a pair of key and value.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="file">file value to append</param>
        public void Append(string key, File file) {
            if (this.IsWritingAttachments && null != file) {
                var fileName = file.Name;
                fileName = fileName  // see https://msdn.microsoft.com/en-us/library/ee158052%28v=exchg.80%29.aspx and https://msdn.microsoft.com/en-us/library/gg672027(v=exchg.80).aspx
                    .Replace('+', '_')
                    .Replace(',', '_')
                    .Replace('=', '_')
                    .Replace('[', '_')
                    .Replace(']', '_')
                    .Replace(';', '_');
                var fileNameBuilder = new StringBuilder(fileName.Length);
                foreach (var c in fileName) {
                    if ((c >= 'a' && c <= 'z')
                        || (c >= 'A' && c <= 'Z')
                        || (c >= '1' && c <= '9')
                        || '0' == c
                        || '.' == c
                        || '_' == c) {
                        fileNameBuilder.Append(c);
                    }
                }
                fileName = fileNameBuilder.ToString();
                if (string.IsNullOrEmpty(fileName)) {
                    fileName = Guid.NewGuid().ToString("N");
                }
                this.Append("ATTACH;FMTTYPE=" + file.MimeType + ";ENCODING=BASE64;VALUE=BINARY;X-FILENAME=" + fileName, Convert.ToBase64String(file.Bytes));
            }
            return;
        }

        /// <summary>
        /// Appends a pair of key and values.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="values">date/time values to append</param>
        /// <param name="timeZone">time zone to append date/time
        /// values for - a corresponding time zone definition has to
        /// be appended before if the time zone is not UTC</param>
        public void Append(string key, IEnumerable<DateTime> values, TimeZoneInfo timeZone) {
            string stringKey;
            bool isTimeZoneUtc = TimeZoneInfo.Utc == timeZone;
            if (isTimeZoneUtc || null == timeZone) {
                stringKey = key;
            } else {
                stringKey = key + ";TZID=" + this.GetTzid(timeZone);
            }
            var stringValues = new List<string>();
            foreach (var value in values) {
                if (null == timeZone) {
                    stringValues.Add(value.ToString("yyyyMMdd\\THHmmss"));
                } else if (isTimeZoneUtc) {
                    stringValues.Add(value.ToUniversalTime().ToString("yyyyMMdd\\THHmmss\\Z"));
                } else {
                    stringValues.Add(TimeZoneInfo.ConvertTimeFromUtc(value, timeZone).ToString("yyyyMMdd\\THHmmss"));
                }
            }
            this.Append(stringKey, stringValues);
            return;
        }

        /// <summary>
        /// Appends a pair of key and values.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="values">values to append</param>
        public void Append(string key, IEnumerable<string> values) {
            this.stringBuilder.Append(key);
            this.stringBuilder.Append(':');
            bool isFirstValue = true;
            foreach (var value in values) {
                if (isFirstValue) {
                    isFirstValue = false;
                } else {
                    this.stringBuilder.Append(',');
                }
                this.stringBuilder.Append(value);
            }
            this.stringBuilder.Append("\r\n");
            return;
        }

        /// <summary>
        /// Appends a pair of key and value.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="value">numeric value to append</param>
        public void Append(string key, int value) {
            this.Append(key, value.ToString(CultureInfo.InvariantCulture));
            return;
        }

        /// <summary>
        /// Appends a pair of key and value.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="user">IUser value to append</param>
        public void Append(string key, IUser user) {
            string value = null;
            if (null != user) {
                if (!string.IsNullOrEmpty(user.EmailAddress)) {
                    value = "MAILTO:" + user.EmailAddress;
                } else if (!string.IsNullOrEmpty(user.FirstName) || !string.IsNullOrEmpty(user.LastName)) {
                    value = (user.FirstName + ' ' + user.LastName).Trim();
                }
            }
            this.Append(key, value);
            return;
        }

        /// <summary>
        /// Appends a pair of key and value.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="value">recurrence rule value to append</param>
        /// <param name="timeZone">time zone to append date/time
        /// values for - a corresponding time zone definition has to
        /// be appended before if the time zone is not UTC</param>
        public void Append(string key, RecurrenceRule value, TimeZoneInfo timeZone) {
            if (value.HasValue) {
                var recurrenceRuleBuilder = new StringBuilder();
                recurrenceRuleBuilder.Append("FREQ=");
                recurrenceRuleBuilder.Append(value.Frequency.ToString().ToUpperInvariant());
                if (value.EndDateTime.HasValue) {
                    recurrenceRuleBuilder.Append(";UNTIL=");
                    if (null == timeZone) {
                        recurrenceRuleBuilder.Append(value.EndDateTime.Value.ToString("yyyyMMdd\\THHmmss"));
                    } else if (TimeZoneInfo.Utc == timeZone) {
                        recurrenceRuleBuilder.Append(value.EndDateTime.Value.ToUniversalTime().ToString("yyyyMMdd\\THHmmss\\Z"));
                    } else {
                        recurrenceRuleBuilder.Append(TimeZoneInfo.ConvertTimeFromUtc(value.EndDateTime.Value, timeZone).ToString("yyyyMMdd\\THHmmss"));
                    }
                }
                if (value.Count.HasValue) {
                    recurrenceRuleBuilder.Append(";COUNT=");
                    recurrenceRuleBuilder.Append(value.Count.Value.ToString(CultureInfo.InvariantCulture));
                }
                recurrenceRuleBuilder.Append(";INTERVAL=");
                recurrenceRuleBuilder.Append(value.Interval.ToString(CultureInfo.InvariantCulture));
                if (value.BySecond.Count > 0) {
                    recurrenceRuleBuilder.Append(";BYSECOND=");
                    recurrenceRuleBuilder.Append(value.BySecond.Join(","));
                }
                if (value.ByMinute.Count > 0) {
                    recurrenceRuleBuilder.Append(";BYMINUTE=");
                    recurrenceRuleBuilder.Append(value.ByMinute.Join(","));
                }
                if (value.ByHour.Count > 0) {
                    recurrenceRuleBuilder.Append(";BYHOUR=");
                    recurrenceRuleBuilder.Append(value.ByHour.Join(","));
                }
                if (value.ByDay.Count > 0) {
                    recurrenceRuleBuilder.Append(";BYDAY=");
                    bool isFirst = true;
                    foreach (var byDay in value.ByDay) {
                        if (true == byDay?.DayOfWeek.HasValue) {
                            if (isFirst) {
                                isFirst = false;
                            } else {
                                recurrenceRuleBuilder.Append(',');
                            }
                            if (byDay.Occurrence.HasValue) {
                                recurrenceRuleBuilder.Append(byDay.Occurrence.Value.ToString(CultureInfo.InvariantCulture));
                            }
                            recurrenceRuleBuilder.Append(VCalendarBuilder.GetDayOfWeekAsString(byDay.DayOfWeek.Value));
                        }
                    }
                }
                if (value.ByMonthDay.Count > 0) {
                    recurrenceRuleBuilder.Append(";BYMONTHDAY=");
                    recurrenceRuleBuilder.Append(value.ByMonthDay.Join(","));
                }
                if (value.ByYearDay.Count > 0) {
                    recurrenceRuleBuilder.Append(";BYYEARDAY=");
                    recurrenceRuleBuilder.Append(value.ByYearDay.Join(","));
                }
                if (value.ByWeekNumber.Count > 0) {
                    recurrenceRuleBuilder.Append(";BYWEEKNO=");
                    recurrenceRuleBuilder.Append(value.ByWeekNumber.Join(","));
                }
                if (value.ByMonth.Count > 0) {
                    recurrenceRuleBuilder.Append(";BYMONTH=");
                    recurrenceRuleBuilder.Append(value.ByMonth.Join(","));
                }
                if (value.BySetPos.Count > 0) {
                    recurrenceRuleBuilder.Append(";BYSETPOS=");
                    recurrenceRuleBuilder.Append(value.BySetPos.Join(","));
                }
                recurrenceRuleBuilder.Append(";WKST=");
                recurrenceRuleBuilder.Append(VCalendarBuilder.GetDayOfWeekAsString(value.WeekStart));
                this.Append(key, recurrenceRuleBuilder.ToString());
            }
            return;
        }

        /// <summary>
        /// Appends a pair of key and value.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="value">value to append</param>
        public void Append(string key, string value) {
            this.Append(key, new string[] { value });
            return;
        }

        /// <summary>
        /// Appends a pair of key and value.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="timeSpan">time span value to append</param>
        public void Append(string key, TimeSpan timeSpan) {
            string value;
            if (null == timeSpan) {
                value = null;
            } else {
                var valueBuilder = new StringBuilder();
                if (timeSpan.Ticks < 0) {
                    valueBuilder.Append('-');
                }
                valueBuilder.Append('P');
                if (timeSpan.Days != 0) {
                    valueBuilder.Append(Math.Abs(timeSpan.Days).ToString(CultureInfo.InvariantCulture));
                    valueBuilder.Append('D');
                }
                valueBuilder.Append('T');
                if (timeSpan.Hours != 0) {
                    valueBuilder.Append(Math.Abs(timeSpan.Hours).ToString(CultureInfo.InvariantCulture));
                    valueBuilder.Append('H');
                }
                if (timeSpan.Minutes != 0) {
                    valueBuilder.Append(Math.Abs(timeSpan.Minutes).ToString(CultureInfo.InvariantCulture));
                    valueBuilder.Append('M');
                }
                valueBuilder.Append(Math.Abs(timeSpan.Seconds).ToString(CultureInfo.InvariantCulture));
                valueBuilder.Append('S');
                value = valueBuilder.ToString();
            }
            this.Append(key, value);
            return;
        }

        /// <summary>
        /// Appends a string.
        /// </summary>
        /// <param name="s">string to append</param>
        public void Append(string s) {
            this.stringBuilder.Append(s);
            return;
        }

        /// <summary>
        /// Appends a time zone.
        /// </summary>
        /// <param name="timeZone">time zone to append</param>
        public void Append(TimeZoneInfo timeZone) {
            this.Append("BEGIN", "VTIMEZONE");
            this.Append("TZID", this.GetTzid(timeZone));
            var adjustmentRules = timeZone.GetAdjustmentRules(); // have to be ordered ascending
            if (adjustmentRules.LongLength < 1L) {
                this.Append(timeZone, DateTime.MinValue);
            } else {
                bool isFirstAdjustmentRule = true;
                for (long i = 0; i < adjustmentRules.LongLength; i++) {
                    var adjustmentRule = adjustmentRules[i];
                    var isLastAdjustmentRule = i == adjustmentRules.LongLength - 1;
                    if (isFirstAdjustmentRule) {
                        if (adjustmentRule.DateStart > DateTime.MinValue) {
                            this.Append(timeZone, DateTime.MinValue);
                        }
                    }
                    int daylightTimeYear = adjustmentRule.DateStart.Year;
                    int standardTimeYear = adjustmentRule.DateStart.Year;
                    var daylightTransitionStart = adjustmentRule.DaylightTransitionStart;
                    var firstDaylightDateTime = GetTransitionDateTime(daylightTransitionStart, daylightTimeYear);
                    if (firstDaylightDateTime.Month != 1 || firstDaylightDateTime.Day != 1 || firstDaylightDateTime.Hour != 0 || firstDaylightDateTime.Minute != 0 || firstDaylightDateTime.Second != 0) { // unfortunatelly it is not documented by Microsoft how to handle this case, so it is probably not implemented correctly
                        this.Append("BEGIN", "DAYLIGHT");
                        var recurrenceRule = new RecurrenceRule {
                            Frequency = RecurrenceFrequency.Yearly
                        };
                        if (!daylightTransitionStart.IsFixedDateRule) {
                            recurrenceRule.ByDay.Add(new ByDay(daylightTransitionStart.DayOfWeek, daylightTransitionStart.Week));
                            recurrenceRule.ByMonth.Add(Convert.ToByte(daylightTransitionStart.Month)); // valid month values are from 1 to 12
                        }
                        if (adjustmentRule.DateEnd < DateTime.MaxValue.AddDays(-1)) {
                            recurrenceRule.EndDateTime = adjustmentRule.DateEnd;
                        }
                        this.Append("DTSTART", firstDaylightDateTime, null);
                        this.Append("RRULE", recurrenceRule, timeZone);
                        this.AppendTimeZoneOffset("TZOFFSETFROM", timeZone.BaseUtcOffset);
                        this.AppendTimeZoneOffset("TZOFFSETTO", timeZone.BaseUtcOffset + adjustmentRule.DaylightDelta);
                        this.Append("END", "DAYLIGHT");
                    }
                    TransitionTime? standardTransitionStart = adjustmentRule.DaylightTransitionEnd;
                    if (standardTransitionStart.Value.Month < daylightTransitionStart.Month) {
                        standardTransitionStart = null;
                        standardTimeYear++;
                        foreach (var potentialNextAdjustmentRule in adjustmentRules) {
                            if (potentialNextAdjustmentRule.DateStart.Year <= standardTimeYear && potentialNextAdjustmentRule.DateEnd.Year >= standardTimeYear) {
                                adjustmentRule = potentialNextAdjustmentRule;
                                standardTransitionStart = potentialNextAdjustmentRule.DaylightTransitionEnd;
                                break;
                            }
                        }
                    }
                    if (standardTransitionStart.HasValue) {
                        var firstStandardDateTime = GetTransitionDateTime(standardTransitionStart.Value, standardTimeYear);
                        this.Append("BEGIN", "STANDARD");
                        var recurrenceRule = new RecurrenceRule {
                            Frequency = RecurrenceFrequency.Yearly
                        };
                        if (!standardTransitionStart.Value.IsFixedDateRule) {
                            recurrenceRule.ByDay.Add(new ByDay(standardTransitionStart.Value.DayOfWeek, standardTransitionStart.Value.Week));
                            recurrenceRule.ByMonth.Add(Convert.ToByte(standardTransitionStart.Value.Month)); // valid month values are from 1 to 12
                        }
                        if (adjustmentRule.DateEnd < DateTime.MaxValue.AddDays(-1)) {
                            recurrenceRule.EndDateTime = adjustmentRule.DateEnd;
                        }
                        this.Append("DTSTART", firstStandardDateTime, null);
                        this.Append("RRULE", recurrenceRule, timeZone);
                        this.AppendTimeZoneOffset("TZOFFSETFROM", timeZone.BaseUtcOffset + adjustmentRule.DaylightDelta);
                        this.AppendTimeZoneOffset("TZOFFSETTO", timeZone.BaseUtcOffset);
                        this.Append("END", "STANDARD");
                    }
                    if (isLastAdjustmentRule && adjustmentRule?.DateEnd < DateTime.MaxValue.AddDays(-1)) {
                        this.Append(timeZone, adjustmentRule.DateEnd);
                    }
                    isFirstAdjustmentRule = false;
                }
            }

            this.Append("END", "VTIMEZONE");
            return;
        }

        /// <summary>
        /// Appends a simple standard rule of a time zone.
        /// </summary>
        /// <param name="timeZone">time zone to append rule for</param>
        /// <param name="startDate">start date of rule</param>
        private void Append(TimeZoneInfo timeZone, DateTime startDate) {
            this.Append("BEGIN", "STANDARD");
            this.Append("DTSTART", startDate, null);
            this.AppendTimeZoneOffset("TZOFFSETFROM", timeZone.BaseUtcOffset);
            this.AppendTimeZoneOffset("TZOFFSETTO", timeZone.BaseUtcOffset);
            this.Append("END", "STANDARD");
            return;
        }

        /// <summary>
        /// Appends a description value.
        /// </summary>
        /// <param name="description">description value to append</param>
        public void AppendDescription(string description) {
            if (XmlUtility.IsXml(description)) {
                this.AppendText("DESCRIPTION", XmlUtility.RemoveTags(description));
                this.AppendText("X-ALT-DESC;FMTTYPE=text/html", description.Replace("\n", string.Empty));
            } else {
                this.AppendText("DESCRIPTION", description);
            }
            return;
        }

        /// <summary>
        /// Appends a pair of key and text value.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="value">text value to append</param>
        public void AppendText(string key, string value) {
            this.AppendText(key, new string[] { value });
            return;
        }

        /// <summary>
        /// Appends a pair of key and text values.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="values">text values to append</param>
        public void AppendText(string key, IEnumerable<string> values) {
            var cleanedValues = new List<string>(values);
            for (int i = 0; i < cleanedValues.Count; i++) {
                var value = cleanedValues[i];
                if (!string.IsNullOrEmpty(value)) {
                    cleanedValues[i] = value
                        .Replace("\\", "\\\\")
                        .Replace(",", "\\,")
                        .Replace(";", "\\;")
                        .Replace("\r", string.Empty)
                        .Replace("\n", "\\n");
                }
            }
            this.Append(key, cleanedValues);
            return;
        }

        /// <summary>
        /// Appends a pair of key and time zone offset.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="timeZoneOffset">time zone offset to append</param>
        private void AppendTimeZoneOffset(string key, TimeSpan timeZoneOffset) {
            var value = timeZoneOffset.ToString("hhmm");
            if (timeZoneOffset < TimeSpan.Zero) {
                value = "-" + value;
            } else {
                value = "+" + value;
            }
            this.Append(key, value);
            return;
        }

        /// <summary>
        /// Gets a day of week as string.
        /// </summary>
        /// <param name="dayOfWeek">day of week to get as string</param>
        /// <returns>day of week as string</returns>
        private static string GetDayOfWeekAsString(DayOfWeek dayOfWeek) {
            string value;
            if (DayOfWeek.Sunday == dayOfWeek) {
                value = "SU";
            } else if (DayOfWeek.Monday == dayOfWeek) {
                value = "MO";
            } else if (DayOfWeek.Tuesday == dayOfWeek) {
                value = "TU";
            } else if (DayOfWeek.Wednesday == dayOfWeek) {
                value = "WE";
            } else if (DayOfWeek.Thursday == dayOfWeek) {
                value = "TH";
            } else if (DayOfWeek.Friday == dayOfWeek) {
                value = "FR";
            } else if (DayOfWeek.Saturday == dayOfWeek) {
                value = "SA";
            } else {
                throw new ArgumentException("Day of week \"" + dayOfWeek.ToString() + "\" is unknown.");
            }
            return value;
        }

        /// <summary>
        /// Gets the string value of an item propery.
        /// </summary>
        /// <param name="sourceObject">presentable object to get
        /// string value from</param>
        /// <param name="sourceKey">key of field of presentable
        /// object to get value from</param>
        private static string GetStringValueOfItemProperty(IPresentableObject sourceObject, string sourceKey) {
            string value;
            var sourceField = sourceObject.FindPresentableField(sourceKey) as IPresentableFieldForElement;
            if (null == sourceField) {
                value = null;
            } else {
                value = sourceField.ValueAsString;
            }
            return value;
        }

        /// <summary>
        /// Gets the date/time for a transition time and a year based
        /// on the following documentation:
        /// https://docs.microsoft.com/de-de/dotnet/api/system.timezoneinfo.transitiontime.isfixeddaterule
        /// </summary>
        /// <param name="transitionTime">transition time to get
        /// date/time for</param>
        /// <param name="year">year to get date/time for</param>
        /// <returns>date/time for a transition time and a year</returns>
        private static DateTime GetTransitionDateTime(TransitionTime transitionTime, int year) {
            DateTime transitionDateTime;
            if (transitionTime.IsFixedDateRule) {
                transitionDateTime = new DateTime(year, transitionTime.Month, transitionTime.Day, transitionTime.TimeOfDay.Hour, transitionTime.TimeOfDay.Minute, transitionTime.TimeOfDay.Second, transitionTime.TimeOfDay.Millisecond);
            } else {
                var dayOfMonthOfStartOfWeek = transitionTime.Week * 7 - 6;
                var dayOfWeekOfFirstDayOfMonth = (int)new DateTime(year, transitionTime.Month, 1).DayOfWeek;
                int dayOfWeekOfChange = (int)transitionTime.DayOfWeek;
                int transitionDay;
                if (dayOfWeekOfFirstDayOfMonth <= dayOfWeekOfChange) {
                    transitionDay = dayOfMonthOfStartOfWeek + (dayOfWeekOfChange - dayOfWeekOfFirstDayOfMonth);
                } else {
                    transitionDay = dayOfMonthOfStartOfWeek + (7 - dayOfWeekOfFirstDayOfMonth + dayOfWeekOfChange);
                }
                if (transitionDay > DateTime.DaysInMonth(year, transitionTime.Month)) { // month has no fifth week
                    transitionDay -= 7;
                }
                transitionDateTime = new DateTime(year, transitionTime.Month, transitionDay, transitionTime.TimeOfDay.Hour, transitionTime.TimeOfDay.Minute, transitionTime.TimeOfDay.Second, transitionTime.TimeOfDay.Millisecond);
            }
            return transitionDateTime;
        }

        /// <summary>
        /// Gets the cleaned ID of a time zone.
        /// </summary>
        /// <param name="timeZone">time zone to get cleaned ID for</param>
        /// <returns>cleaned ID of time zone</returns>
        private string GetTzid(TimeZoneInfo timeZone) {
            return timeZone.Id.Replace(":", string.Empty);
        }

        /// <summary>
        /// Gets the value of an item propery.
        /// </summary>
        /// <param name="sourceObject">presentable object to get
        /// value from</param>
        /// <param name="sourceKey">key of field of presentable
        /// object to get value from</param>
        /// <typeparam name="T">type of value to get</typeparam>
        private static T GetValueOfItemProperty<T>(IPresentableObject sourceObject, string sourceKey) {
            T value;
            var sourceField = sourceObject.FindPresentableField(sourceKey) as IPresentableFieldForElement;
            if (null == sourceField) {
                value = default(T);
            } else {
                value = (T)sourceField.ValueAsObject;
            }
            return value;
        }

        /// <summary>
        /// Indicates whether calendar builder is for the supplied
        /// method.
        /// </summary>
        /// <param name="method">method to check</param>
        /// <returns>true if calendar builder is for the supplied
        /// method, false otherwise</returns>
        public bool IsFor(Method method) {
            return method == this.Method;
        }

        /// <summary>
        /// Indicates whether calendar builder is for one of the
        /// supplied methods.
        /// </summary>
        /// <param name="methods">methods to check</param>
        /// <returns>true if calendar builder is for one of the
        /// supplied methods, false otherwise</returns>
        public bool IsFor(IEnumerable<Method> methods) {
            bool isForMethod = false;
            foreach (var method in methods) {
                if (this.IsFor(method)) {
                    isForMethod = true;
                    break;
                }
            }
            return isForMethod;
        }

        /// <summary>
        /// Sets values to an item propery.
        /// </summary>
        /// <param name="targetField">field to copy values into</param>
        /// <param name="sourceObject">presentable object to copy
        /// values from</param>
        /// <param name="sourceKey">key of field of presentable
        /// object to copy values from</param>
        /// <typeparam name="T">type of values to be added</typeparam>
        private static void SetValuesToItemProperty<T>(IPresentableFieldForCollection<T> targetField, IPresentableObject sourceObject, string sourceKey) {
            var sourceField = sourceObject.FindPresentableField(sourceKey) as IPresentableFieldForCollection;
            if (null != sourceField) {
                foreach (var value in sourceField.GetValuesAsObject()) {
                    targetField.Add((T)value);
                }
            }
            return;
        }

        /// <summary>
        /// Converts the value of this instance to a string.
        /// </summary>
        /// <returns>value of this instance as string</returns>
        public override string ToString() {
            return this.stringBuilder.ToString();
        }

    }

}