// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MyApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyApp
{
    /// <summary>
    /// Benchmark Health Check
    /// </summary>
    public partial class BenchmarkHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Write the health check results as json
        /// </summary>
        /// <param name="httpContext">HttpContext</param>
        /// <param name="healthReport">HealthReport</param>
        /// <returns>Task</returns>
        public static Task IetfResponseWriter(HttpContext httpContext, HealthReport healthReport)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (healthReport == null)
            {
                throw new ArgumentNullException(nameof(healthReport));
            }

            // create the dictionaries
            Dictionary<string, object> result = new ();
            Dictionary<string, object> checks = new ();

            // add header values
            result.Add("status", IetfCheck.ToIetfStatus(healthReport.Status));
            result.Add("serviceId", ServiceId);
            result.Add("description", Description);

            // add all the entries
            foreach (HealthReportEntry e in healthReport.Entries.Values)
            {
                // add all the data elements
                foreach (KeyValuePair<string, object> d in e.Data)
                {
                    // transform HealthzCheck into IetfCheck
                    if (d.Value is HealthzCheck r)
                    {
                        // add to checks dictionary
                        checks.Add(d.Key, new IetfCheck(r));
                    }
                    else
                    {
                        // add to the main dictionary
                        result.Add(d.Key, d.Value);
                    }
                }
            }

            // add the checks to the dictionary
            result.Add("checks", checks);

            // write the json
            httpContext.Response.ContentType = "application/health+json";
            httpContext.Response.StatusCode = healthReport.Status == HealthStatus.Unhealthy ? (int)System.Net.HttpStatusCode.ServiceUnavailable : (int)System.Net.HttpStatusCode.OK;
            return httpContext.Response.WriteAsync(JsonSerializer.Serialize(result, jsonOptions));
        }

        /// <summary>
        /// Write the Health Check results as json
        /// </summary>
        /// <param name="httpContext">HttpContext</param>
        /// <param name="res">HealthCheckResult</param>
        /// <param name="totalTime">TimeSpan</param>
        /// <returns>Task</returns>
        public static Task IetfResponseWriter(HttpContext httpContext, HealthCheckResult res, TimeSpan totalTime)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            // Convert the HealthCheckResult to a HealthReport
            HealthReport rpt = new (
                new Dictionary<string, HealthReportEntry> { { BenchmarkHealthCheck.ServiceId, new HealthReportEntry(res.Status, res.Description, totalTime, res.Exception, res.Data) } },
                totalTime);

            // call the response writer
            return IetfResponseWriter(httpContext, rpt);
        }
    }
}
