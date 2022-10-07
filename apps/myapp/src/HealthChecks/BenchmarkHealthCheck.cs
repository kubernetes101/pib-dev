// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Model;
using CseLabs.Middleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace MyApp
{
    /// <summary>
    /// Benchmark Health Check
    /// </summary>
    public partial class BenchmarkHealthCheck : IHealthCheck
    {
        public static readonly string ServiceId = "MyApp";
        public static readonly string Description = "Test App Health Check";

        private static JsonSerializerOptions jsonOptions;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BenchmarkHealthCheck"/> class.
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="dal">IDAL</param>
        public BenchmarkHealthCheck(ILogger<BenchmarkHealthCheck> logger)
        {
            // save to member vars
            this.logger = logger;

            // setup serialization options
            if (jsonOptions == null)
            {
                // ignore nulls in json
                jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                };

                // serialize enums as strings
                jsonOptions.Converters.Add(new JsonStringEnumConverter());

                // serialize TimeSpan as 00:00:00.1234567
                jsonOptions.Converters.Add(new TimeSpanConverter());
            }
        }

        /// <summary>
        /// Run the health check (IHealthCheck)
        /// </summary>
        /// <param name="context">HealthCheckContext</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>HealthCheckResult</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // dictionary
            Dictionary<string, object> data = new ();

            try
            {
                HealthStatus status = HealthStatus.Healthy;

                // add instance and version
                data.Add("Instance", Environment.GetEnvironmentVariable("WEBSITE_ROLE_INSTANCE_ID") ?? "unknown");
                data.Add("Version", VersionExtension.Version);

                // Run each health check
                await GetBenchmarkAsync(data).ConfigureAwait(false);

                // overall health is the worst status
                foreach (object d in data.Values)
                {
                    if (d is HealthzCheck h && h.Status != HealthStatus.Healthy)
                    {
                        status = h.Status;
                    }

                    if (status == HealthStatus.Unhealthy)
                    {
                        break;
                    }
                }

                // return the result
                return new HealthCheckResult(status, Description, data: data);
            }
            catch (Exception ex)
            {
                // log and return unhealthy
                logger.LogError($"{ex}\nException:Healthz:{ex.Message}");

                data.Add("Exception", ex.Message);

                return new HealthCheckResult(HealthStatus.Unhealthy, Description, ex, data);
            }
        }
    }
}
