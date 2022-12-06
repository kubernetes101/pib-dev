// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyApp.Model
{
    /// <summary>
    /// Health Check that supports IETF json
    /// </summary>
    public class IetfCheck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IetfCheck"/> class.
        /// </summary>
        public IetfCheck()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IetfCheck"/> class.
        /// Create an IetfCheck from a HealthzCheck
        /// </summary>
        /// <param name="hzCheck">HealthzCheck</param>
        public IetfCheck(HealthzCheck hzCheck)
        {
            if (hzCheck == null)
            {
                throw new ArgumentNullException(nameof(hzCheck));
            }

            Status = ToIetfStatus(hzCheck.Status);
            ComponentId = hzCheck.ComponentId;
            ComponentType = hzCheck.ComponentType;
            ObservedValue = Math.Round(hzCheck.Duration.TotalMilliseconds, 2);
            TargetValue = Math.Round(hzCheck.TargetDuration.TotalMilliseconds, 0);
            ObservedUnit = "ms";
            Time = hzCheck.Time;
            Message = hzCheck.Message;

            if (hzCheck.Status != HealthStatus.Healthy && !string.IsNullOrEmpty(hzCheck.Endpoint))
            {
                AffectedEndpoints = new List<string> { hzCheck.Endpoint };
            }
        }

        public string Status { get; set; }
        public string ComponentId { get; set; }
        public string ComponentType { get; set; }
        public string ObservedUnit { get; set; }
        public double ObservedValue { get; set; }
        public double TargetValue { get; set; }
        public string Time { get; set; }
        public List<string> AffectedEndpoints { get; }
        public string Message { get; set; }

        /// <summary>
        /// Convert the dotnet HealthStatus to the IETF Status
        /// </summary>
        /// <param name="status">HealthStatus (dotnet)</param>
        /// <returns>string</returns>
        public static string ToIetfStatus(HealthStatus status)
        {
            return status switch
            {
                HealthStatus.Healthy => "pass",
                HealthStatus.Degraded => "warn",
                _ => "fail"
            };
        }
    }
}
