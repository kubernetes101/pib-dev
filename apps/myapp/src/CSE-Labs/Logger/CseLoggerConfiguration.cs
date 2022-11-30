// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;

namespace CseLabs.Middleware
{
    /// <summary>
    /// Logger Configuration
    /// </summary>
    public class CseLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
    }
}
