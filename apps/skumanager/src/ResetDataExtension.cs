// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using Microsoft.AspNetCore.Builder;

namespace SkuManager
{
    /// <summary>
    /// Registers aspnet middleware handler that handles /api/reset-data
    /// </summary>
    public static class ResetDataExtension
    {
        // cached values
        private const string Path = "/api/v1/reset-data";
        private static byte[] responseBytes;

        /// <summary>
        /// Middleware extension method to handle /api/reset-data request
        /// </summary>
        /// <param name="builder">this IApplicationBuilder</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseResetData(this IApplicationBuilder builder)
        {
            responseBytes ??= System.Text.Encoding.UTF8.GetBytes("reset");

            // implement the middleware
            builder.Use(async (context, next) =>
            {
                // matches path
                if (context.Request.Path.ToString().Equals(Path, StringComparison.OrdinalIgnoreCase))
                {
                    // delete the data file - use the data from swagger
                    if (File.Exists(App.Config.DataFilePath))
                    {
                        File.Delete(App.Config.DataFilePath);
                    }

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
