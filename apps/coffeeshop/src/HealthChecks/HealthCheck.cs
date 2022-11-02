// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CseLabs.Middleware;
using CoffeeShop.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace CoffeeShop
{
    /// <summary>
    /// Health Check
    /// </summary>
    public partial class HealthCheck : IHealthCheck
    {
        public static readonly string ServiceId = "CoffeeShop";
        public static readonly string Description = "CoffeeShop Health Check";

        private const int MaxResponseTime = 200;
        private static JsonSerializerOptions jsonOptions;
        private readonly Stopwatch stopwatch = new ();
        private readonly ILogger logger;
        private Database db = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheck"/> class.
        /// </summary>
        /// <param name="logger">ILogger</param>
        public HealthCheck(ILogger<HealthCheck> logger)
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

                // add version
                data.Add("Version", App.Config.Version);

                // Run each health check
                DbCheck(data);

                // todo - make async
                await Task.Delay(1, CancellationToken.None);

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
                logger.LogError("Exception:Healthz:{ex.Message}", ex);

                data.Add("Exception", ex.Message);

                return new HealthCheckResult(HealthStatus.Unhealthy, Description, ex, data);
            }
        }

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
        /// Healthcheck
        /// </summary>
        /// <returns>HealthzCheck</returns>
        private HealthzCheck DbCheck(Dictionary<string, object> data = null)
        {
            const string name = "dbCheck";
            string path = "/";

            stopwatch.Restart();

            try
            {
                db = new();

                if (db == null)
                {
                    Exception ex = new NullReferenceException("Database::New is null");
                    return BuildHealthzCheck(path, MaxResponseTime, ex, data, name);
                }

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
