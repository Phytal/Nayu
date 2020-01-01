﻿using System.Text.Json.Serialization;

namespace Victoria.Payloads {
    /// <summary>
    ///     Equalizer band
    /// </summary>
    public struct EqualizerBand {
        /// <summary>
        ///     15 bands (0-14) that can be changed.
        /// </summary>
        [JsonPropertyName("band")]
        public ushort Number { get; set; }

        /// <summary>
        ///     Gain is the multiplier for the given band. The default value is 0. Valid values range from -0.25 to 1.0,
        ///     where -0.25 means the given band is completely muted, and 0.25 means it is doubled.
        /// </summary>
        [JsonPropertyName("gain")]
        public double Gain { get; set; }
    }
}