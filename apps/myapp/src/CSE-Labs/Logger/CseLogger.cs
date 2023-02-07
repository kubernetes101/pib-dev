// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CseLabs.Middleware
{
    /// <summary>
    /// Simple aspnet core middleware that logs requests to the console
    /// </summary>
    public class CseLogger : ILogger
    {
        private readonly string name;
        private readonly CseLoggerConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="CseLogger"/> class.
        /// </summary>
        /// <param name="name">Logger Name</param>
        /// <param name="config">Logger Config</param>
        public CseLogger(string name, CseLoggerConfiguration config)
        {
            this.name = name;
            this.config = config;
        }

        public static string Zone { get; set; } = string.Empty;
        public static string Region { get; set; } = string.Empty;

        public IDisposable BeginScope<TState>(TState state)
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= config.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            Dictionary<string, object> d = new ()
            {
                { "logName", name },
                { "logLevel", logLevel.ToString() },
                { "eventId", eventId.Id },
                { "eventName", eventId.Name },
            };

            if (!string.IsNullOrEmpty(Zone))
            {
                d.Add("Zone", Zone);
            }

            if (!string.IsNullOrEmpty(Region))
            {
                d.Add("Region", Region);
            }

            // convert state to list
            if (state is IReadOnlyList<KeyValuePair<string, object>> roList)
            {
                List<KeyValuePair<string, object>> list = roList.ToList();

                switch (list.Count)
                {
                    case 0:
                        break;
                    case 1:
                        // clean up name
                        list.Add(new KeyValuePair<string, object>("message", list[0].Value));
                        list.RemoveAt(0);
                        break;
                    default:
                        // remove formatting key-value
                        list.RemoveAt(list.Count - 1);
                        break;
                }

                HttpContext c = null;

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    // add from HttpContext.Items
                    if (c == null && list[i].Value is HttpContext)
                    {
                        c = list[i].Value as HttpContext;

                        // add specific items here
                    }
                    else
                    {
                        d.Add(list[i].Key.ToString(), list[i].Value == null ? string.Empty : list[i].Value.ToString());
                    }
                }
            }

            // add exception
            if (exception != null)
            {
                d.Add("Exception", exception.Message);
            }

            if (logLevel >= LogLevel.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(JsonSerializer.Serialize(d));
            }
            else
            {
                Console.ForegroundColor = logLevel == LogLevel.Warning ? ConsoleColor.Yellow : Console.ForegroundColor;
                Console.WriteLine(JsonSerializer.Serialize(d));
            }

            Console.ResetColor();

            // free the memory for GC
            d.Clear();
        }
    }
}
