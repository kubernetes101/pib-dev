// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace CseLabs.Middleware
{
    /// <summary>
    /// cse-labs Logger Extensions
    /// </summary>
    public static class CseLoggerExtensions
    {
        public static ILoggingBuilder AddJsonLogger(this ILoggingBuilder builder)
        {
            return builder.AddJsonLogger(new CseLoggerConfiguration());
        }

        public static ILoggingBuilder AddJsonLogger(this ILoggingBuilder builder, Action<CseLoggerConfiguration> configure)
        {
            CseLoggerConfiguration config = new ();
            configure(config);

            return builder.AddJsonLogger(config);
        }

        public static ILoggingBuilder AddJsonLogger(this ILoggingBuilder builder, CseLoggerConfiguration config)
        {
            return builder.AddProvider(new CseLoggerProvider(config));
        }
    }
}
