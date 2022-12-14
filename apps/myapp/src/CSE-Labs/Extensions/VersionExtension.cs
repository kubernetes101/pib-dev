// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace CseLabs.Middleware
{
    /// <summary>
    /// Registers aspnet middleware handler that handles /version
    /// </summary>
    public static class VersionExtension
    {
        // cached values
        private static byte[] responseBytes;
        private static string version = string.Empty;
        private static string shortVersion = string.Empty;

        /// <summary>
        /// Gets the app version
        /// </summary>
        public static string Version => version;

        /// <summary>
        /// Gets the app short version
        /// </summary>
        public static string ShortVersion => shortVersion;

        /// <summary>
        /// Cache version values with reflection
        /// </summary>
        public static void Init()
        {
            // cache the version info
            if (Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyInformationalVersionAttribute)) is AssemblyInformationalVersionAttribute v)
            {
                version = v.InformationalVersion;
                shortVersion = version;

                if (version.Contains('-', StringComparison.OrdinalIgnoreCase))
                {
                    shortVersion = version[..version.IndexOf('-', StringComparison.OrdinalIgnoreCase)];
                }
            }
        }

        /// <summary>
        /// Middleware extension method to handle /version request
        /// </summary>
        /// <param name="builder">this IApplicationBuilder</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseVersion(this IApplicationBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                Init();
            }

            responseBytes = System.Text.Encoding.UTF8.GetBytes(version);

            // implement the middleware
            builder.Use(async (context, next) =>
            {
                string path = "/version";

                // matches /version
                if (context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase))
                {
                    // return the version info
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
