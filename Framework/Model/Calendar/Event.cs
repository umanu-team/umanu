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
    using Presentation;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Represents an iCalendar calendar event as defined in RFC5545.
    /// </summary>
    public class Event : PersistentObject, IComparable<Event>, IProvidableObject {

        // TODO: Add REQUEST-STATUS.

        /// <summary>
        /// Alarms (VALARM).
        /// Specifies alarm triggers with respect to the start or end
        /// of the event.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<Alarm> Alarms { get; private set; }

        /// <summary>
        /// Attachments (ATTACH).
        /// This property provides the capability to associate a
        /// document object with an event.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<File> Attachments { get; private set; }

        /// <summary>
        /// Attendees (ATTENDEE).
        /// This property defines an "Attendee" within a calendar
        /// component.
        /// </summary>
        public PersistentFieldForIUserCollection Attendees { get; private set; }

        /// <summary>
        /// Categories (CATEGORIES).
        /// This property defines the categories for a calendar
        /// component.
        /// </summary>
        public PersistentFieldForStringCollection Categories { get; private set; }

        /// <summary>
        /// Classification (CLASS).
        /// This property defines the access classification for a
        /// calendar component.
        /// </summary>
        public Classification? Classification {
            get { return (Classification?)this.classification.Value; }
            set { this.classification.Value = (int?)value; }
        }
        private readonly PersistentFieldForNullableInt classification =
            new PersistentFieldForNullableInt(nameof(Classification), (int)Model.Classification.Public);

        /// <summary>
        /// Comments (COMMENT).
        /// This property specifies non-processing information
        /// intended to provide a comment to the calendar user.
        /// </summary>
        public PersistentFieldForStringCollection Comments { get; private set; }

        /// <summary>
        /// Contacts (CONTACT).
        /// This property is used to represent contact information or
        /// alternately a reference to contact information associated
        /// with the event.
        /// </summary>
        public Group Contacts {
            get { return this.contacts.Value; }
            set { this.contacts.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<Group> contacts =
            new PersistentFieldForPersistentObject<Group>(nameof(Contacts), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Description (DESCRIPTION).
        /// This property provides a more complete description of the
        /// calendar component than that provided by the "SUMMARY"
        /// property.
        /// </summary>
        public string Description {
            get { return this.description.Value; }
            set { this.description.Value = value; }
        }
        private readonly PersistentFieldForString description =
            new PersistentFieldForString(nameof(Description));

        /// <summary>
        /// Duration of event (DURATION).
        /// </summary>
        public TimeSpan? Duration {
            get {
                TimeSpan? duration;
                if (this.StartDateTime.HasValue && this.EndDateTime.HasValue) {
                    duration = this.EndDateTime.Value - this.StartDateTime.Value;
                } else {
                    duration = null;
                }
                return duration;
            }
        }

        /// <summary>
        /// Date-Time End (DTEND).
        /// This property defines the date and time by which the
        /// event ends.
        /// </summary>
        public DateTime? EndDateTime {
            get { return this.endDateTime.Value; }
            set { this.endDateTime.Value = value; }
        }
        private readonly PersistentFieldForNullableDateTime endDateTime =
            new PersistentFieldForNullableDateTime(nameof(EndDateTime));

        /// <summary>
        /// Exception date/times (EXDATE).
        /// Defines the list of DATE-TIME exceptions for recurring
        /// events.
        /// </summary>
        public PersistentFieldForDateTimeCollection ExceptionDateTimes { get; private set; }

        /// <summary>
        /// Indicates whether event has recurrence.
        /// </summary>
        public bool HasRecurrence {
            get { return true == this.RecurrenceRule?.HasValue || this.RecurrenceDateTimes.Count > 0; }
        }

        /// <summary>
        /// Indicates whether a field value is changed that is
        /// relevant for the sequence number.
        /// </summary>
        public bool IsSequenceNumberRelevantFieldChanged {
            get { return this.sequenceNumber.IsChanged; }
        }

        /// <summary>
        /// Location (LOCATION/GEO).
        /// This property defines the intended venue for the activity
        /// defined by a calendar component.
        /// </summary>
        public Location Location {
            get { return this.location.Value; }
            set { this.location.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<Location> location =
            new PersistentFieldForPersistentObject<Location>(nameof(Location), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Organizer (ORGANIZER).
        /// This property is specifies the organizer of a
        /// group-scheduled calendar entity.
        /// </summary>
        public IUser Organizer {
            get { return this.organizer.Value; }
            set { this.organizer.Value = value; }
        }
        private readonly PersistentFieldForIUser organizer =
            new PersistentFieldForIUser(nameof(Organizer));

        /// <summary>
        /// Priority (PRIORITY).
        /// This property defines the relative priority.
        /// </summary>
        public Priority? Priority {
            get { return (Priority?)this.priority.Value; }
            set { this.priority.Value = (int?)value; }
        }
        private readonly PersistentFieldForNullableInt priority =
            new PersistentFieldForNullableInt(nameof(Priority), (int)Model.Priority.NotDefined);

        /// <summary>
        /// Recurrence date/times (RDATE).
        /// Defines the list of DATE-TIME values for recurring
        /// events.
        /// </summary>
        public PersistentFieldForDateTimeCollection RecurrenceDateTimes { get; private set; }

        /// <summary>
        /// Recurrence ID (RECURRENCE-ID).
        /// This property is used in conjunction with the Id (UID)
        /// and SequenceNumber (SEQUENCE) properties to identify a
        /// specific instance of a recurring calendar component.
        /// The property value is the original value of the
        /// StartDateTime (DTSTART) property of the recurrence
        /// instance.
        /// </summary>
        public DateTime? RecurrenceId {
            get { return this.recurrenceId.Value; }
            set { this.recurrenceId.Value = value; }
        }
        private readonly PersistentFieldForNullableDateTime recurrenceId =
            new PersistentFieldForNullableDateTime(nameof(RecurrenceId));

        /// <summary>
        /// Recurrence rule (RRULE).
        /// Defines a rule or repeating pattern for recurring events.
        /// </summary>
        public RecurrenceRule RecurrenceRule {
            get { return this.recurrenceRule.Value; }
            set { this.recurrenceRule.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<RecurrenceRule> recurrenceRule =
            new PersistentFieldForPersistentObject<RecurrenceRule>(nameof(RecurrenceRule), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Resources (RESOURCES).
        /// This property defines the equipment or resources anticipated for
        /// an activity specified by a calendar component.
        /// </summary>
        public PersistentFieldForStringCollection ResourceCollection { get; private set; }

        /// <summary>
        /// Sequence number (SEQUENCE).
        /// When a calendar component is created, its sequence
        /// number is 0.  It is monotonically incremented by the Organizer's
        /// CUA each time the Organizer makes a significant revision to the
        /// calendar component.
        /// </summary>
        public uint SequenceNumber {
            get { return this.sequenceNumber.Value; }
            set { this.sequenceNumber.Value = value; }
        }
        private readonly PersistentFieldForUInt sequenceNumber =
            new PersistentFieldForUInt(nameof(SequenceNumber), 0);

        /// <summary>
        /// Date-Time Start (DTSTART).
        /// Within the VEVENT calendar component, this property
        /// defines the start date and time for the event.
        /// </summary>
        public DateTime? StartDateTime {
            get { return this.startDateTime.Value; }
            set { this.startDateTime.Value = value; }
        }
        private readonly PersistentFieldForNullableDateTime startDateTime =
            new PersistentFieldForNullableDateTime(nameof(StartDateTime));

        /// <summary>
        /// Status (STATUS).
        /// This property defines the overall status or confirmation.
        /// </summary>
        public EventStatus? Status {
            get { return (EventStatus?)this.status.Value; }
            set { this.status.Value = (int?)value; }
        }
        private readonly PersistentFieldForNullableInt status =
            new PersistentFieldForNullableInt(nameof(Status), (int)EventStatus.Confirmed);

        /// <summary>
        /// Time Transparency (TRANSP).
        /// Characteristic of an event that determines whether it
        /// appears to consume time on a calendar.
        /// </summary>
        public TimeTransparency? TimeTransparency {
            get { return (TimeTransparency?)this.timeTransparency.Value; }
            set { this.timeTransparency.Value = (int?)value; }
        }
        private readonly PersistentFieldForNullableInt timeTransparency =
            new PersistentFieldForNullableInt(nameof(TimeTransparency), (int)Calendar.TimeTransparency.Opaque);

        /// <summary>
        /// Time zone to be used for calculation of recurrent dates.
        /// </summary>
        public TimeZoneInfo TimeZone {
            get {
                var systemTimeZone = Event.timeZoneCache.GetOrAdd(this.timeZone.Value, delegate (string key) {
                    try {
                        return TimeZoneInfo.FindSystemTimeZoneById(key);
                    } catch (TimeZoneNotFoundException) {
                        return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id);
                    }
                });
                return systemTimeZone;
            }
            set {
                this.timeZone.Value = value.Id;
            }
        }
        private readonly PersistentFieldForString timeZone =
            new PersistentFieldForString(nameof(TimeZone), TimeZoneInfo.Local.Id);

        /// <summary>
        /// Cache of system time zones.
        /// </summary>
        private static readonly ConcurrentDictionary<string, TimeZoneInfo> timeZoneCache = new ConcurrentDictionary<string, TimeZoneInfo>();

        /// <summary>
        /// Title (SUMMARY).
        /// This property defines a short summary or subject for the
        /// calendar component.
        /// </summary>
        public string Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PersistentFieldForString title =
            new PersistentFieldForString(nameof(Title));

        /// <summary>
        /// Uniform Resource Locator (URL).
        /// This property may be used in a calendar component to
        /// convey a location where a more dynamic rendition of the calendar
        /// information associated with the calendar component can be found.
        /// </summary>
        public string WebSite {
            get { return this.webSite.Value; }
            set { this.webSite.Value = value; }
        }
        private readonly PersistentFieldForString webSite =
            new PersistentFieldForString(nameof(WebSite));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Event()
            : base() {
            this.Alarms = new PersistentFieldForPersistentObjectCollection<Alarm>(nameof(this.Alarms), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.Alarms);
            this.Attachments = new PersistentFieldForPersistentObjectCollection<File>(nameof(this.Attachments), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.Attachments);
            this.Attendees = new PersistentFieldForIUserCollection(nameof(this.Attendees));
            this.RegisterPersistentField(this.Attendees);
            this.Categories = new PersistentFieldForStringCollection(nameof(this.Categories));
            this.RegisterPersistentField(this.Categories);
            this.RegisterPersistentField(this.classification);
            this.Comments = new PersistentFieldForStringCollection(nameof(this.Comments));
            this.RegisterPersistentField(this.Comments);
            this.Contacts = new Group("Event Contacts");
            this.RegisterPersistentField(this.contacts);
            this.RegisterPersistentField(this.description);
            this.RegisterPersistentField(this.endDateTime);
            this.ExceptionDateTimes = new PersistentFieldForDateTimeCollection(nameof(this.ExceptionDateTimes));
            this.RegisterPersistentField(this.ExceptionDateTimes);
            this.RegisterPersistentField(this.location);
            this.RegisterPersistentField(this.organizer);
            this.RegisterPersistentField(this.priority);
            this.RecurrenceDateTimes = new PersistentFieldForDateTimeCollection(nameof(this.RecurrenceDateTimes));
            this.RegisterPersistentField(this.RecurrenceDateTimes);
            this.RegisterPersistentField(this.recurrenceId);
            this.RegisterPersistentField(this.recurrenceRule);
            this.ResourceCollection = new PersistentFieldForStringCollection(nameof(this.ResourceCollection));
            this.RegisterPersistentField(this.ResourceCollection);
            this.RegisterPersistentField(this.sequenceNumber);
            this.RegisterPersistentField(this.startDateTime);
            this.RegisterPersistentField(this.status);
            this.RegisterPersistentField(this.timeTransparency);
            this.RegisterPersistentField(this.timeZone);
            this.RegisterPersistentField(this.title);
            this.RegisterPersistentField(this.webSite);
            this.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                if (nameof(Event.Attachments) == e.PropertyName
                || nameof(Event.Description) == e.PropertyName
                || nameof(Event.Duration) == e.PropertyName
                || nameof(Event.EndDateTime) == e.PropertyName
                || nameof(Event.ExceptionDateTimes) == e.PropertyName
                || nameof(Event.Location) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.Attachments)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.City)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.Contacts)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.Country)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.Description)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.HouseNumber)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.OpeningHours)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.Photo)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.PostOfficeBox)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.State)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.Street)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.Title)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.WebSite)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.Location), nameof(Model.Location.ZipCode)) == e.PropertyName
                || nameof(Event.RecurrenceRule) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.ByDay)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.ByHour)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.ByMinute)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.ByMonth)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.ByMonthDay)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.BySecond)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.BySetPos)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.ByWeekNumber)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.ByYearDay)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.Count)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.EndDateTime)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.Frequency)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.Interval)) == e.PropertyName
                || KeyChain.ConcatToKey(nameof(Event.RecurrenceRule), nameof(Calendar.RecurrenceRule.WeekStart)) == e.PropertyName
                || nameof(Event.RecurrenceDateTimes) == e.PropertyName
                || nameof(Event.StartDateTime) == e.PropertyName
                || nameof(Event.Status) == e.PropertyName
                || nameof(Event.Title) == e.PropertyName) {
                    if (!this.IsNew && !this.IsSequenceNumberRelevantFieldChanged) {
                        this.SequenceNumber++;
                    }
                }
            };
        }

        /// <summary>
        /// Compares the current instance with another object of the
        /// same type by start date and end date.
        /// </summary>
        /// <param name="other">An object to compare with this
        /// instance.</param>
        /// <returns>A signed integer that indicates the relative
        /// order of the comparands: Less than zero if this instance
        /// is less than the other instance. Equal to zero if this
        /// instance is equal to the other instance. Greater than
        /// zero if this instance is greater than the other instance.</returns>
        public int CompareTo(Event other) {
            int result;
            if (null == other) {
                result = 1;
            } else {
                if (this.StartDateTime.HasValue && other.StartDateTime.HasValue) {
                    result = this.StartDateTime.Value.CompareTo(other.StartDateTime.Value);
                } else if (!this.StartDateTime.HasValue && !other.StartDateTime.HasValue) {
                    result = 0;
                } else if (this.StartDateTime.HasValue && !other.StartDateTime.HasValue) {
                    result = 1;
                } else {
                    result = -1;
                }
                if (0 == result) {
                    if (this.EndDateTime.HasValue && other.EndDateTime.HasValue) {
                        result = this.EndDateTime.Value.CompareTo(other.StartDateTime.Value);
                    } else if (!this.EndDateTime.HasValue && !other.EndDateTime.HasValue) {
                        result = 0;
                    } else if (this.EndDateTime.HasValue && !other.EndDateTime.HasValue) {
                        result = 1;
                    } else {
                        result = -1;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Converts this event from UTC to local time zone.
        /// </summary>
        public void ConvertFromUtc() {
            if (this.EndDateTime.HasValue) {
                this.EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(this.EndDateTime.Value, this.TimeZone);
            }
            for (int i = 0; i < this.ExceptionDateTimes.Count; i++) {
                var exceptionDataTime = this.ExceptionDateTimes[i];
                if (null != exceptionDataTime) {
                    this.ExceptionDateTimes[i] = TimeZoneInfo.ConvertTimeFromUtc(exceptionDataTime, this.TimeZone);
                }
            }
            for (int i = 0; i < this.RecurrenceDateTimes.Count; i++) {
                var recurrenceDateTime = this.RecurrenceDateTimes[i];
                if (null != recurrenceDateTime) {
                    this.RecurrenceDateTimes[i] = TimeZoneInfo.ConvertTimeFromUtc(recurrenceDateTime, this.TimeZone);
                }
            }
            if (null != this.RecurrenceRule && this.RecurrenceRule.EndDateTime.HasValue) {
                this.RecurrenceRule.EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(this.RecurrenceRule.EndDateTime.Value, this.TimeZone);
            }
            if (this.StartDateTime.HasValue) {
                this.StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(this.StartDateTime.Value, this.TimeZone);
            }
            return;
        }

        /// <summary>
        /// Converts this event to UTC from local time zone.
        /// </summary>
        public void ConvertToUtc() {
            if (this.EndDateTime.HasValue) {
                this.EndDateTime = TimeZoneInfo.ConvertTimeToUtc(this.EndDateTime.Value, this.TimeZone);
            }
            for (int i = 0; i < this.ExceptionDateTimes.Count; i++) {
                var exceptionDataTime = this.ExceptionDateTimes[i];
                if (null != exceptionDataTime) {
                    this.ExceptionDateTimes[i] = TimeZoneInfo.ConvertTimeToUtc(exceptionDataTime, this.TimeZone);
                }
            }
            for (int i = 0; i < this.RecurrenceDateTimes.Count; i++) {
                var recurrenceDateTime = this.RecurrenceDateTimes[i];
                if (null != recurrenceDateTime) {
                    this.RecurrenceDateTimes[i] = TimeZoneInfo.ConvertTimeToUtc(recurrenceDateTime, this.TimeZone);
                }
            }
            if (null != this.RecurrenceRule && this.RecurrenceRule.EndDateTime.HasValue) {
                this.RecurrenceRule.EndDateTime = TimeZoneInfo.ConvertTimeToUtc(this.RecurrenceRule.EndDateTime.Value, this.TimeZone);
            }
            if (this.StartDateTime.HasValue) {
                this.StartDateTime = TimeZoneInfo.ConvertTimeToUtc(this.StartDateTime.Value, this.TimeZone);
            }
            return;
        }

        /// <summary>
        /// Gets all date/times matching the recurrence rule.
        /// However, event needs to be converted from UTC before and
        /// back to UTC afterwards.
        /// </summary>
        /// <returns>all date/times matching the recurrence rule</returns>
        public IEnumerable<DateTime> GetDateTimesIgnoreTimeZone() {
            if (this.StartDateTime.HasValue) {
                var recurrenceDateTimes = this.RecurrenceDateTimes.ToArray();
                Array.Sort<DateTime>(recurrenceDateTimes);
                long indexOfRecurrenceDateTimes = 0;
                if (true == this.RecurrenceRule?.HasValue) {
                    foreach (var recurrenceRuleDateTime in this.RecurrenceRule.GetDateTimes(this.StartDateTime.Value)) {
                        while (indexOfRecurrenceDateTimes < recurrenceDateTimes.LongLength && recurrenceDateTimes[indexOfRecurrenceDateTimes] < recurrenceRuleDateTime) {
                            if (!this.ExceptionDateTimes.Contains(recurrenceDateTimes[indexOfRecurrenceDateTimes])) {
                                yield return recurrenceDateTimes[indexOfRecurrenceDateTimes];
                            }
                            indexOfRecurrenceDateTimes++;
                        }
                        if (!this.ExceptionDateTimes.Contains(recurrenceRuleDateTime)) {
                            yield return recurrenceRuleDateTime;
                        }
                    }
                } else {
                    if (!this.ExceptionDateTimes.Contains(this.StartDateTime.Value)) {
                        yield return this.StartDateTime.Value;
                    }
                }
                while (indexOfRecurrenceDateTimes < this.RecurrenceDateTimes.Count) {
                    if (!this.ExceptionDateTimes.Contains(recurrenceDateTimes[indexOfRecurrenceDateTimes])) {
                        yield return recurrenceDateTimes[indexOfRecurrenceDateTimes];
                    }
                    indexOfRecurrenceDateTimes++;
                }
            }
        }

        /// <summary>
        /// Gets all events as non recurrent shallow copies in UTC.
        /// </summary>
        /// <param name="minDateTime">minimum date/time</param>
        /// <param name="maxDateTime">maximum date/time</param>
        /// <returns>all events as non-recurrent shallow copies in
        /// UTC</returns>
        public IEnumerable<Event> GetFlatEvents(DateTime minDateTime, DateTime maxDateTime) {
            Event sourceEvent;
            if (!this.StartDateTime.HasValue || this.StartDateTime.Value.Kind != DateTimeKind.Utc) {
                sourceEvent = this;
            } else {
                sourceEvent = new Event();
                sourceEvent.CopyFrom(this, CopyBehaviorForAllowedGroups.ShallowCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.ShallowCopy);
                sourceEvent.Id = this.Id;
                sourceEvent.ConvertFromUtc();
            }
            TimeSpan? duration;
            if (sourceEvent.EndDateTime.HasValue) {
                duration = sourceEvent.EndDateTime - sourceEvent.StartDateTime;
            } else {
                duration = null;
            }
            foreach (var dateTime in sourceEvent.GetDateTimesIgnoreTimeZone()) {
                if (dateTime > maxDateTime) {
                    break;
                } else if (dateTime > minDateTime) {
                    var flatEvent = new Event {
                        Alarms = sourceEvent.Alarms,
                        AllowedGroups = sourceEvent.AllowedGroups,
                        AllowedGroupsCascadedRemovalBehavior = sourceEvent.AllowedGroupsCascadedRemovalBehavior,
                        Attachments = sourceEvent.Attachments,
                        Attendees = sourceEvent.Attendees,
                        Categories = sourceEvent.Categories,
                        Classification = sourceEvent.Classification,
                        Comments = sourceEvent.Comments,
                        Contacts = sourceEvent.Contacts,
                        Description = sourceEvent.Description
                    };
                    if (duration.HasValue) {
                        flatEvent.EndDateTime = dateTime.Add(duration.Value);
                    }
                    flatEvent.Id = sourceEvent.Id;
                    flatEvent.Location = sourceEvent.Location;
                    flatEvent.Organizer = sourceEvent.Organizer;
                    flatEvent.Priority = sourceEvent.Priority;
                    flatEvent.ResourceCollection = sourceEvent.ResourceCollection;
                    flatEvent.StartDateTime = dateTime;
                    flatEvent.Status = sourceEvent.Status;
                    flatEvent.TimeTransparency = sourceEvent.TimeTransparency;
                    flatEvent.TimeZone = sourceEvent.TimeZone;
                    flatEvent.Title = sourceEvent.Title;
                    flatEvent.WebSite = sourceEvent.WebSite;
                    flatEvent.ConvertToUtc();
                    yield return flatEvent;
                }
            }
        }

        /// <summary>
        /// Gets all events as non recurrent shallow copies in a
        /// specified time zone.
        /// </summary>
        /// <param name="minDateTime">minimum date/time</param>
        /// <param name="maxDateTime">maximum date/time</param>
        /// <param name="destinationTimeZone">specifies the
        /// destination time zone to convert events to</param>
        /// <returns>all events as non-recurrent shallow copies in
        /// specified destination time zone</returns>
        public IEnumerable<Event> GetFlatEvents(DateTime minDateTime, DateTime maxDateTime, TimeZoneInfo destinationTimeZone) {
            foreach (var flatEvent in this.GetFlatEvents(minDateTime, maxDateTime)) {
                flatEvent.TimeZone = destinationTimeZone;
                flatEvent.ConvertFromUtc();
                yield return flatEvent;
            }
        }

        /// <summary>
        /// Gets the latest recurrence of event.
        /// </summary>
        /// <returns>latest recurrence of event</returns>
        public DateTime GetLatestRecurrence() {
            var latestRecurrence = this.StartDateTime.Value;
            if (this.EndDateTime.HasValue && this.EndDateTime.Value > latestRecurrence) {
                latestRecurrence = this.EndDateTime.Value;
            }
            if (null != this.RecurrenceRule) {
                if (this.RecurrenceRule.EndDateTime.HasValue && this.RecurrenceRule.EndDateTime.Value > latestRecurrence) {
                    latestRecurrence = this.RecurrenceRule.EndDateTime.Value;
                } else {
                    latestRecurrence = UtcDateTime.MaxValue;
                }
            }
            if (latestRecurrence < UtcDateTime.MaxValue) {
                foreach (var recurrentDateTime in this.RecurrenceDateTimes) {
                    if (recurrentDateTime > latestRecurrence) {
                        latestRecurrence = recurrentDateTime;
                    }
                }
            }
            return latestRecurrence;
        }

        /// <summary>
        /// Gets the title of providable object.
        /// </summary>
        public string GetTitle() {
            return this.Title;
        }

    }

}