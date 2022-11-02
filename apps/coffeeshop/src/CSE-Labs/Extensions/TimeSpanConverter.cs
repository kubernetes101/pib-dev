// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CseLabs.Middleware
{
    /// <summary>
    /// Custom TimeSpan Converter
    ///
    /// 00:00:00.123
    /// </summary>
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeSpan.Parse(reader.GetString(), CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            // write timespan to milliseconds
            writer.WriteStringValue(value.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture));
        }
    }
}
