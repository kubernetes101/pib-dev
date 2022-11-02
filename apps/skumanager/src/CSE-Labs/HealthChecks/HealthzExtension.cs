// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;

namespace CseLabs.Middleware
{
    /// <summary>
    /// Registers aspnet middleware handler that handles /healthz
    /// </summary>
    public static class HealthzExtension
    {
        // cached values
        private static byte[] responseBytes;

        /// <summary>
        /// Middleware extension method to handle /healthz request
        /// </summary>
        /// <param name="builder">this IApplicationBuilder</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseHealthz(this IApplicationBuilder builder)
        {
            responseBytes ??= System.Text.Encoding.UTF8.GetBytes("pass");

            // implement the middleware
            builder.Use(async (context, next) =>
            {
                string path = "/healthz";

                // matches /healthz
                if (context.Request.Path.ToString().Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    // return the healthz info
                    context.Response.ContentType = "text/plain";

                    await context.Response.Body.WriteAsync(responseBytes).ConfigureAwait(false);
                }
                else
                {
                    // not a match, so call next middleware handler
                    await next().ConfigureAwait(false);
                }
            });

            return builder;
        }
    }
}
