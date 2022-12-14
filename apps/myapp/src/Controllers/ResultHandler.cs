// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CseLabs.Middleware;
using CseLabs.Middleware.Validation;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    /// <summary>
    /// Handles query requests from the controllers
    /// </summary>
    public static class ResultHandler
    {
        /// <summary>
        /// Handle an IActionResult request from a controller
        /// </summary>
        /// <typeparam name="T">type of result</typeparam>
        /// <param name="task">async task (usually the query)</param>
        /// <param name="logger">Log</param>
        /// <returns>IActionResult</returns>
        public static async Task<IActionResult> Handle<T>(Task<T> task, CseLog logger)
        {
            // log the request
            logger.LogInformation("Handle<T>", "DS request");

            // return exception if task is null
            if (task == null)
            {
                logger.LogError("Handle<T>", "Exception: task is null", CseLog.LogEvent500, ex: new ArgumentNullException(nameof(task)));

                return CreateResult(logger.ErrorMessage, HttpStatusCode.InternalServerError);
            }

            try
            {
                // return an OK object result
                return new OkObjectResult(await task.ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                // log and return exception
                logger.LogError("Handle<T>", "Exception", CseLog.LogEvent500, ex: ex);

                // return 500 error
                return CreateResult("Internal Server Error", HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// ContentResult factory
        /// </summary>
        /// <param name="message">string</param>
        /// <param name="statusCode">int</param>
        /// <returns>JsonResult</returns>
        public static JsonResult CreateResult(string message, HttpStatusCode statusCode)
        {
            JsonResult res = new (new ErrorResult { Error = statusCode, Message = message })
            {
                StatusCode = (int)statusCode,
            };

            return res;
        }

        public static JsonResult CreateResult(List<ValidationError> errorList, string path)
        {
            Dictionary<string, object> data = new ()
            {
                { "type", "ParameterValidationError" },
                { "title", "Parameter validation error" },
                { "detail", "One or more invalid parameters were specified." },
                { "status", (int)HttpStatusCode.BadRequest },
                { "instance", path },
                { "validationErrors", errorList },
            };

            JsonResult res = new (data)
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ContentType = "application/problem+json",
            };

            return res;
        }
    }
}
