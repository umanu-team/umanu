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

namespace Framework.Model.Units {

    /// <summary>
    /// Represents a weight as number with any unit.
    /// </summary>
    public class Weight : NumberWithAnyUnit {

        /// <summary>
        /// Weight in centigrams (cg).
        /// </summary>
        public decimal? InCentigrams {
            get { return this.ConvertValueToUnit(Mass.InCentigrams); }
        }

        /// <summary>
        /// Weight in decagrams (dag).
        /// </summary>
        public decimal? InDecagrams {
            get { return this.ConvertValueToUnit(Mass.InDecagrams); }
        }

        /// <summary>
        /// Weight in decigrams (dg).
        /// </summary>
        public decimal? InDecigrams {
            get { return this.ConvertValueToUnit(Mass.InDecigrams); }
        }

        /// <summary>
        /// Weight in drams (dr).
        /// </summary>
        public decimal? InDrams {
            get { return this.ConvertValueToUnit(Mass.InDrams); }
        }

        /// <summary>
        /// Weight in gigagrams (Gg).
        /// </summary>
        public decimal? InGigagrams {
            get { return this.ConvertValueToUnit(Mass.InGigagrams); }
        }

        /// <summary>
        /// Weight in grains (gr).
        /// </summary>
        public decimal? InGrains {
            get { return this.ConvertValueToUnit(Mass.InGrains); }
        }

        /// <summary>
        /// Weight in drams (g).
        /// </summary>
        public decimal? InGrams {
            get { return this.ConvertValueToUnit(Mass.InGrams); }
        }

        /// <summary>
        /// Weight in hectograms (hg).
        /// </summary>
        public decimal? InHectograms {
            get { return this.ConvertValueToUnit(Mass.InHectograms); }
        }

        /// <summary>
        /// Weight in hundredweight (cwt).
        /// </summary>
        public decimal? InHundredweight {
            get { return this.ConvertValueToUnit(Mass.InHundredweight); }
        }

        /// <summary>
        /// Weight in kilograms (kg).
        /// </summary>
        public decimal? InKilograms {
            get { return this.ConvertValueToUnit(Mass.InKilograms); }
        }

        /// <summary>
        /// Weight in megagrams (Mg).
        /// </summary>
        public decimal? InMegagrams {
            get { return this.ConvertValueToUnit(Mass.InMegagrams); }
        }

        /// <summary>
        /// Weight in micrograms (µg).
        /// </summary>
        public decimal? InMicrograms {
            get { return this.ConvertValueToUnit(Mass.InMicrograms); }
        }

        /// <summary>
        /// Weight in milligrams (mg).
        /// </summary>
        public decimal? InMilligrams {
            get { return this.ConvertValueToUnit(Mass.InMilligrams); }
        }

        /// <summary>
        /// Weight in nanograms (ng).
        /// </summary>
        public decimal? InNanograms {
            get { return this.ConvertValueToUnit(Mass.InNanograms); }
        }

        /// <summary>
        /// Weight in ounce (oz).
        /// </summary>
        public decimal? InOunce {
            get { return this.ConvertValueToUnit(Mass.InOunce); }
        }

        /// <summary>
        /// Weight in pounds (lb).
        /// </summary>
        public decimal? InPounds {
            get { return this.ConvertValueToUnit(Mass.InPounds); }
        }

        /// <summary>
        /// Weight in quarter (qtr).
        /// </summary>
        public decimal? InQuarter {
            get { return this.ConvertValueToUnit(Mass.InQuarter); }
        }

        /// <summary>
        /// Weight in tonnes (t).
        /// </summary>
        public decimal? InTonnes {
            get { return this.ConvertValueToUnit(Mass.InTonnes); }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Weight()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="number">value as decimal number</param>
        /// <param name="unit">unit of value</param>
        public Weight(decimal? number, string unit)
            : this() {
            this.Number = number;
            this.Unit = unit;
        }

        /// <summary>
        /// Gets this value in another unit.
        /// </summary>
        /// <param name="targetUnit">unit to get value in</param>
        /// <returns>value in other unit</returns>
        protected decimal? ConvertValueToUnit(Unit targetUnit) {
            NumberWithFixUnit weight;
            if (this.HasValue) {
                string unit = this.Unit;
                if (Volume.TryParse(unit, out Unit volumeUnit)) {
                    // conversion from volume to mass might be inaccurate for other liquids than water
                    var volume = new NumberWithFixUnit(this.Number, volumeUnit);
                    weight = new NumberWithFixUnit(volume.ConvertValueToUnit(Volume.InLitres), Mass.InKilograms);
                } else {
                    weight = NumberWithFixUnit.Parse(this.Number, unit);
                }
            } else {
                weight = null;
            }
            return weight?.ConvertToUnit(targetUnit).Number;
        }

    }

}