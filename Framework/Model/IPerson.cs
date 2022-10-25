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
    using Persistence;
    using System;

    /// <summary>
    /// Interface for person with personal details.
    /// </summary>
    public interface IPerson : IPresentableObject {

        /// <summary>
        /// Day of birth.
        /// </summary>
        DateTime? Birthday { get; set; }

        /// <summary>
        /// City of address.
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// Company name.
        /// </summary>
        string Company { get; set; }

        /// <summary>
        /// Country of address.
        /// </summary>
        Country Country { get; set; }

        /// <summary>
        /// ISO 3166-1 alpha-2 country code of address.
        /// </summary>
        string CountryAlpha2Code { get; set; }

        /// <summary>
        /// Department of address.
        /// </summary>
        string Department { get; set; }

        /// <summary>
        /// Short description of user.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Name to be displayed.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// E-Mail address.
        /// </summary>
        string EmailAddress { get; set; }

        /// <summary>
        /// Facsimile number.
        /// </summary>
        string FaxNumber { get; set; }

        /// <summary>
        /// First name.
        /// </summary>
        string FirstName { get; set; }

        /// <summary>
        /// Number of home phone.
        /// </summary>
        string HomePhoneNumber { get; set; }

        /// <summary>
        /// House number of address.
        /// </summary>
        string HouseNumber { get; set; }

        /// <summary>
        /// Initials of name.
        /// </summary>
        string Initials { get; }

        /// <summary>
        /// Job title.
        /// </summary>
        string JobTitle { get; set; }

        /// <summary>
        /// Last name.
        /// </summary>
        string LastName { get; set; }

        /// <summary>
        /// Number of mobile phone.
        /// </summary>
        string MobilePhoneNumber { get; set; }

        /// <summary>
        /// "About me" notes.
        /// </summary>
        string Notes { get; set; }

        /// <summary>
        /// Name of office building.
        /// </summary>
        string Office { get; set; }

        /// <summary>
        /// Number of office phone.
        /// </summary>
        string OfficePhoneNumber { get; set; }

        /// <summary>
        /// Personal titles like academic titles or aristocratic
        /// titles.
        /// </summary>
        string PersonalTitle { get; set; }

        /// <summary>
        /// Thumbnail photo.
        /// </summary>
        ImageFile Photo { get; set; }

        /// <summary>
        /// P.O. box of address.
        /// </summary>
        string PostOfficeBox { get; set; }

        /// <summary>
        /// State of address.
        /// </summary>
        string State { get; set; }

        /// <summary>
        /// Street of address.
        /// </summary>
        string Street { get; set; }

        /// <summary>
        /// URL of web site.
        /// </summary>
        string WebSite { get; set; }

        /// <summary>
        /// Zip code of address.
        /// </summary>
        string ZipCode { get; set; }

    }

}