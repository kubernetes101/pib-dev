// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CseLabs.Middleware
{
    public class CseLog
    {
        public const string MessageInvalidQueryString = "Invalid query string";

        private static readonly JsonSerializerOptions Options = new ()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        public static LogLevel LogLevel { get; set; } = LogLevel.Information;
        public static string Zone { get; set; } = string.Empty;
        public static string Region { get; set; } = string.Empty;

        public static LogEventId LogEvent400 { get; } = new LogEventId((int)HttpStatusCode.BadRequest, HttpStatusCode.BadRequest.ToString());
        public static LogEventId LogEvent404 { get; } = new LogEventId((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());
        public static LogEventId LogEvent500 { get; } = new LogEventId((int)HttpStatusCode.InternalServerError, "Exception");

        public string Name { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string NotFoundError { get; set; } = string.Empty;

        /// <summary>
        /// Log information message
        /// </summary>
        /// <param name="method">method to log</param>
        /// <param name="message">message to log</param>
        /// <param name="context">http context</param>
        /// <param name="dictionary">optional dictionary</param>
        public void LogInformation(string method, string message, HttpContext context = null, Dictionary<string, object> dictionary = null)
        {
            if (LogLevel <= LogLevel.Information)
            {
                WriteLog(LogLevel.Information, GetDictionary(method, message, LogLevel.Information, null, context, dictionary));
            }
        }

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="method">method to log</param>
        /// <param name="message">message to log</param>
        /// <param name="eventId">Event ID</param>
        /// <param name="context">http context</param>
        /// <param name="dictionary">optional dictionary</param>
        public void LogWarning(string method, string message, LogEventId eventId = null, HttpContext context = null, Dictionary<string, object> dictionary = null)
        {
            if (LogLevel <= LogLevel.Warning)
            {
                WriteLog(LogLevel.Warning, GetDictionary(method, message, LogLevel.Warning, eventId, context, dictionary));
            }
        }

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="method">method to log</param>
        /// <param name="message">message to log</param>
        /// <param name="eventId">Event ID</param>
        /// <param name="context">http context</param>
        /// <param name="ex">exception</param>
        /// <param name="dictionary">optional dictionary</param>
        public void LogError(string method, string message, LogEventId eventId = null, HttpContext context = null, Exception ex = null, Dictionary<string, object> dictionary = null)
        {
            if (LogLevel <= LogLevel.Error)
            {
                Dictionary<string, object> d = GetDictionary(method, message, LogLevel.Error, eventId, context);

                // add exception
                if (ex != null)
                {
                    d.Add("ExceptionType", ex.GetType().FullName);
                    d.Add("ExceptionMessage", ex.Message);
                }

                // add dictionary
                if (dictionary != null && dictionary.Count > 0)
                {
                    foreach (KeyValuePair<string, object> kv in dictionary)
                    {
                        d.Add(kv.Key, kv.Value);
                    }
                }

                // log the error
                WriteLog(LogLevel.Error, d);
            }
        }

        // write the log to console or console.error
        private static void WriteLog(LogLevel logLevel, Dictionary<string, object> data)
        {
            Console.ForegroundColor = logLevel switch
            {
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Warning => ConsoleColor.Yellow,
                _ => ConsoleColor.Green,
            };

            if (logLevel == LogLevel.Error)
            {
                Console.Error.WriteLine(JsonSerializer.Serialize(data, Options));
            }
            else
            {
                Console.WriteLine(JsonSerializer.Serialize(data, Options));
            }

            Console.ResetColor();
        }

        // convert log to dictionary
        private Dictionary<string, object> GetDictionary(string method, string message, LogLevel logLevel, LogEventId eventId = null, HttpContext context = null, Dictionary<string, object> dictionary = null)
        {
            Dictionary<string, object> data = new ()
            {
                { "Date", DateTime.UtcNow },
                { "LogName", Name },
                { "Method", method },
                { "Message", message },
                { "LogLevel", logLevel.ToString() },
            };

            if (context != null && context.Items != null)
            {
                data.Add("Path", RequestLogger.GetPathAndQuerystring(context.Request));
            }

            // add LogEventId
            if (eventId != null && eventId.Id > 0)
            {
                data.Add("EventId", eventId.Id);
            }

            if (eventId != null && !string.IsNullOrWhiteSpace(eventId.Name))
            {
                data.Add("EventName", eventId.Name);
            }

            // add Zone and Region
            if (!string.IsNullOrEmpty(Zone))
            {
                data.Add("Zone", Zone);
            }

            if (!string.IsNullOrEmpty(Region))
            {
                data.Add("Region", Region);
            }

            // add dictionary
            if (dictionary != null && dictionary.Count > 0)
            {
                foreach (KeyValuePair<string, object> kv in dictionary)
                {
                    data.Add(kv.Key, kv.Value);
                }
            }

            return data;
        }
    }
}
