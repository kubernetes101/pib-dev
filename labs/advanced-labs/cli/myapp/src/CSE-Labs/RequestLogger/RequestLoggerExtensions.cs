// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace CseLabs.Middleware
{
    /// <summary>
    /// Middleware extension to make registering Logger easy
    ///
    /// Note: Logger should be one of the first things registered in DI
    /// </summary>
    public static class RequestLoggerExtensions
    {
        public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder, RequestLoggerOptions options = null)
        {
            // extension - use app.UseRequestLogger();
            if (options == null)
            {
                options = new RequestLoggerOptions();
            }

            return builder.UseMiddleware<RequestLogger>(Options.Create<RequestLoggerOptions>(options));
        }
    }
}
