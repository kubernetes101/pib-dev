// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace CseLabs.Middleware
{
    /// <summary>
    /// Logger options used to configure DI
    /// </summary>
    public class RequestLoggerOptions
    {
        public bool Log2xx { get; set; } = true;
        public bool Log3xx { get; set; } = true;
        public bool Log4xx { get; set; } = true;
        public bool Log5xx { get; set; } = true;
        public double TargetMs { get; set; } = 1000;
    }
}
