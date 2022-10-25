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

namespace Framework.BusinessApplications {

    using Framework.Persistence;
    using Framework.Persistence.Fields;

    /// <summary>
    /// Settings to use for SAP connection.
    /// </summary>
    public sealed class SapConnectionSettings : PersistentObject {

        /// <summary>
        /// SAP connection tennant.
        /// </summary>
        public string Client {
            get { return this.client.Value; }
            set { this.client.Value = value; }
        }
        private readonly PersistentFieldForString client =
            new PersistentFieldForString(nameof(Client));

        /// <summary>
        /// SAP connection language.
        /// </summary>
        public string Language {
            get { return this.language.Value; }
            set { this.language.Value = value; }
        }
        private readonly PersistentFieldForString language =
            new PersistentFieldForString(nameof(Language));

        /// <summary>
        /// ERP Connect license key.
        /// </summary>
        public string LicenceKey {
            get { return this.licenceKey.Value; }
            set { this.licenceKey.Value = value; }
        }
        private readonly PersistentFieldForString licenceKey =
            new PersistentFieldForString(nameof(LicenceKey));

        /// <summary>
        /// ERP Connect logon group.
        /// </summary>
        public string LogonGroup {
            get { return this.logonGroup.Value; }
            set { this.logonGroup.Value = value; }
        }
        private readonly PersistentFieldForString logonGroup =
            new PersistentFieldForString(nameof(LogonGroup));

        /// <summary>
        /// ERP Connect message server.
        /// </summary>
        public string MessageServer {
            get { return this.messageServer.Value; }
            set { this.messageServer.Value = value; }
        }
        private readonly PersistentFieldForString messageServer =
            new PersistentFieldForString(nameof(MessageServer));

        /// <summary>
        /// SAP connection password.
        /// </summary>
        public string Password {
            get { return this.password.Value; }
            set { this.password.Value = value; }
        }
        private readonly PersistentFieldForString password =
            new PersistentFieldForString(nameof(Password));

        /// <summary>
        /// SAP client protocol (RFC, NWRFC or HttpSoap).
        /// </summary>
        public string Protocol {
            get { return this.protocol.Value; }
            set { this.protocol.Value = value; }
        }
        private readonly PersistentFieldForString protocol =
            new PersistentFieldForString(nameof(Protocol));

        /// <summary>
        /// ERP Connect SID.
        /// </summary>
        public string Sid {
            get { return this.sid.Value; }
            set { this.sid.Value = value; }
        }
        private readonly PersistentFieldForString sid =
            new PersistentFieldForString(nameof(Sid));

        /// <summary>
        /// SAP connection user name.
        /// </summary>
        public string UserName {
            get { return this.userName.Value; }
            set { this.userName.Value = value; }
        }
        private readonly PersistentFieldForString userName =
            new PersistentFieldForString(nameof(UserName));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public SapConnectionSettings()
            : base() {
            this.RegisterPersistentField(this.client);
            this.RegisterPersistentField(this.language);
            this.RegisterPersistentField(this.licenceKey);
            this.RegisterPersistentField(this.logonGroup);
            this.RegisterPersistentField(this.messageServer);
            this.RegisterPersistentField(this.password);
            this.Protocol = "NWRFC";
            this.RegisterPersistentField(this.protocol);
            this.RegisterPersistentField(this.sid);
            this.RegisterPersistentField(this.userName);
        }

    }

}