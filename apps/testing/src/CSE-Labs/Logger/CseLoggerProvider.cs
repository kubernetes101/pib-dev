// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace CseLabs.Middleware
{
    /// <summary>
    /// cse-labs Logger Provider
    /// </summary>
    public sealed class CseLoggerProvider : ILoggerProvider
    {
        private readonly CseLoggerConfiguration config;
        private readonly ConcurrentDictionary<string, CseLogger> loggers = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="CseLoggerProvider"/> class.
        /// </summary>
        /// <param name="loggerConfig">JSON Logger Config</param>
        public CseLoggerProvider(CseLoggerConfiguration loggerConfig)
        {
            config = loggerConfig;
        }

        /// <summary>
        /// Create a logger by category name (usually assembly)
        /// </summary>
        /// <param name="categoryName">Category Name</param>
        /// <returns>ILogger</returns>
        public ILogger CreateLogger(string categoryName)
        {
            CseLogger logger = loggers.GetOrAdd(categoryName, new CseLogger(categoryName, config));
            return logger;
        }

        /// <summary>
        /// IDispose.Dispose()
        /// </summary>
        public void Dispose()
        {
            loggers.Clear();
        }
    }
}
