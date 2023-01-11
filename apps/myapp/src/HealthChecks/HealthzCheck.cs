// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyApp.Model
{
    /// <summary>
    /// Health Check that supports dotnet IHeathCheck
    /// </summary>
    public class HealthzCheck
    {
        public const string TimeoutMessage = "Request exceeded expected duration";

        public HealthStatus Status { get; set; }
        public string ComponentId { get; set; }
        public string ComponentType { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan TargetDuration { get; set; }
        public string Time { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        public string Endpoint { get; set; }
        public string Message { get; set; }
    }
}
