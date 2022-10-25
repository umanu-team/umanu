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

    using Diagnostics;
    using Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Mail;

    /// <summary>
    /// Represents an e-mail message.
    /// </summary>
    public abstract class Email : EmailTemplate {

        /// <summary>
        /// Event handler which is called after email was queued.
        /// </summary>
        public event EventHandler OnSent;

        /// <summary>
        /// Priority of this mail.
        /// </summary>
        public MailPriority? Priority {
            get { return (MailPriority?)this.priority.Value; }
            set { this.priority.Value = (int)value; }
        }
        private readonly PresentableFieldForNullableInt priority;

        /// <summary>
        /// Persons to reply to.
        /// </summary>
        public ICollection<IPerson> ReplyTo {
            get { return this.replyTo; }
        }
        private readonly PresentableFieldForEmailPersonCollection replyTo;

        /// <summary>
        /// Reply to addresses as string.
        /// </summary>
        public string ReplyToAsString {
            get { return Email.GetAddressLineFor(this.ReplyTo); }
        }

        /// <summary>
        /// Delivery status of email.
        /// </summary>
        public EmailStatus Status { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Email()
            : base() {
            this.priority = new PresentableFieldForNullableInt(this, nameof(this.Priority));
            this.AddPresentableField(this.priority);
            this.replyTo = new PresentableFieldForEmailPersonCollection(this, nameof(this.ReplyTo));
            this.AddPresentableField(this.replyTo);
            this.Status = EmailStatus.NotSent;
        }

        /// <summary>
        /// Initializes the mail message to be sent.
        /// </summary>
        /// <param name="mailMessage">mail message to be initialized</param>
        /// <param name="streamCollection">collection of streams to
        /// be disposed later</param>
        protected virtual void InitializeMailMessage(MailMessage mailMessage, DisposableCollection<MemoryStream> streamCollection) {
            var fromMailAddress = Email.GetMailAddressFor(this.From);
            if (null != fromMailAddress) {
                mailMessage.From = fromMailAddress;
            }
            foreach (var replyTo in this.ReplyTo) {
                var replyToMailAddress = Email.GetMailAddressFor(replyTo);
                if (null != replyToMailAddress && !mailMessage.ReplyToList.Contains(replyToMailAddress)) {
                    mailMessage.ReplyToList.Add(replyToMailAddress);
                }
            }
            foreach (var to in this.To) {
                var toMailAddress = Email.GetMailAddressFor(to);
                if (null != toMailAddress && !mailMessage.To.Contains(toMailAddress)) {
                    mailMessage.To.Add(toMailAddress);
                }
            }
            foreach (var cc in this.Cc) {
                var ccMailAddress = Email.GetMailAddressFor(cc);
                if (null != ccMailAddress && !mailMessage.To.Contains(ccMailAddress) && !mailMessage.CC.Contains(ccMailAddress)) {
                    mailMessage.CC.Add(ccMailAddress);
                }
            }
            foreach (var bcc in this.Bcc) {
                var bccMailAddress = Email.GetMailAddressFor(bcc);
                if (null != bccMailAddress && !mailMessage.To.Contains(bccMailAddress) && !mailMessage.CC.Contains(bccMailAddress) && !mailMessage.Bcc.Contains(bccMailAddress)) {
                    mailMessage.Bcc.Add(bccMailAddress);
                }
            }
            if (this.Priority.HasValue) {
                mailMessage.Priority = this.Priority.Value;
            }
            mailMessage.Subject = this.Subject;
            mailMessage.Body = this.BodyText;
            foreach (var attachment in this.Attachments) {
                var attachmentStream = new MemoryStream(attachment.Bytes);
                streamCollection.Add(attachmentStream);
                mailMessage.Attachments.Add(new Attachment(attachmentStream, attachment.Name, attachment.MimeType));
            }
            return;
        }

        /// <summary>
        /// Prepares this email to be sent.
        /// </summary>
        private void PrepareForSending() {
            if (EmailStatus.NotSent == this.Status) {
                this.Status = EmailStatus.Pending;
            } else {
                throw new InvalidOperationException("Emails can only be sent once and this email has been sent already.");
            }
            this.RetrieveUsers();
            this.ReplacePlaceholders();
            return;
        }

        /// <summary>
        /// Replaces placeholders in a text like [Sender.FirstName]
        /// or [Receiver.LastName] by the actual values of a user.
        /// Possible values are defined in
        /// Framework.Persistence.Directories.Field plus
        /// &quot;CountryName&quot;.
        /// </summary>
        /// <param name="additionalPlaceholders">pairs of additional
        /// keys and values to be replaced in subject and body text</param>
        public void ReplacePlaceholders(IEnumerable<KeyValuePair<string, string>> additionalPlaceholders) {
            base.ReplacePlaceholders();
            if (null != additionalPlaceholders) {
                foreach (var additionalPlaceholder in additionalPlaceholders) {
                    if (null != this.Subject) {
                        this.Subject = this.Subject.Replace(additionalPlaceholder.Key, additionalPlaceholder.Value);
                    }
                    if (null != this.BodyText) {
                        this.BodyText = this.BodyText.Replace(additionalPlaceholder.Key, additionalPlaceholder.Value);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Retrieves all senders and receivers of the email message.
        /// This is important because the parent user directory might
        /// be disposed already by the time the mail is sent.
        /// </summary>
        private void RetrieveUsers() {
            Email.GetMailAddressFor(this.From);
            foreach (var replyTo in this.ReplyTo) {
                Email.GetMailAddressFor(replyTo);
            }
            foreach (var to in this.To) {
                Email.GetMailAddressFor(to);
            }
            foreach (var cc in this.Cc) {
                Email.GetMailAddressFor(cc);
            }
            foreach (var bcc in this.Bcc) {
                Email.GetMailAddressFor(bcc);
            }
            return;
        }

        /// <summary>
        /// Sends out this e-mail asynchronously.
        /// </summary>
        public void SendAsync() {
            this.SendAsync(null);
            return;
        }

        /// <summary>
        /// Sends out this e-mail asynchronously.
        /// </summary>
        /// <param name="settings">settings to use for SMTP client</param>
        public void SendAsync(ISmtpSettings settings) {
            this.PrepareForSending();
            JobQueue.EnqueueAtBeginning(delegate () {
                this.SendUnsafe(settings);
            });
            return;
        }

        /// <summary>
        /// Sends an email message asynchronously.
        /// </summary>
        /// <param name="from">sender</param>
        /// <param name="to">primary receivers</param>
        /// <param name="cc">copy receivers</param>
        /// <param name="subject">subject of mail</param>
        /// <param name="bodyText">body text of e-mail</param>
        /// <param name="additionalPlaceholders">pairs of additional
        /// keys and values to be replaced in subject and body text</param>
        /// <param name="priority">priority of mail</param>
        /// <typeparam name="T">type of email to send</typeparam>
        protected static void SendAsync<T>(IPerson from, IEnumerable<IPerson> to, IEnumerable<IPerson> cc, string subject, string bodyText, IEnumerable<KeyValuePair<string, string>> additionalPlaceholders, MailPriority priority) where T : Email, new() {
            var email = new T {
                From = from
            };
            if (null != to) {
                foreach (var toReceiver in to) {
                    email.To.Add(toReceiver);
                }
            }
            if (null != cc) {
                foreach (var ccReceiver in cc) {
                    email.Cc.Add(ccReceiver);
                }
            }
            email.Priority = priority;
            email.Subject = subject;
            email.BodyText = bodyText;
            email.ReplacePlaceholders(additionalPlaceholders);
            email.SendAsync();
            return;
        }

        /// <summary>
        /// Sends out this e-mail synchronously. This should only be
        /// used within queued jobs or timer jobs. Please use
        /// SendAsync() in all other cases.
        /// </summary>
        public void SendSynchronously() {
            this.SendSynchronously(null);
            return;
        }

        /// <summary>
        /// Sends out this e-mail synchronously. This should only be
        /// used within queued jobs or timer jobs. Please use
        /// SendAsync(ISmtpSettings) in all other cases.
        /// </summary>
        /// <param name="settings">settings to use for SMTP client</param>
        public void SendSynchronously(ISmtpSettings settings) {
            this.PrepareForSending();
            this.SendUnsafe(settings);
            return;
        }

        /// <summary>
        /// Sends this email that has to be prepared for sending
        /// already.
        /// </summary>
        /// <param name="settings">settings to use for SMTP client</param>
        private void SendUnsafe(ISmtpSettings settings) {
            using (var smtpClient = new SmtpClient()) {
                if (null != settings) {
                    smtpClient.Host = settings.SmtpHost;
                    smtpClient.Port = settings.SmtpPort;
                    smtpClient.EnableSsl = settings.SmtpEnableSsl;
                    if (!string.IsNullOrEmpty(settings.SmtpUserName) && !string.IsNullOrEmpty(settings.SmtpPassword)) {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(settings.SmtpUserName, settings.SmtpPassword);
                    }
                }
                using (var mailMessage = new MailMessage()) {
                    using (var streamCollection = new DisposableCollection<MemoryStream>()) {
                        this.InitializeMailMessage(mailMessage, streamCollection);
                        if (!string.IsNullOrEmpty(settings?.SmtpFrom) && null == mailMessage.From) {
                            mailMessage.From = new MailAddress(settings.SmtpFrom);
                        }
                        if (mailMessage.To.Count > 0 || mailMessage.CC.Count > 0 || mailMessage.Bcc.Count > 0) {
                            try {
                                smtpClient.Send(mailMessage);
                                this.Status = EmailStatus.Sent;
                                this.OnSent?.Invoke(this, EventArgs.Empty);
                            } catch (Exception exception) {
                                if (null != JobQueue.Log) {
                                    string receivers = string.Empty;
                                    foreach (var receiver in mailMessage.To) {
                                        if (receivers.Length > 0) {
                                            receivers += ", ";
                                        }
                                        receivers += receiver.Address;
                                    }
                                    foreach (var receiver in mailMessage.CC) {
                                        if (receivers.Length > 0) {
                                            receivers += ", ";
                                        }
                                        receivers += receiver.Address;
                                    }
                                    foreach (var receiver in mailMessage.Bcc) {
                                        if (receivers.Length > 0) {
                                            receivers += ", ";
                                        }
                                        receivers += receiver.Address;
                                    }
                                    JobQueue.Log?.WriteEntry("Attempt to send an email to the following receivers failed: " + receivers + Environment.NewLine + exception.ToString(), LogLevel.Error);
                                }
                                throw;
                            }
                        }
                    }
                }
            }
            return;
        }

    }

}