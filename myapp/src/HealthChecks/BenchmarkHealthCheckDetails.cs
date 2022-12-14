// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MyApp.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyApp
{
    /// <summary>
    /// Benchmark Health Check
    /// </summary>
    public partial class BenchmarkHealthCheck : IHealthCheck
    {
        private const int MaxResponseTime = 200;
        private readonly Stopwatch stopwatch = new ();

        /// <summary>
        /// Build the response
        /// </summary>
        /// <param name="uri">string</param>
        /// <param name="targetDurationMs">double (ms)</param>
        /// <param name="ex">Exception (default = null)</param>
        /// <param name="data">Dictionary(string, object)</param>
        /// <param name="testName">Test Name</param>
        /// <returns>HealthzCheck</returns>
        private HealthzCheck BuildHealthzCheck(string uri, double targetDurationMs, Exception ex = null, Dictionary<string, object> data = null, string testName = null)
        {
            stopwatch.Stop();

            // create the result
            HealthzCheck result = new ()
            {
                Endpoint = uri,
                Status = HealthStatus.Healthy,
                Duration = stopwatch.Elapsed,
                TargetDuration = new System.TimeSpan(0, 0, 0, 0, (int)targetDurationMs),
                ComponentId = testName,
                ComponentType = "datastore",
            };

            // check duration
            if (result.Duration.TotalMilliseconds > targetDurationMs)
            {
                result.Status = HealthStatus.Degraded;
                result.Message = HealthzCheck.TimeoutMessage;
            }

            // add the exception
            if (ex != null)
            {
                result.Status = HealthStatus.Unhealthy;
                result.Message = ex.Message;
            }

            // add the results to the dictionary
            if (data != null && !string.IsNullOrEmpty(testName))
            {
                data.Add(testName + ":responseTime", result);
            }

            return result;
        }

        /// <summary>
        /// Get Genres Healthcheck
        /// </summary>
        /// <returns>HealthzCheck</returns>
        private async Task<HealthzCheck> GetBenchmarkAsync(Dictionary<string, object> data = null)
        {
            const string name = "getBenchmark";
            const string path = "/api/benchmark/256";

            stopwatch.Restart();

            try
            {
                // TODO - implement
                await Task.Delay(10);

                return BuildHealthzCheck(path, MaxResponseTime, null, data, name);
            }
            catch (Exception ex)
            {
                BuildHealthzCheck(path, MaxResponseTime, ex, data, name);

                // throw the exception so that HealthCheck logs
                throw;
            }
        }
    }
}
