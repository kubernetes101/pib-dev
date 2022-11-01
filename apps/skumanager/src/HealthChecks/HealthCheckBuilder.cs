// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SkuManager
{
    public static class HealthCheckBuilder
    {
        public static IHealthChecksBuilder AddHealthCheck(
            this IHealthChecksBuilder builder,
            string name,
            HealthStatus? failureStatus = null,
            IEnumerable<string> tags = null)
        {
            // Register a check of type Benchmark
            return builder.AddCheck<HealthCheck>(name, failureStatus ?? HealthStatus.Degraded, tags);
        }
    }
}
