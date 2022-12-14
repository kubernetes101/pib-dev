// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace CseLabs.Middleware
{
    /// <summary>
    /// Nullable EventId class for logging
    /// </summary>
    public class LogEventId
    {
        public LogEventId(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
